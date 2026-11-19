using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleMoveController : MonoBehaviour {
    [Header("Movement Settings")]
    [SerializeField] private CharacterController controller;
    public float moveSpeed = 6f;
    public float rotationSpeed = 720f;
    public float gravity = -20f;
    private float verticalVelocity = 0f;

    [Header("Punch Settings")]
    public float punchRange = 1.5f;
    public float punchRadius = 0.75f;
    public Vector3 punchOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private LayerMask punchableLayers = ~0; // Defaults to Everything

    private void Awake() {
        if (controller == null) {
            controller = GetComponent<CharacterController>();
        }
    }

    private void Update() {
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.E)) {
            PerformPunch();
        }
    }

    private void HandleMovement() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(horizontal, 0f, vertical).normalized;

        // Rotate smoothly toward movement direction
        if (moveDir.sqrMagnitude > 0.01f) {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Apply simple gravity so the player stays grounded
        if (controller.isGrounded && verticalVelocity < 0f) {
            verticalVelocity = -2f;
        }
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalVelocity = (moveDir * moveSpeed) + (Vector3.up * verticalVelocity);
        controller.Move(finalVelocity * Time.deltaTime);
    }

    private void PerformPunch() {
        Vector3 checkOrigin = transform.position + punchOffset + (transform.forward * punchRange);
        Collider[] hitColliders = Physics.OverlapSphere(checkOrigin, punchRadius, punchableLayers);

        // Track already-punched targets in case an object has multiple child colliders
        HashSet<IPunchable> punchedTargets = new HashSet<IPunchable>();

        for (int i = 0; i < hitColliders.Length; i++) {
            IPunchable punchable = hitColliders[i].GetComponentInParent<IPunchable>();
            if (punchable != null && punchedTargets.Add(punchable)) {
                punchable.Impact(transform.position);
            }
        }
    }

    // Visualizes the punch hitbox in the Scene view
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Vector3 checkOrigin = transform.position + punchOffset + (transform.forward * punchRange);
        Gizmos.DrawWireSphere(checkOrigin, punchRadius);
    }
}