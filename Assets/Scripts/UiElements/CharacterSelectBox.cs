using UnityEngine;

public class CharacterSelectBox : MonoBehaviour
{
    public bool connectedPlayer = false;

    [SerializeField] GameObject disconnectedBlocker;


    public void ActivateSelect() { //might have to pass the gamepad here to allow for changing buttons here.
        connectedPlayer = true;
        disconnectedBlocker.SetActive(false);
    }

    public void DisconnectPlayer() { //Remove hooks here.
        connectedPlayer = false;
        disconnectedBlocker.SetActive(true);
    }
}
