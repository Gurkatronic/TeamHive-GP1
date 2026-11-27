using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLogic : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerSounds playerSounds;
    private AudioSource audioSource;
    public int playerIndex;
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
    public float knockedDownTime = 10f;
    public float killStreakTime;
    [SerializeField] private float killstreakDuration = 3f;
    public float time = 0f;
    public int score = 0;
    public int pointsPerKill = 100;
    public float streakBonusPerKill = 0.1f;
    public float relicScoreMultiplier = 2f;
    public bool hasRelic = false;
    public float relicSpeedMultiplier = 1f;
    public bool stunned = false;
    public float stunTimeLeft = 0f;
    public float throwChargeStartTime;
    private float throwCharge;
    [SerializeField] int scoreLossOnDeath = 1000;
    private Combat combat;

    private void Awake()
    {
        combat = GetComponent<Combat>();
        audioSource = GetComponent<AudioSource>();
    }

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
        if (killStreakTime < time)
        {
            killStreak = 0;
            if (rageMode)
            {
                RageEnded();
            }
        }
        if (knockedDown)
        {
            knockedDownTime -= Time.deltaTime;
            if (knockedDownTime <= 0f)
            {
                Revive();
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
            GameManager.Instance.uiManager.IncreaseScore(playerIndex, Mathf.RoundToInt(pointsPerKill * GetScoreMultiplier()));
            killStreak++;
            killStreakTime = time + killstreakDuration;
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
        audioSource.PlayOneShot(GetSoundEffect(PlayerSoundType.Hurt));

        if (hasRelic)
        {
            Relic.Instance.Drop();
        }

        if (health <= 0)
        {
            KnockedDown();
            return 1;
        }
        return 0;
    }

    public void KnockedDown()
    {
        if (stunned)
        {
            stunned = false;
        }
        knockedDown = true;
        audioSource.PlayOneShot(GetSoundEffect(PlayerSoundType.Dead));
        if (rageMode) RageEnded();
        knockedDownTime = time;
        // animation switch to knocked down state
        score -= scoreLossOnDeath;
        GameManager.Instance.uiManager.IncreaseScore(playerIndex, -scoreLossOnDeath);
    }
    public void Revive()
    {
        knockedDown = false;
        health = 100;
        killStreak = 0;
        // animation switch to normal state
    }

    public void ThrowRelicPlayer(InputAction.CallbackContext context)
    {
        if (hasRelic)
        {
            if (throwChargeStartTime == 0f)
            {
                throwChargeStartTime = time;
            }
            if (context.canceled)
            {
                if (time - throwChargeStartTime > 1f) throwCharge = 1f;
                else throwCharge = time - throwChargeStartTime;
                Relic.Instance.ThrowRelic(throwCharge);
            }

        }
    }
    public void RageEnded()
    {
        rageMode = false;
        combat.rageParticleSystem.Stop();
    }
    public void SendAudioClips(PlayerSounds psound) {
        this.playerSounds = psound;
        Debug.Log("GIVE ME SOUNDS");
    }


    [Button("Test give 10 score")]
    public void TestScorePop()
    {
        GameManager.Instance.uiManager.IncreaseScore(playerIndex, 10);
    }


    public AudioClip GetSoundEffect(PlayerSoundType type) {
        if (playerSounds.soundDict.ContainsKey(type)) {
            return playerSounds.soundDict[type];
        }
        else
            return null;
    }
}
