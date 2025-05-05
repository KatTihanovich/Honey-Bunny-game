using UnityEngine;

public class AttackBehavior : StateMachineBehaviour
{
    private Rigidbody2D rb;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var patrol = animator.GetComponent<NeckkerPatrol>();

        rb = animator.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var patrol = animator.GetComponent<NeckkerPatrol>();

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
        }
    }
}
