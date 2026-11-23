using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControllerButtonImage : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public ControllerButtons useButton;
    public Image glyphImage;
    public bool alwaysOn = false;
    public bool selected = false;
    public bool active = false;


    public void Update() {
        if(alwaysOn) {
            ShowGlyph();
        }
        else {

        
            if(selected && !active) {
                ShowGlyph();
            }

            if(!selected && active) {
                glyphImage.enabled = false;
                active = false;
            }
        }
    }


    public void ShowGlyph() {
        Debug.Log("HELELEODADOAPDA!??!?!");
        ControllerGlyphsScriptableObject controllerGlyphs = ControllerDetector.Instance.slots[0].controllerGlyphs;
        if (controllerGlyphs == null) {
            glyphImage.enabled = false;
            return;
        }
        glyphImage.sprite = controllerGlyphs.GetControllerGlyph(useButton);
        glyphImage.enabled = true;
        active = true;
    }

    public void OnSelect(BaseEventData eventData) {
        selected = true;
    }

    public void OnDeselect(BaseEventData eventData) {
        selected = false;
    }
}
