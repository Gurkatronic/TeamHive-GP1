using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerInputManager playerInputManager;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] UiManager uiManager;
    [SerializeField] SessionPackage currentSession;
    int currentPlayerCount;
    public bool debugmode = false;


    private void Start() {
        if (!debugmode) {
            Debug.Log("Starting session");
            SessionPackage sessionPackage = PersistantMemory.Instance.StartSession();
            currentSession = sessionPackage;
            currentPlayerCount = sessionPackage.playerSlots.Count;
            scoreManager = new(currentPlayerCount);
            Debug.Log("playerCount: " + currentPlayerCount);
            uiManager.ShowUi(currentPlayerCount);
        }
    }

    [Button("Test start with session package")]
    public void TestSessionPackage() {
        SessionPackage sessionPackage = PersistantMemory.Instance.StartSession();
        currentSession = sessionPackage;
        currentPlayerCount = sessionPackage.playerSlots.Count;
        scoreManager = new(currentPlayerCount);
        Debug.Log("playerCount: " + currentPlayerCount);
        uiManager.ShowUi(currentPlayerCount);
    }

    [Button("SpawnPrefabs with scriptableobjs")]
    public void TestSpawnPlayers() {
        for(int i = 0; i < currentSession.playerSlots.Count; i++) {
            PlayerSlot playerSlot = currentSession.playerSlots[i];
            GameObject prefab = currentSession.activePlayers[i].characterPrefab;
            PlayerInput player = PlayerInput.Instantiate(prefab, playerIndex: playerSlot.slotIndex, controlScheme: "Gamepad", pairWithDevice: playerSlot.gamepad);
        }
    }
    public void KickPlayer() {
        
    }

}
