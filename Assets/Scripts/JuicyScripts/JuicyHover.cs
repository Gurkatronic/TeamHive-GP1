using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;

public class JuicyHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler
{
    public Vector3 goalPos;
    public Vector3 endPos;
    public Vector3 startPos;

    public float frequency = 15f;
    public float dampingRatio = 0.5f;
    UISpring uiSpring;
    bool hovered = false;

    private void Awake() {
        uiSpring = GetComponent<UISpring>();
    }

    private void Update() {


    }

    public void OnPointerEnter(PointerEventData eventData) {
        uiSpring.Nudge(frequency);
    }

    public void OnPointerExit(PointerEventData eventData) {

    }

    public void OnPointerClick(PointerEventData eventData) {
        uiSpring.SnapTo(-1);
    }

    public void OnSelect(BaseEventData eventData) {
        uiSpring.Nudge(frequency);
    }
}
