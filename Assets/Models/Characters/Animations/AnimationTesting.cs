using UnityEngine;

public class AnimationTesting : MonoBehaviour
{
    public Animator anim;

    public void Attack()
    {
        anim.SetTrigger("Attack");
    }
}
