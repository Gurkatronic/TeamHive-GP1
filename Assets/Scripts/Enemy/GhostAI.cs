using UnityEngine;

public class GhostAI : MonoBehaviour
{
    GhostLogic ghostLogic;
    GameObject relic;
    [SerializeField] BoxCollider ghostAttackHitbox;
    Vector3 direction;
    public float ghostSpeed;
    void Start()
    {
        relic = GameObject.FindGameObjectWithTag("Relic");
        ghostLogic = GetComponent<GhostLogic>();
    }


    void Update()
    {
        GhostMovingToRelic();
        if (ghostLogic.time - ghostLogic.lastAttackTime >= ghostLogic.attackCooldown)
        {
            ghostLogic.lastAttackTime = ghostLogic.time;
            GhostSpawnHitbox();
        }
    }

    private void GhostMovingToRelic()
    {
        direction = relic.transform.position - transform.position;
        direction.Normalize();
        Vector3 enemyPos = transform.position + direction * Time.deltaTime * ghostSpeed;
        transform.position = enemyPos;
    }



    private void EnableEnemyHitBox()
    {
        ghostAttackHitbox.enabled = true;
    }

    public void GhostSpawnHitbox()
    {
        float distanceBetweenEnemyAndPlayer = Vector3.Distance(relic.transform.position, transform.position);
        if (distanceBetweenEnemyAndPlayer <= ghostLogic.attackRange)
        {
            EnableEnemyHitBox();
        }
        else
        {
            ghostAttackHitbox.enabled = false;
        }
    }


}
