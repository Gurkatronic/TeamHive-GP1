using UnityEngine;

public class PlayerLogic : MonoBehaviour, IDamageable
{
    public int health = 100;
    public int damage = 10;
    public int killStreak = 0;
    public int streakNeededForRage = 10;
    public bool knockedDown = false;
    public bool rageMode = false;
    public float attackCooldown = 0.2f;
    public float lastAttackTime = 0f;
    public float attackDuration = 0.05f;
    public float rageAttackCooldown = 0.05f;
    public float rageAttackDuration = 0.02f;
    public float time = 0f;
    public int score = 0;
    public int pointsPerKill = 100;
    public float streakBonusPerKill = 0.1f;
    public float relicScoreMultiplier = 2f;
    public bool hasRelic = false;
    public float relicSpeedMultiplier = 1f;
    public bool stunned = false;
    public float stunTimeLeft = 0f;


    private void Update()
    {
        time += Time.deltaTime;

        if (stunned)
        {
            stunTimeLeft -= Time.deltaTime;
            if (stunTimeLeft <= 0f)
            {
                stunned = false;
            }
        }
    }

    public float GetScoreMultiplier()
    {
        float multiplier = 1f + streakBonusPerKill * killStreak;
        if (hasRelic)
        {
            multiplier *= relicScoreMultiplier;
        }
        return multiplier;
    }

    public void AddKills(int kills)
    {
        for (int i = 0; i < kills; i++)
        {
            score += Mathf.RoundToInt(pointsPerKill * GetScoreMultiplier());
            killStreak++;
        }
    }

    public void Stun(float duration)
    {
        stunned = true;
        stunTimeLeft = duration;
    }

    public int OnDamage(int damage)
    {
        if (knockedDown) return 0;

        Debug.Log("player took damage: " + damage);
        health -= damage;

        if (hasRelic)
        {
            Relic.Instance.Drop();
        }

        if (health <= 0)
        {
            knockedDown = true;
            return 1;
        }
        return 0;
    }
}
