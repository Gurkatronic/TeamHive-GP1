using DamageNumbersPro;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUiBox : MonoBehaviour
{
    [SerializeField] UiSpringFill healthBar;
    [SerializeField] Image playerBar;
    [SerializeField] Image rageBarFill;
    [SerializeField] TextMeshProUGUI scoreMesh;
    [SerializeField] DamageNumberGUI pointPop;
    public int currentScore = 0;

    private void Awake() {
        
    }

    public void FillBox() {
        healthBar.SnapHealth(0);
        healthBar.SetHealth(1);
        //take in player information and fill the boxes accordingly
        currentScore = 0; // can be changed so save previous levels or something
        scoreMesh.text = currentScore.ToString();
        rageBarFill.fillAmount = 0;
        gameObject.SetActive(true);
    }

    public void AddScore(int amount) {

        currentScore += amount;
        //added some basic floating number to the Ui part. This is a prefab that can be modified.
        DamageNumber newPop = pointPop.SpawnGUI(scoreMesh.rectTransform, new Vector3(0, 0, 0), amount);
        scoreMesh.text = currentScore.ToString(); //Maybe some counter animation can be added here
    }

    [Button("Test popping numbers")]
    public void TestNumberPop() {
        int amount = Random.Range(10, 50);
        DamageNumber newPop = pointPop.SpawnGUI(scoreMesh.rectTransform, new Vector3(0, 0, 0), amount);
    }
}
