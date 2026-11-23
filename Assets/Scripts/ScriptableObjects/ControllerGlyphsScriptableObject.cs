using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerButtons {
    ControllerSymbol,
    // Face Buttons (Platform-Agnostic / Xbox / PlayStation / Switch)
    ButtonSouth,        // Xbox: A | PS: Cross | Switch: B
    ButtonEast,         // Xbox: B | PS: Circle | Switch: A
    ButtonWest,         // Xbox: X | PS: Square | Switch: Y
    ButtonNorth,        // Xbox: Y | PS: Triangle | Switch: X

    // D-Pad
    DpadUp,
    DpadDown,
    DpadLeft,
    DpadRight,
    DpadAll,            // Full D-Pad icon

    // Shoulders & Triggers
    LeftShoulder,       // LB / L1 / L
    RightShoulder,      // RB / R1 / R
    LeftTrigger,        // LT / L2 / ZL
    RightTrigger,       // RT / R2 / ZR

    // Stick Clicks
    LeftStickPress,     // L3 / LS
    RightStickPress,    // R3 / RS

    // Left Stick Directions
    LeftStickUp,
    LeftStickDown,
    LeftStickLeft,
    LeftStickRight,
    LeftStickHorizontal,
    LeftStickVertical,
    LeftStickAll,       // Full Left Stick icon

    // Right Stick Directions
    RightStickUp,
    RightStickDown,
    RightStickLeft,
    RightStickRight,
    RightStickHorizontal,
    RightStickVertical,
    RightStickAll,      // Full Right Stick icon

    // System / Center Buttons
    Start,              // Start / Options / +
    Select,             // Back / View / Share / Create / -
    System,             // Xbox Guide / PS Button / Home
    TouchpadPress,      // PS4 / PS5 Touchpad click
    Capture             // Switch Capture / Xbox Series Share
}



[CreateAssetMenu(fileName = "ControllerGlyphsScriptableObject", menuName = "Ui interface/Controller Glyph package")]
public class ControllerGlyphsScriptableObject : ScriptableObject
{
    [SerializeField] private Dictionary<ControllerButtons, Sprite> glyphLibrary = new();



    public Sprite GetControllerGlyph(ControllerButtons button) {
        if (glyphLibrary.ContainsKey(button))
            return glyphLibrary[button];
        else {
            Debug.Log("Button glyph is missing");
            return null;
        }
    }

    [Button("Populate all Dictionary slots slots")]
    void PopulateDictionary() {
        int enumAmount = Enum.GetValues(typeof(ControllerButtons)).Length;
        for(int i = 0; i < enumAmount; i++) {
            glyphLibrary.Add((ControllerButtons)i, null);
        }
    }
}
