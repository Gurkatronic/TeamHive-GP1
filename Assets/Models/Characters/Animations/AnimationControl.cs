using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    public Animator anim;

    public void SetRun(bool isMoving)
    {
        anim.SetBool("Running", isMoving);
    }
    public void SetAttack()
    {
        anim.SetTrigger("Attack");
    }
}
