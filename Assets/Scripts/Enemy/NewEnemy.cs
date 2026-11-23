using UnityEngine;

public class NewEnemy : EnemyObject {
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
