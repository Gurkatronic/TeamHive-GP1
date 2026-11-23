using NaughtyAttributes;
using UnityEngine;

public class Combat : MonoBehaviour
{
    PlayerLogic playerLogic;
    [SerializeField] BoxCollider attackHitbox;
    [SerializeField] AttackSphere attackSphere;

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
            //EnableHitbox(); // call from animtation later
            HandleAttack(attackSphere.Attack());
        }
    }

    void HandleAttack(Collider[] colliders) {
        Debug.Log("collider len: " + colliders.Length);
        if(colliders.Length <= 0) {
            return;
        }

        for(int i = 0; i < colliders.Length; i++) {
            if (colliders[i].TryGetComponent(out IDamageable idmg)) {
                idmg.OnDamage(playerLogic.damage);
                Debug.Log("Attacking with rewrite, idamageable");
            }
            if (colliders[i].TryGetComponent(out IPunchable ipunch)) {
                ipunch.Impact(transform.position);
                Debug.Log("Attacking with rewrite, ipunchable");
            }
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
