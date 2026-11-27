using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieEnemy : EnemyObject {
    NavMeshAgent meshAgent;
    float rotateSpeed = 2f;

    protected override void AwakeLoad() {
        meshAgent = GetComponent<NavMeshAgent>();
        //meshAgent.autoRepath = false;
    }
    protected override void EnemyAttack() {
        //Zombie attack trigger
        Debug.Log("Lol im attacking :PPppP");
        SoundManager.Instance.PlayRandomSound(RandomAudioType.ZombieHittingSomeone, audioSource);
        currentState = EnemyState.Searching;
    }

    protected override void FixedMove() {

        meshAgent.SetDestination(trackTarget.transform.position);
        transform.LookAt(trackTarget.transform.position);
        Vector3 movedesire = meshAgent.desiredVelocity.normalized;
        
        
        rb.AddForce(movedesire * moveSpeed, ForceMode.Acceleration);
        //meshAgent.CalculatePath() // This returns false if there is no path to take.
        //Maybe it can attack then to get objects out of the way

        base.FixedMove();
    }

    public void PlayFootStep(int left) {
        
    }


    [Button("Test damage numb")]
    public void TestDamage() {
        TakingDamage(Random.Range(1, 30));
    }

    public override void TakingDamage(int damage) {
        base.TakingDamage(damage);
        SoundManager.Instance.PlayRandomSound(RandomAudioType.ZombieHit, audioSource);
    }
}
