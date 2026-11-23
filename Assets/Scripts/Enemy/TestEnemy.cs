using UnityEngine;

public class TestEnemy : MonoBehaviour, IDamageable
{
    public int hp = 100;
    //Test for enemy and combat
    public int OnDamage(int damage)
    {
        Debug.Log("Enemy took damage: " + damage);
        hp -= damage;
        if (hp <= 0)
        {
            Destroy(gameObject); //can change to a death animation or something else later
            return 1;
        }
        return 0;
    }
}
