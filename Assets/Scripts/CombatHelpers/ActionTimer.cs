using UnityEngine;

public class ActionTimer 
{
    public float maxTime = 1f;
    public float currentTime = 0;
    public float nextCountDelay = 0f;

    /// <summary>
    /// Create a timer that will add time with updatetime
    /// </summary>
    /// <param name="maxTime">How long until it has hit its target</param>
    /// <param name="nextCountDelay">When it hits it target it should have negation to the current time to increase time to reach its max next time.</param>
    public ActionTimer(float maxTime, float nextCountDelay = 0) {
        this.maxTime = maxTime;
        this.nextCountDelay = nextCountDelay;
        currentTime = 0;
    }

    public bool UpdateTimer() {
        if(currentTime < maxTime) {
            currentTime += Time.deltaTime;
            return false;
        }
        else {
            currentTime = 0;
            currentTime -= nextCountDelay;
            return true;
        }
    }

    public void ResetTimer() {
        currentTime = 0;
    }
}
