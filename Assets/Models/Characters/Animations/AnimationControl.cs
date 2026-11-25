using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    public Animator anim;

    public void SetRun()
    {
        anim.SetBool("Running", true);
    }
    public void SetAttack()
    {
        anim.SetTrigger("Attack");
    }
}
