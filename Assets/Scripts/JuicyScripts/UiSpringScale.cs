using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UISpringScale : MonoBehaviour {
    [SerializeField] private UISpring spring;
    [SerializeField] private Vector3 baseScale = Vector3.one;
    [SerializeField] private Vector3 scaleOffset = new Vector3(0.15f, -0.15f, 0f);
    [Tooltip("Use Abs(value) when driving intro (-1) -> rest (0) -> outro (1) transitions.")]
    [SerializeField] private bool useAbsoluteValue = false;

    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        if (spring == null) spring = GetComponent<UISpring>();
    }

    private void OnEnable() => spring.OnSpringValue += HandleSpringValue;
    private void OnDisable() => spring.OnSpringValue -= HandleSpringValue;

    private void HandleSpringValue(float value) {
        float t = useAbsoluteValue ? Mathf.Abs(value) : value;
        rectTransform.localScale = baseScale + (scaleOffset * t);
    }
}