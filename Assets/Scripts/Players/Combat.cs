using UnityEngine;

public class Combat : MonoBehaviour
{
    PlayerLogic playerLogic;
    [SerializeField] BoxCollider attackHitbox;

    private void Awake()
    {
        playerLogic = GetComponent<PlayerLogic>();
    }
    public void OnAttack()
    {
        if (playerLogic.stunned) return;

        if (playerLogic.time - playerLogic.lastAttackTime >= playerLogic.attackCooldown)
        {
            playerLogic.lastAttackTime = playerLogic.time;
            EnableHitbox(); // call from animtation later
        }
    }

    public void EnableHitbox()
    {
        attackHitbox.enabled = true;
    }

    void Update()
    {
        if (attackHitbox.enabled && playerLogic.time - playerLogic.lastAttackTime > playerLogic.attackDuration)
        {
            attackHitbox.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (playerLogic.rageMode)
        {
            if (other != this)
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    playerLogic.AddKills(damageable.OnDamage(playerLogic.damage));
                }
            }

        }
        else
        {
            if (other.CompareTag("Enemy"))
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    playerLogic.AddKills(damageable.OnDamage(playerLogic.damage));
                    if (playerLogic.killStreak >= playerLogic.streakNeededForRage)
                    {
                        playerLogic.rageMode = true;
                    }
                }
            }
        }
    }
}
