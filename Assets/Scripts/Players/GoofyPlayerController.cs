using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Rigidbody))]
public class GoofyPlayerController : MonoBehaviour {
    [Header("Hover Spring")]
    [SerializeField] private float rideHeight = 1.2f;
    [SerializeField] private float rayLength = 2.0f;
    [SerializeField] private float rideSpringStrength = 250f;
    [SerializeField] private float rideSpringDamper = 20f;
    [SerializeField] private LayerMask groundLayers;

    [Header("Upright Torque - If this is set incorrect the player will spin violently")]
    [SerializeField] private float uprightSpringStrength = 25f;
    [SerializeField] private float uprightSpringDamper = 8f;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float maxAccelForce = 150f;
    [Tooltip("X = Dot product (-1 opposite dir, 0 perpendicular, 1 same dir). Y = Multiplier (e.g. 2 at -1, 1 at 1).")]
    [SerializeField]
    private AnimationCurve accelerationFactorFromDot = new AnimationCurve(
        new Keyframe(-1f, 2f),
        new Keyframe(0f, 1.5f),
        new Keyframe(1f, 1f)
    );
    [SerializeField] private float maxAccelForceFactorFromDot = 2f;
    [SerializeField] private Vector3 forceScale = new Vector3(1f, 0f, 1f);

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [SerializeField] private float lowJumpGravityMultiplier = 3f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;
    private PlayerLogic playerLogic;

    private Rigidbody rb;
    private Transform cameraTransform;

    private Vector2 moveInput;
    private Vector3 goalVelocity;
    private Quaternion targetOrientation = Quaternion.identity;

    private bool isGrounded;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool jumpHeld;

    PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction jumpAction;
    [SerializeField] bool isMoving = false;

    [Header("Animation Controller")]
    [SerializeField] AnimationControl animationControl;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        animationControl = GetComponentInChildren<AnimationControl>();
        rb.constraints = RigidbodyConstraints.None;
        playerLogic = GetComponent<PlayerLogic>();

        //map the controls
        playerInput = GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Player");

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];

        //assign the input actions here, assigning dynamic functions wont work on multiple controllers
        if (moveAction != null) {
            moveAction.performed += OnMove;
            moveAction.canceled += OnStop;
        }
        if (jumpAction != null) {
            jumpAction.started += OnJump;
            jumpAction.canceled += OnJump;
        }


        if (Camera.main != null) {
            cameraTransform = Camera.main.transform;
        }
    }

    private void OnDisable() {
        if (moveAction != null) {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnStop;
        }

        if (jumpAction != null) {
            jumpAction.started -= OnJump;
            jumpAction.canceled -= OnJump;
        }
    }

    private void LateUpdate() {
        animationControl.SetRun(isMoving);
    }

    public void OnMove(InputAction.CallbackContext context) {
        if (playerLogic.stunned || playerLogic.knockedDown)
        {
            isMoving = false;
            return;
        }
        moveInput = context.ReadValue<Vector2>();
        isMoving = true;
    }

    public void OnStop(InputAction.CallbackContext context) {
        isMoving = false;
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (playerLogic.stunned || playerLogic.knockedDown) return;
        if (context.started) {
            jumpBufferTimer = jumpBufferTime;
            jumpHeld = true;
        }
        else if (context.canceled) {
            jumpHeld = false;
        }
    }

    private void Update() {

        if (isGrounded) {
            coyoteTimer = coyoteTime;
        }
        else {
            coyoteTimer -= Time.deltaTime;
        }

        if (jumpBufferTimer > 0f) {
            jumpBufferTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate() {
        UpdateFloating();
        UpdateUprightForce();
        UpdateMovement();
        HandleJumpAndFall();
    }

    private void UpdateFloating() { //This is to handle the spring, with a raycast
        Ray downRay = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(downRay, out RaycastHit rayHit, rayLength, groundLayers, QueryTriggerInteraction.Ignore)) {
            isGrounded = rayHit.distance <= rideHeight * 1.15f;

            Vector3 vel = rb.linearVelocity;
            Vector3 rayDir = Vector3.down;

            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = rayHit.rigidbody;
            if (hitBody != null) {
                otherVel = hitBody.linearVelocity;
            }

            float rayDirVel = Vector3.Dot(rayDir, vel);
            float otherDirVel = Vector3.Dot(rayDir, otherVel);
            float relVel = rayDirVel - otherDirVel;

            float x = rayHit.distance - rideHeight;
            float springForce = (x * rideSpringStrength) - (relVel * rideSpringDamper);

            rb.AddForce(rayDir * springForce);

            if (hitBody != null) {
                hitBody.AddForceAtPosition(rayDir * -springForce, rayHit.point);
            }
        }
        else {
            isGrounded = false;
        }
    }

    private void UpdateUprightForce() {
        Quaternion toGoal = targetOrientation * Quaternion.Inverse(transform.rotation);
        toGoal.ToAngleAxis(out float rotDegrees, out Vector3 rotAxis);

        if (rotDegrees > 180f) {
            rotDegrees -= 360f;
        }

        if (Mathf.Abs(rotDegrees) < 0.01f || rotAxis.sqrMagnitude < 0.0001f) {
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, uprightSpringDamper * Time.fixedDeltaTime);
            return;
        }

        rotAxis.Normalize();
        float rotRadians = rotDegrees * Mathf.Deg2Rad;

        Vector3 springTorque = rotAxis * (rotRadians * uprightSpringStrength);
        Vector3 dampingTorque = rb.angularVelocity * uprightSpringDamper;

        rb.AddTorque(springTorque - dampingTorque, ForceMode.Acceleration);
    }

    private void UpdateMovement() { //This uses the cameras angle to handle input works good, Might be a issue if we move the camera
        Vector3 moveDir = GetCameraRelativeInput();

        if (moveDir.sqrMagnitude > 0.01f) {
            targetOrientation = Quaternion.LookRotation(moveDir, Vector3.up);
        }

        Vector3 unitGoal = moveDir;
        Vector3 unitVel = goalVelocity.normalized;
        float velDot = Vector3.Dot(unitGoal, unitVel);

        float accel = acceleration * accelerationFactorFromDot.Evaluate(velDot);
        Vector3 targetVel = unitGoal * maxSpeed;

        goalVelocity = Vector3.MoveTowards(goalVelocity, targetVel, accel * Time.fixedDeltaTime);

        Vector3 neededAccel = (goalVelocity - rb.linearVelocity) / Time.fixedDeltaTime;
        float maxAccel = maxAccelForce * Mathf.Lerp(maxAccelForceFactorFromDot, 1f, Mathf.Clamp01((velDot + 1f) * 0.5f));
        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

        rb.AddForce(Vector3.Scale(neededAccel * rb.mass, forceScale));
    }

    private void HandleJumpAndFall() {
        if (jumpBufferTimer > 0f && coyoteTimer > 0f) {
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            isGrounded = false;
        }

        if (rb.linearVelocity.y < 0f) {
            rb.AddForce(Physics.gravity * (fallGravityMultiplier - 1f), ForceMode.Acceleration);
        }
        else if (rb.linearVelocity.y > 0f && !jumpHeld) {
            rb.AddForce(Physics.gravity * (lowJumpGravityMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    private Vector3 GetCameraRelativeInput() {
        if (moveInput.sqrMagnitude < 0.001f) return Vector3.zero;

        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);
        if (input.sqrMagnitude > 1f) input.Normalize();

        if (cameraTransform == null) return input;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        return (camForward * input.z) + (camRight * input.x);
    }
}