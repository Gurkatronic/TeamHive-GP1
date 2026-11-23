using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManagerX : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform[] spawnPoints;

    ControllerDetector controllerDetector;
    PlayerInput[] players;

    private void Start()
    {
        controllerDetector = FindAnyObjectByType<ControllerDetector>();
        players = new PlayerInput[controllerDetector.slots.Length];
    }

    private void Update()
    {
        foreach (PlayerSlot slot in controllerDetector.slots)
        {
            PlayerInput player = players[slot.slotIndex];

            if (player == null)
            {
                if (slot.isConnected)
                {
                    SpawnPlayer(slot);
                }
                continue;
            }

            if (!player.isActiveAndEnabled) continue;

            if (!slot.isConnected)
            {
                if (player.devices.Count > 0)
                {
                    player.user.UnpairDevices();
                }
            }
            else if (player.devices.Count != 1 || player.devices[0] != slot.gamepad)
            {
                player.SwitchCurrentControlScheme("Gamepad", slot.gamepad);
            }
        }
    }

    void SpawnPlayer(PlayerSlot slot)
    {
        PlayerInput player = PlayerInput.Instantiate(playerPrefab, playerIndex: slot.slotIndex, controlScheme: "Gamepad", pairWithDevice: slot.gamepad);

        if (slot.slotIndex < spawnPoints.Length)
        {
            player.transform.position = spawnPoints[slot.slotIndex].position;
        }

        players[slot.slotIndex] = player;
    }
}
