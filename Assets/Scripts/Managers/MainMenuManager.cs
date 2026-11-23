using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

[Serializable]
public class MenuScreen {
    public GameObject screen;
    public GameObject buttonFirstSelect;
}

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;
    [Serializable]
    public class MainMenuNavigationSounds {
        public AudioClip hoverSound;
        public AudioClip clickSound;
    }
    public enum MenuState {
        MainMenu,
        Options,
        CharacterSelect,
        Credits
    }

    [SerializeField] MenuState currentState = MenuState.MainMenu;
    [SerializeField] AudioSource audioSource;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] GameObject characterSelectWindow;
    [SerializeField] Dictionary<MenuState, MenuScreen> menuScreens = new();
    [SerializeField] MainMenuNavigationSounds navSounds;

    [Header("Character select components")]
    [SerializeField] CharacterSelectBox[] characterSelectBoxes;
    GameObject lastSelectedObj;

    public void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SwapScreen(MenuState.MainMenu);
    }
    public void Update() {
        if (lastSelectedObj == null) return;
        if (lastSelectedObj != eventSystem.currentSelectedGameObject) {
            lastSelectedObj = eventSystem.currentSelectedGameObject;
            audioSource.PlayOneShot(navSounds.hoverSound);
        }

    }

    public void GoCharacterSelect() {
        SwapScreen(MenuState.CharacterSelect);
        ControllerDetector.Instance.ChangeState(ControllerDetectorState.CharacterSelect);
        PlayerSlot[] playerSlots = ControllerDetector.Instance.slots;
        for(int i = 0; i < playerSlots.Length; i++) {
            if (playerSlots[i].isConnected) {
                characterSelectBoxes[i].ActivateSelect(i, playerSlots[i].gamepad);
            }
        }
    }
    public void GoMainMenu() {
        SwapScreen(MenuState.MainMenu);
        ControllerDetector.Instance.ChangeState(ControllerDetectorState.Detection);
    }

    public void SwapScreen(MenuState menuState) {
        currentState = menuState;
        foreach(MenuState state in menuScreens.Keys) {
            menuScreens[state].screen?.SetActive(false);
        }

        MenuScreen currentScreen = menuScreens[currentState];
        currentScreen.screen.SetActive(true);
        eventSystem.SetSelectedGameObject(currentScreen.buttonFirstSelect);
        lastSelectedObj = currentScreen.buttonFirstSelect;
        audioSource.PlayOneShot(navSounds.clickSound);

    }

    public void ConnectPlayerToSelect(PlayerSlot playerSlot) {
        characterSelectBoxes[playerSlot.slotIndex].ActivateSelect(playerSlot.slotIndex, playerSlot.gamepad);
    }

    public void DisconnectPlayerToSelect(int index) {
        characterSelectBoxes[index].DisconnectPlayer();
    }


}
