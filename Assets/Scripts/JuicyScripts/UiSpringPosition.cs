using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UISpringPosition : MonoBehaviour {
    [SerializeField] private UISpring spring;
    [SerializeField] private Vector2 moveOffset = new Vector2(0f, -300f);

    private RectTransform rectTransform;
    private Vector2 initialAnchoredPos;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        initialAnchoredPos = rectTransform.anchoredPosition;
        if (spring == null) spring = GetComponent<UISpring>();
    }

    private void OnEnable() => spring.OnSpringValue += HandleSpringValue;
    private void OnDisable() => spring.OnSpringValue -= HandleSpringValue;

    private void HandleSpringValue(float value) {
        rectTransform.anchoredPosition = initialAnchoredPos + (moveOffset * value);
    }
}