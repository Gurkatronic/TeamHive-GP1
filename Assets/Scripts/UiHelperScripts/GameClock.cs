
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class GameClock : MonoBehaviour
{
    [SerializeField] private Transform clockArm;
    [SerializeField] private Image clockBgFill;
    [SerializeField] public float maxTime;
    [SerializeField] float currentTime;

    bool finished = false;
    public bool clockWise = false;

    private void Awake() {
        clockBgFill.fillClockwise = clockWise;
    }

    private void Update() {
        if (finished)
            return;
            currentTime += Time.deltaTime;
        if (currentTime >= maxTime) {
            clockArm.rotation = Quaternion.Euler(0, 0, 0);
            clockBgFill.fillAmount = 1f;
            finished = true;
        }
        else {
            float rotateValue = 360 / maxTime;
            if(clockWise)
                clockArm.Rotate(new Vector3(0, 0, -rotateValue * Time.deltaTime));
            else
                clockArm.Rotate(new Vector3(0, 0, rotateValue * Time.deltaTime));
            clockBgFill.fillAmount = currentTime / maxTime;
        }
    }

    [Button("Reset Clock")]
    void ResetClock() {
        currentTime = 0;
        clockArm.rotation = Quaternion.Euler(0, 0, 0);
        clockBgFill.fillAmount = 0;
        finished = false;
    }
}
