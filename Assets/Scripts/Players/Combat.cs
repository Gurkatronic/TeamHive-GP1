using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Combat : MonoBehaviour
{
    PlayerLogic playerLogic;
    [SerializeField] BoxCollider attackHitbox;
    [SerializeField] AudioSource audioSource;
    public ParticleSystem rageParticleSystem;
    private Animator animator;

    private void Awake()
    {
        playerLogic = GetComponent<PlayerLogic>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (playerLogic.stunned || playerLogic.knockedDown) return;

        if (playerLogic.time - playerLogic.lastAttackTime >= playerLogic.attackCooldown)
        {
            if (context.started)
            {
                animator.SetTrigger("Attack");
                playerLogic.lastAttackTime = playerLogic.time;
                EnableHitbox(); // call from animtation later
                SoundManager.Instance.PlayRandomSound(RandomAudioType.Whiff, audioSource);
            }
        }
    }

    public void EnableHitbox()
    {
        attackHitbox.enabled = true;
        //spela ljud om ingen collider är damageble, annars annat ljud, använd bool i OnTriggerEnter för att kolla om det är damageble eller inte
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
        if(other.TryGetComponent(out IPunchable ipunch))
        {
            ipunch.Impact(transform.position);
        }
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
                    SoundManager.Instance.PlayRandomSound(RandomAudioType.Punch, audioSource);
                    Debug.Log("Player attacked here <-------------------------");
                    if (playerLogic.killStreak >= playerLogic.streakNeededForRage)
                    {
                        audioSource.PlayOneShot(playerLogic.GetSoundEffect(PlayerSoundType.Ragemode));
                        playerLogic.rageMode = true;
                        rageParticleSystem.Play();
                    }
                }
            }
            
        }
    }
}
