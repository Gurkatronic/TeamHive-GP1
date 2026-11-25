using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.Utilities;
using System;
using UnityEngine.InputSystem.Users;
using NaughtyAttributes;

public enum ControllerTypes {
    Playstation,
    Xbox,
    Xinput
}

[Serializable]
public class PlayerSlot {
    public int slotIndex;
    public InputUser user;
    public Gamepad gamepad;
    public bool isConnected;
    public ControllerGlyphsScriptableObject controllerGlyphs;
}

public enum ControllerDetectorState {
    CharacterSelect,
    Gameplay,
    Detection
}

public class ControllerDetector : MonoBehaviour
{
    public static ControllerDetector Instance;

    [SerializeField] private Dictionary<ControllerTypes, ControllerGlyphsScriptableObject> masterGlyphLibrary;
    public PlayerSlot[] slots = new PlayerSlot[4];
    [SerializeField] ControllerDetectorState currentState = ControllerDetectorState.CharacterSelect;
    //this one will follow around the scenes so it will not be destroyed,
    //handle what happens when you add a controller or drop a controller in scenes
    [SerializeField] List<Gamepad> activeGamepads;
    IDisposable anyButtonListener;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < slots.Length; i++) {
            slots[i] = new PlayerSlot { slotIndex = i };
        }

        
    }

    private void OnEnable() {
        anyButtonListener = InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable() {
        anyButtonListener?.Dispose();
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
    private void Update() {
        if(Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) {
            
        }
    }

    public void ChangeState(ControllerDetectorState newState) {
        currentState = newState;
    }

    void OnDeviceChange(InputDevice device, InputDeviceChange change) {
        if (device is not Gamepad gamepad) return;

        if (change == InputDeviceChange.Disconnected) {
            for (int i = 0; i < slots.Length; i++) {
                if (slots[i].isConnected && slots[i].gamepad == gamepad) {
                    slots[i].isConnected = false;
                    PlayerDisconnect(i);
                    Debug.Log($"Player {i} controller disconnected!");
                }
            }
        }
    }
    private void OnAnyButtonPress(InputControl control) {
        //Debug.Log("Some button pressed");
        if(currentState == ControllerDetectorState.Detection) {
            Debug.Log("Controller detector is just looking right now");
            return;
        }
        //Check what type of device it is. Should map this so keyboard can be used for one controller scheme
        // Ignore keyboards, mice, or non-gamepad devices
        if (control.device is not Gamepad gamepad) {
            //Debug.Log("This is not a gamepad");
            return;
        }

        if (control.parent is UnityEngine.InputSystem.Controls.StickControl) {
            return; 
        }


        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].isConnected && slots[i].gamepad == gamepad)
                return;
        }

        for(int i = 0; i < slots.Length; i++) {
            PlayerSlot playerSlot = slots[i];
            if (!playerSlot.isConnected) {
                playerSlot.user = InputUser.PerformPairingWithDevice(gamepad);
                playerSlot.gamepad = gamepad;
                playerSlot.controllerGlyphs = CheckControllerGlyphs(gamepad);
                playerSlot.isConnected = true;
                Debug.Log($"Player {i} joined with gamepad {gamepad.displayName}");
                OnPlayerJoined(i);
                return;
            }
        }
    }

    [Button("Save the controllers to persistant")]
    public void SaveControllers() {
        List<PlayerSlot> activeSlots = new();
        for(int i = 0; i < slots.Length; i++) {
            if (slots[i].isConnected)
                activeSlots.Add(slots[i]);
        }

        PersistantMemory.Instance.SaveControllers(activeSlots);
    }

    public void OnPlayerJoined(int playerId) { 
        switch(currentState) {
            case ControllerDetectorState.CharacterSelect:
                MainMenuManager mainMenu = MainMenuManager.Instance;
                if(mainMenu == null) {
                    Debug.Log("Controller detector cant find main menu manager");
                    return;
                }
                mainMenu.ConnectPlayerToSelect(slots[playerId]);
                //ADD from characterselect 
                //this should call the mainmenu script
                Debug.Log("This is characterselect join");
                break;
            case ControllerDetectorState.Gameplay:
                Debug.Log("This is GameplayJoin");
                break;
        }
    }

    public void PlayerDisconnect(int playerId) {
        switch (currentState) {
            case ControllerDetectorState.CharacterSelect:
                Debug.Log("Lost input in character select");
                MainMenuManager mainMenu = MainMenuManager.Instance;
                if (mainMenu == null) {
                    Debug.Log("Controller detector cant find main menu manager");
                    return;
                }
                mainMenu.DisconnectPlayerToSelect(playerId);
                //Remove from character select window
                //this should talk to the main menu script
                break;
            case ControllerDetectorState.Gameplay:
                Debug.Log("Lost input in game");


                break;
        }
    }

    ControllerGlyphsScriptableObject CheckControllerGlyphs(Gamepad gamepad) {
        string gamepadName = gamepad.name;
        gamepadName = gamepadName.ToLower();

        foreach(ControllerTypes key in masterGlyphLibrary.Keys) {
            foreach (string keyword in masterGlyphLibrary[key].controllerKeywords) {
                if(gamepadName.Contains(keyword, StringComparison.OrdinalIgnoreCase)) {
                    return masterGlyphLibrary[key];
                }
            }
        }

        return null;
    }

}
