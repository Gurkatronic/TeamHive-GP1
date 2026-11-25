using NaughtyAttributes;
using System;
using UnityEngine;

public class UISpring : MonoBehaviour {
    [Header("Spring Tuning")]
    [Tooltip("How fast/snappy the spring reacts (e.g. 12 - 25 for crisp UI).")]
    [SerializeField] private float angularFrequency = 18f;
    [Tooltip("< 1 = bouncy, 1 = smooth/no overshoot, > 1 = sluggish.")]
    [Range(0f, 2f)]
    [SerializeField] private float dampingRatio = 0.65f;

    [Header("State")]
    public float position = 0f;
    public float velocity = 0f;
    public float targetPosition = 0f;

    public event Action<float> OnSpringValue;

    private SpringUtils.tDampedSpringMotionParams motionParams = new();

    private void Update() {
        SpringUtils.CalcDampedSpringMotionParams(
            ref motionParams,
            Time.unscaledDeltaTime,
            angularFrequency,
            dampingRatio
        );

        SpringUtils.UpdateDampedSpringMotion(
            ref position,
            ref velocity,
            targetPosition,
            in motionParams
        );

        OnSpringValue?.Invoke(position);
    }

    /// <summary>
    /// Adds an instant velocity impulse (great for button clicks, lock-in pops, or attract bounces).
    /// </summary>
    [Button("Test nudge")]
    public void TestNudge() {
        Nudge(100);
    }
    public void Nudge(float impulse) {
        velocity += impulse;
    }
    [Button("Test Snap")]
    public void TestSnap() {
        SnapTo(0);
    }

    /// <summary>
    /// Snaps immediately to a value (e.g., -1 when hiding off-screen before animating in to 0).
    /// </summary>
    public void SnapTo(float value, float initialVelocity = 0f) {
        position = value;
        targetPosition = value;
        velocity = initialVelocity;
        OnSpringValue?.Invoke(position);
    }
}