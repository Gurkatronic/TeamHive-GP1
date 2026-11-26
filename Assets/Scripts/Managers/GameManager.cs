using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] PlayerInputManager playerInputManager;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] RoomEnemySpawner roomEnemySpawner;
    [SerializeField] public UiManager uiManager;
    [SerializeField] SessionPackage currentSession;
    [SerializeField] PlayableCharacterScriptableObject[] debugPlayerScripts;
    int currentPlayerCount;
    public bool debugmode = false;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        

        if (roomEnemySpawner == null) {
            roomEnemySpawner = FindAnyObjectByType<RoomEnemySpawner>();
        }
    }

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
        if(debugmode) {
            currentSession.activePlayers = debugPlayerScripts.ToList();
        }
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
            PlayerLogic pLogic = player.GetComponent<PlayerLogic>();
            if(pLogic == null) {
                Debug.Log("Player logic can not be found on the spawned player.");
            }
            else {
                pLogic.playerIndex = i;
            }
        }

        scoreManager = new(currentPlayerCount);
        uiManager.StartClock();
        SoundManager.Instance.StartMusic();
        roomEnemySpawner.spawnActive = true;
    }
    public void KickPlayer() {
        
    }

    public void GameLost() {
        uiManager.GameOverShow();
        SoundManager.Instance.StopMusic();
        Time.timeScale = 0;
    }

    public void GameWon() {
        uiManager.GameWonShow();
        SoundManager.Instance.StopMusic();
        Time.timeScale = 0;
    }

    public void RestartPrototype() {
        Application.Quit();
    }

}
