using TMPro;
using UnityEngine;

public class PlayerScoreShow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerText;
    [SerializeField] TextMeshProUGUI scoreText;

    public void ShowScore(string playername, string score) {
        playerText.text = playername;
        scoreText.text = score;
    }

}
