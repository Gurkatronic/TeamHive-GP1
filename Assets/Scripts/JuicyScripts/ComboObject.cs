using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public interface IPunchable {
    void Impact(Vector3 fromPos);
}

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class ComboObject : MonoBehaviour, IPunchable {
    [Header("Combo Settings")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;
    [SerializeField] AudioSource audioSource;
    public int maxPunch = 3;
    public float comboWindow = 1.0f;
    private int currentPunch = 0;
    private float comboTimer = 0f;
    private bool isComboActive = false;
    private Vector3 lastHitDir;

    [Header("Shake Juice (Hits 1 to Max-1)")]
    public float shakeDuration = 0.25f;
    public float shakeIntensity = 0.15f;
    [Tooltip("Curve from 0 to 1 controlling the vertical jank up and back down to 0.")]
    [SerializeField]
    private AnimationCurve shakeBounceCurve;
    public float bounceHeight = 0.5f;
    [Tooltip("Layers that will NOT be pushed around while this object is shaking.")]
    [SerializeField] private LayerMask shakeExcludeLayers;
    private LayerMask defaultExcludeLayers;

    [Header("Launch Settings")]
    [SerializeField] private AnimationCurve flyCurve;
    [SerializeField] private AnimationCurve flyUpCurve;
    public float forcePower = 25f;
    public float forcePowerUp = 35f;
    [Range(0.1f, 1f)] public float droppedForceMultiplier = 0.35f;
    public float upwardLift = 0.5f;
    public float spinForce = 10f;
    [Header("Debug testing")]
    [SerializeField] InputAction testPunch;
    [SerializeField] InputAction resetObject;

    private Coroutine punchRoutine;
    private Vector3 startPos;
    private Quaternion startRot;
    private Vector3 anchorPos;
    public Action attacked;

    private void Awake() {
        startPos = transform.position;
        startRot = transform.rotation;
        anchorPos = startPos;

        if (rb == null) {
            rb = GetComponent<Rigidbody>();
        }

        if (col == null) {
            col = GetComponent<Collider>();
        }

        if(audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        defaultExcludeLayers = col.excludeLayers;

        testPunch.Enable();
        testPunch.performed += TestPunch;
        resetObject.Enable();
        resetObject.performed += TestReset;
    }

    private void Update() {


        if (isComboActive) {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f) {
                DropCombo();
            }
        }
    }

    void TestPunch(InputAction.CallbackContext callbackContext) {
        Impact(transform.position + Vector3.forward);
    }

    void TestReset(InputAction.CallbackContext callbackContext) {
        ResetObject();
    }

    public void Impact(Vector3 fromPos) {
        if (currentPunch >= maxPunch) {
            return;
        }
        attacked?.Invoke();
        //Sound goes here
        //SoundManager.Instance.PlayRandomSound(RandomAudioType.GuitarRiff, audioSource);                
        anchorPos = transform.position;

        if (punchRoutine != null) {
            StopCoroutine(punchRoutine);
            transform.position = anchorPos;
            col.excludeLayers = defaultExcludeLayers;
        }

        currentPunch++;
        lastHitDir = (transform.position - fromPos);
        lastHitDir.y = 0f;
        lastHitDir.Normalize();

        if (currentPunch < maxPunch) {
            isComboActive = true;
            comboTimer = comboWindow;
            punchRoutine = StartCoroutine(ShakeRoutine(lastHitDir));
        }
        else {
            isComboActive = false;
            //Finished combo
            punchRoutine = StartCoroutine(FlyAnimation(lastHitDir, 1f));
            //SoundManager.Instance.PlaySingleSound(SingleAudioType.ComboSuccess, audioSource);
        }
    }

    private void DropCombo() {
        isComboActive = false;
        Debug.Log("Combo failed");
        //SoundManager.Instance.PlaySingleSound(SingleAudioType.ComboFail, audioSource);
        currentPunch = maxPunch;


        if (punchRoutine != null) {
            StopCoroutine(punchRoutine);
            transform.position = anchorPos;
            col.excludeLayers = defaultExcludeLayers;
        }

        punchRoutine = StartCoroutine(FlyAnimation(lastHitDir, droppedForceMultiplier));
    }

    private IEnumerator ShakeRoutine(Vector3 hitDir) {
        rb.isKinematic = true;
        col.excludeLayers = defaultExcludeLayers | shakeExcludeLayers; //Disable collision checks against this so items to build up momentum when touched

        anchorPos += hitDir * 0.1f;

        float elapsed = 0f;
        float currentIntensity = shakeIntensity * currentPunch;

        while (elapsed < shakeDuration) {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / shakeDuration;

            float verticalOffset = shakeBounceCurve.Evaluate(normalizedTime) * bounceHeight;
            float damper = 1f - normalizedTime;
            Vector3 randomShake = Random.insideUnitSphere * currentIntensity * damper;
            randomShake.y *= 0.25f;

            transform.position = anchorPos + (Vector3.up * verticalOffset) + randomShake;
            yield return null;
        }

        transform.position = anchorPos;
        col.excludeLayers = defaultExcludeLayers; //Restore collision check
    }

    private IEnumerator FlyAnimation(Vector3 forceDir, float powerMultiplier) {
        //There could be a material flickering with white here so its easy to tell that its finished.
        //Particle effect could also work here
        col.excludeLayers = defaultExcludeLayers;
        rb.isKinematic = false;

        Vector3 launchDir = (forceDir + Vector3.up * upwardLift).normalized;
        rb.AddTorque(Random.insideUnitSphere * (spinForce * powerMultiplier), ForceMode.Impulse);

        float maxTime = 0.4f;
        float currentTime = 0f;
        float finalPower = forcePower * powerMultiplier;
        float finalUpPower = forcePowerUp * powerMultiplier;

        while (currentTime < maxTime) {
            currentTime += Time.deltaTime;
            float normalizedTime = currentTime / maxTime;

            float newForce = flyCurve.Evaluate(normalizedTime) * finalPower;
            float newUpForce = flyUpCurve.Evaluate(normalizedTime) * finalUpPower;
            rb.AddForce(launchDir * newForce, ForceMode.Acceleration);
            rb.AddForce(Vector3.up * newUpForce, ForceMode.Acceleration);

            yield return null;
        }

        ResetCombo(); //reset combo after flying so it can recieve damage again
        yield return null;

    }

    void ResetCombo() {
        currentPunch = 0;
        isComboActive = false;
        comboTimer = 0;
    }

    [Button("Reset Object")]
    public void ResetObject() {
        if (punchRoutine != null) {
            StopCoroutine(punchRoutine);
        }
        col.excludeLayers = defaultExcludeLayers;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = startPos;
        transform.rotation = startRot;
        anchorPos = startPos;
        currentPunch = 0;
        isComboActive = false;
        comboTimer = 0f;
    }
}