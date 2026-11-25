using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.Controls.AxisControl;

[RequireComponent(typeof(UISpring))]
public class UiSpringFill : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] Image chipImage;
    [SerializeField] Image imageFill;
    [SerializeField] UISpring springComp;
    [Space(10)]

    [Header("Spring Settings")]
    [MinMaxSlider(2f, 50f)][SerializeField] float frequency = 14f;
    [MinMaxSlider(0f, 2f)] [SerializeField] float dampening = 0.8f;

    [Header("Nudge Settings")]
    [SerializeField] float damageStr = 10f;
    [SerializeField] float healStr = -6f;

    private float currentFill = 1f;
    private float fillVelocity = 0f;
    private float targetFill = 1f;

    private SpringUtils.tDampedSpringMotionParams motionParams = new();

    // Update is called once per frame
    void Update()
    {
        SpringUtils.CalcDampedSpringMotionParams(
            ref motionParams,
            Time.deltaTime,
            frequency,
            dampening
        );

        SpringUtils.UpdateDampedSpringMotion(
            ref currentFill,
            ref fillVelocity,
            targetFill,
            in motionParams
        );

        if (chipImage != null) {
            chipImage.fillAmount = Mathf.Clamp01(currentFill);
        }
    }

    [Button("Test half")]
    public void TestHealth() {
        SetHealth(0.5f);
    }

    [Button("Test 0.2")]
    public void TestHealth2() {
        SetHealth(0.2f);
    }

    [Button("Test 0.9")]
    public void TestHealth3() {
        SetHealth(0.9f);
    }

    [Button("Snap full")]
    public void TestHealth4() {
        SnapHealth(1);
    }

    public void SetHealth(float normalizedHealth) {
        float clampedHealth = Mathf.Clamp01(normalizedHealth);
        float delta = clampedHealth - targetFill;
        targetFill = clampedHealth;

        if (imageFill != null) {
            imageFill.fillAmount = targetFill;
        }

        if (springComp != null && Mathf.Abs(delta) > 0.001f) {
            springComp.Nudge(delta < 0f ? damageStr : healStr);
        }
    }

    public void SnapHealth(float normalizedHealth) {
        float clampedFloat = Mathf.Clamp01(normalizedHealth);
        currentFill = clampedFloat;
        targetFill = clampedFloat;
        fillVelocity = 0;

        if (chipImage != null) chipImage.fillAmount = clampedFloat;
        if (imageFill != null) imageFill.fillAmount = clampedFloat;
    }
}
