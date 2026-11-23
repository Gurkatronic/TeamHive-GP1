using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUiBox : MonoBehaviour
{
    [SerializeField] Image playerBar;
    [SerializeField] Image healthBarFill;
    [SerializeField] Image rageBarFill;
    [SerializeField] TextMeshProUGUI scoreMesh;
    public int currentScore = 0;

    private void Awake() {
        
    }

    public void FillBox() {
        //take in player information and fill the boxes accordingly
        ShowScore(0); // can be changed so save previous levels or something
        rageBarFill.fillAmount = 0;
        healthBarFill.fillAmount = 1;
        gameObject.SetActive(true);
    }

    public void ShowScore(int amount) {
        currentScore = amount;
        scoreMesh.text = currentScore.ToString(); //Maybe some counter animation can be added here
    }
}
