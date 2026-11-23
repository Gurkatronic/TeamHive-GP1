using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelectBox : MonoBehaviour
{
    public bool connectedPlayer = false;
    public bool lockedIn = false;
    private int slotIndex;
    private Gamepad gamepad;

    [SerializeField] GameObject disconnectedBlocker;
    [SerializeField] GameObject lockedInScreen;


    public void ActivateSelect(int slotIndex, Gamepad gamepad) { //might have to pass the gamepad here to allow for changing buttons here.
        this.slotIndex = slotIndex;
        this.gamepad = gamepad;
        connectedPlayer = true;
        disconnectedBlocker.SetActive(false);
    }

    public void DisconnectPlayer() { //Remove hooks here.
        gamepad = null;
        connectedPlayer = false;
        lockedIn = false;
        lockedInScreen.SetActive(false);
        disconnectedBlocker.SetActive(true);
    }

    private void Update() {
        if (!connectedPlayer || gamepad == null) return;

        if(gamepad.buttonEast.wasPressedThisFrame) {
            if(lockedIn)
                LockOut();
            else {
                ControllerDetector.Instance.PlayerDisconnect(slotIndex);
            }
        }

        if(gamepad.buttonSouth.wasPressedThisFrame) {
            if (lockedIn)
                return;
            else {
                LockIn();
            }
        }
    }

    void LockIn() {
        lockedIn = true;
        lockedInScreen.SetActive(true);
    }

    void LockOut() {
        lockedIn = false;
        lockedInScreen.SetActive(false);
    }
}
