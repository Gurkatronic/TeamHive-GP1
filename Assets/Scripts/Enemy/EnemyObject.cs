using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public abstract class EnemyObject : MonoBehaviour
{
    [Serializable]
    protected enum EnemyState {
        Attacking,
        AttackCharge,
        Stunned,
        Searching,
        Moving,
        Spawning
    }
    protected enum EnemySearchMethod {
        OnlyRelic,
        ClosePlayerOrRelic,
        OnlyPlayer
    }
    [Header("ENEMY IS IN STATE")]
    [SerializeField] protected EnemySearchMethod searchMethod = EnemySearchMethod.OnlyRelic;
    [SerializeField] protected  EnemyState currentState = EnemyState.Spawning;
    [SerializeField] protected float spawnTime = 0.5f;
    protected ActionTimer spawnTimer;

    [Header("Search Settings")]
    [SerializeField] protected float playerSearchRange = 5f;
    [SerializeField] protected LayerMask playerLayer;

    [Header("Attack Information")]
    [SerializeField] protected Transform trackTarget;
    [SerializeField] protected bool moveAttack = false;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float attackSpeed = 1f;
    [SerializeField] protected float attackCooldown = 0.5f;
    [SerializeField] protected int attackDamage = 1;
    [SerializeField] protected LayerMask attackLayers;
    protected ActionTimer attackTimer;
    [Space(10)]

    [Header("Movement")]
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected float moveSpeed = 5f;


    [Header("Disable Information")]
    [SerializeField] protected float stunTime = 1f;
    protected ActionTimer stunTimer;




    private void Awake() {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        attackTimer = new(attackSpeed, attackCooldown);
        stunTimer = new(stunTime);
        spawnTimer = new(spawnTime);
        AwakeLoad();
    }

    protected virtual void AwakeLoad() {

    }

    public virtual void SpawnEnemy(Vector3 spawnPos) {
        currentState = EnemyState.Spawning;
        transform.position = spawnPos;
    }

    private void Update() {
        switch(currentState) {
            case EnemyState.Searching:
                FindTarget();
                break;

            case EnemyState.AttackCharge:
                if (!IsTargetInRange()) {
                    attackTimer.ResetTimer();
                    if (trackTarget != null) {
                        currentState = EnemyState.Moving;
                    }
                }
                else if (attackTimer.UpdateTimer()) {
                    currentState = EnemyState.Attacking;
                    EnemyAttack();
                }
                break;

            case EnemyState.Stunned:
                if(stunTimer.UpdateTimer()) {
                    currentState = EnemyState.Searching;
                }
                break;

            case EnemyState.Spawning:
                if(spawnTimer.UpdateTimer()) {
                    currentState = EnemyState.Searching;
                }
                break;
        }
    }

    private void FixedUpdate() {
        switch (currentState) {
            case EnemyState.Moving:
                if(trackTarget == null) {
                    currentState = EnemyState.Searching;
                    return;
                }
                FixedMove();
                break;
        }
    }

    /// <summary>
    /// Look for what to track, transition to whatever state feels necessary, but moving is usually the right choice in that case base.FindTarget()
    /// </summary>
    protected virtual void FindTarget() {
        switch(searchMethod) {
            case EnemySearchMethod.OnlyRelic:
                trackTarget = Relic.Instance.transform;
                break;
            case EnemySearchMethod.ClosePlayerOrRelic: //search for player, if it fails track the relic
                if(!FindPlayer())
                    trackTarget = Relic.Instance.transform;
                break;
            case EnemySearchMethod.OnlyPlayer:
                break;
        }
        currentState = EnemyState.Moving;
    }

    bool FindPlayer() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, playerSearchRange, playerLayer);

        for(int i = 0; i < colliders.Length; i++) {
            if (colliders[i].CompareTag("Player")) { //kan skicka denna, kan göra en range check för att se vilken som är närmast.
                trackTarget = colliders[i].transform;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Enemy is in state moving. This runs on the fixedupdate cycle
    /// </summary>
    protected virtual void FixedMove() {
        bool targetInRange = IsTargetInRange();
        if(moveAttack && targetInRange) {
            if(attackTimer.UpdateTimer()) {
                EnemyAttack();
            }
        }
        else if(targetInRange) {
            currentState = EnemyState.AttackCharge;
        }
    }

    /// <summary>
    /// Checks if the tracktarget position is within the attack range.
    /// </summary>
    /// <returns>True if target is close, false if not</returns>
    protected bool IsTargetInRange() {
        if (trackTarget == null) {
            currentState = EnemyState.Searching;
            return false;
        }

        return Vector3.Distance(transform.position, trackTarget.position) < attackRange;
    }

    /// <summary>
    /// What the enemy does when the attack is ready
    /// </summary>
    protected abstract void EnemyAttack();

    /// <summary>
    /// Stunning, can be called multiple times, but will reset the stun timer and change the state to stunned.
    /// </summary>
    protected virtual void EnemyStunned() {
        stunTimer.ResetTimer();
        currentState = EnemyState.Stunned;
    }
}
