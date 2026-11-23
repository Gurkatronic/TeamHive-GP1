using NaughtyAttributes;
using UnityEngine;


public class UiManager : MonoBehaviour
{
    [SerializeField] PlayerUiBox[] playerUiBoxes;


    [Button("TestShow")]
    public void ShowUi(int playerAmount) {
        for(int i = 0; i < playerAmount && i < playerUiBoxes.Length; i++) {
            playerUiBoxes[i].FillBox();
        }
    }
    [Button("HIDE")]
    public void ResetUi() {
        for(int i = 0; i < playerUiBoxes.Length; i++) {
            playerUiBoxes[i].gameObject.SetActive(false);
        }
    }
}
