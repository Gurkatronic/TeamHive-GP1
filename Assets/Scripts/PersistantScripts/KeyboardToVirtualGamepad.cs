using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XInput;

public class KeyboardToVirtualGamepad : MonoBehaviour {
    [Header("Toggle Virtual Controller")]
    public Key toggleControllerKey = Key.F1;

    private XInputController virtualPad;

    private void Update() {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // Press F1 to plug/unplug the virtual Xbox 360 controller
        if (kb[toggleControllerKey].wasPressedThisFrame) {
            if (virtualPad == null)
                virtualPad = InputSystem.AddDevice<XInputController>("Virtual 360 Pad 1");
            else {
                InputSystem.RemoveDevice(virtualPad);
                virtualPad = null;
            }
        }

        if (virtualPad == null) return;

        // Map WASD to Left Stick
        Vector2 stick = Vector2.zero;
        if (kb.wKey.isPressed) stick.y += 1f;
        if (kb.sKey.isPressed) stick.y -= 1f;
        if (kb.aKey.isPressed) stick.x -= 1f;
        if (kb.dKey.isPressed) stick.x += 1f;

        // Build gamepad state from keyboard keys
        GamepadState state = new GamepadState {
            leftStick = stick.normalized
        };

        // Map Space -> A (South), E -> B (East), Enter -> Start
        if (kb.spaceKey.isPressed) state.buttons |= 1 << (int)GamepadButton.South;
        if (kb.eKey.isPressed) state.buttons |= 1 << (int)GamepadButton.East;
        if (kb.enterKey.isPressed) state.buttons |= 1 << (int)GamepadButton.Start;

        InputSystem.QueueStateEvent(virtualPad, state);
    }

    private void OnDisable() {
        if (virtualPad != null) {
            InputSystem.RemoveDevice(virtualPad);
            virtualPad = null;
        }
    }
}