using UnityEngine;

public class NewEnemy : EnemyObject {
    public ComboObject comboScript;

    protected override void AwakeLoad() {
        comboScript = GetComponent<ComboObject>();
        if (comboScript != null)
            comboScript.attacked += EnemyStunned;
    }
    protected override void EnemyAttack() {
        Debug.Log("Im attacking");
        currentState = EnemyState.Stunned; //Transition to whatever state here
    }

    protected override void FixedMove() { //movement can be done however here
        Vector3 moveDir = trackTarget.position - transform.position;
        moveDir.Normalize();

        rb.AddForce(moveDir * moveSpeed);
        base.FixedMove();
    }

    


}
