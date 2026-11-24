using UnityEngine;

[RequireComponent(typeof(AttackSphere), typeof(ComboObject))]
public class NewEnemy : EnemyObject {
    public ComboObject comboScript;
    public AttackSphere attackSphere;

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
                if (hits[i].TryGetComponent(out IDamageable idmg)) {
                    Debug.Log("Enemy attack here");
                    idmg.OnDamage(attackDamage);
                }
                if (hits[i].TryGetComponent(out IPunchable ipunch)) {
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
