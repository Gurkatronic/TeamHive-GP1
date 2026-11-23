using UnityEngine;

public class EnemyLogic : MonoBehaviour, IDamageable
{
    public int health = 100;
    public int damage = 10;
    public float attackCooldown = 0.2f;
    public float lastAttackTime = 0f;
    public float attackDuration = 0.05f;
    public float attackRange = 0.5f;
    public float time = 0f;


    private void Update()
    {
        time += Time.deltaTime;
    }

    public int OnDamage(int damage)
    {
        Debug.Log("enemy took damage: " + damage);
        health -= damage;
        if (health <= 0)
        {
            return 1;
        }
        return 0;
    }
}