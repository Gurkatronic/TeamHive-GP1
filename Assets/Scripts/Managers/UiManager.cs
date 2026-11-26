using NaughtyAttributes;
using UnityEngine;


public class UiManager : MonoBehaviour
{
    [SerializeField] GameClock gameClock;
    [SerializeField] PlayerUiBox[] playerUiBoxes;
    [SerializeField] PlayerScoreShow[] playerScoreShow;
    [SerializeField] GameObject scoreScreen;
    [SerializeField] GameObject gameOverScreen;
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

    public void IncreaseScore(int playerIndex, int amount) {
        playerUiBoxes[playerIndex].AddScore(amount);
    }

    public void GameWonShow() {
        for(int i = 0; i < playerUiBoxes.Length; i++) {
            if (playerUiBoxes[i].isActiveAndEnabled) {
                playerScoreShow[i].ShowScore($"Player {i+1}",playerUiBoxes[i].currentScore.ToString());
            }
        }
        scoreScreen.SetActive(true);
    }

    public void GameOverShow() {
        gameOverScreen.SetActive(true);
    }

    public void StartClock() {
        gameClock.StartClock();
    }
}
