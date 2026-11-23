using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    EnemyLogic enemyLogic;
    GameObject closestPlayer;
    private NavMeshAgent agent;
    List<GameObject> PlayersInGameList = new List<GameObject>();
    [SerializeField] BoxCollider enemyAttackHitbox;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyLogic = GetComponent<EnemyLogic>();

        foreach (GameObject playersFound in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayersInGameList.Add(playersFound);
        }

    }

    private void Update()
    {
        if(enemyLogic.time - enemyLogic.lastAttackTime >= enemyLogic.attackCooldown)
        {
            enemyLogic.lastAttackTime = enemyLogic.time;
            EnemyOnAttack();
        }
    }
    void FixedUpdate()
    {
        if (PlayersInGameList != null)
        {
            EnemyMoveTowardsClosestPlayer(PlayersInGameList);
            agent.SetDestination(closestPlayer.transform.position);
        }
    }

    private void EnemyMoveTowardsClosestPlayer(List<GameObject> playersList)
    {
        if (playersList != null)
        {
            float minDistance = Mathf.Infinity;
            foreach (GameObject playersFound in playersList)
            {
                float distanceBetweenEnemyAndPlayer = Vector3.Distance(transform.position, playersFound.transform.position);
                if (distanceBetweenEnemyAndPlayer < minDistance)
                {
                    minDistance = distanceBetweenEnemyAndPlayer;
                    closestPlayer = playersFound;
                }
            }      
        }
    }

    private void EnableEnemyHitBox()
    {
        enemyAttackHitbox.enabled = true;
        Debug.Log("HitboxActivated");
    }

    public void EnemyOnAttack()
    {
        float distanceBetweenEnemyAndPlayer = Vector3.Distance(closestPlayer.transform.position, transform.position);
        if (distanceBetweenEnemyAndPlayer <= enemyLogic.attackRange)
        {
            EnableEnemyHitBox(); 
        }
        else
        {
            enemyAttackHitbox.enabled = false;
        }
    }
}







