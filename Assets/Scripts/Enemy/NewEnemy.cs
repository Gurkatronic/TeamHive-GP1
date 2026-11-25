using UnityEngine;

[RequireComponent(typeof(AttackSphere), typeof(ComboObject))]
public class NewEnemy : EnemyObject, IDamageable{
    public ComboObject comboScript;
    public AttackSphere attackSphere;

    public int OnDamage(int damage) {
        currentHealth -= damage;
        currentState = EnemyState.Stunned;
        Debug.Log("Enemy takes damage");
        if (currentHealth < 0) {
            Debug.Log("Enemy dead");
            EnemyDead();
            return 1;
        }
        else
            return 0;
    }

    protected override void AwakeLoad() {
        comboScript = GetComponent<ComboObject>();
        if (comboScript != null)
            comboScript.attacked += EnemyStunned;
        if (attackSphere == null)
            attackSphere = GetComponent<AttackSphere>();

        attackSphere.attackLayers = attackLayers;
    }
    protected override void EnemyAttack() {
        Collider[] hits = attackSphere.Attack();

        for(int i = 0; i < hits.Length; i++) {
            if (hits[i].gameObject != gameObject) {
                if (hits[i].TryGetComponent(out IDamageable idmg) && !hits[i].CompareTag("Enemy")) {
                    Debug.Log("Enemy attack here");
                    idmg.OnDamage(attackDamage);
                }
                if (hits[i].TryGetComponent(out IPunchable ipunch) && !hits[i].CompareTag("Enemy")) {
                    Debug.Log("Enemy punch objects here");
                    ipunch.Impact(transform.position);
                }
            }
        }

        currentState = EnemyState.Moving; //Transition to whatever state here
    }

    protected override void FixedMove() { //movement can be done however here
        Vector3 moveDir = trackTarget.position - transform.position;
        moveDir.Normalize();

        rb.AddForce(moveDir * moveSpeed);
        base.FixedMove();
    }

    


}
