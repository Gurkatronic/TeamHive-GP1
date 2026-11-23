using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class SessionPackage {
    public List<PlayerSlot> playerSlots = new();
    public List<PlayableCharacterScriptableObject> activePlayers = new();
}


public class PersistantMemory : MonoBehaviour
{
    public static PersistantMemory Instance;
    [SerializeField] SessionPackage debugPackage;
    [SerializeField] SessionPackage lastSessionPackage;
    public bool DebugMode = false;


    private void Awake() {
        if(Instance != this) {
            Instance = this;
        }

        //DontDestroyOnLoad(this);
    }

    public SessionPackage StartSession() {
        if (DebugMode)
            return debugPackage;
        return lastSessionPackage; //need a way to check if the last session package is truly null
    }

    /// <summary>
    /// This should take in the characters that have been chosen in main menu. Should happen on locking in.
    /// </summary>
    public void SaveControllers(List<PlayerSlot> playerSlots) {
        lastSessionPackage.playerSlots = playerSlots;
    }

    public void CreateSession() {

    }
}
