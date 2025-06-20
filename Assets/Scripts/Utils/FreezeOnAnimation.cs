using UnityEngine;

public class AttackBehavior : StateMachineBehaviour
{
    private Rigidbody2D rb;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController player = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
        if (player != null)
        {
            player.IsMeditation = true;
        }
        PlayerAnimation player2 = GameObject.FindWithTag("Player")?.GetComponent<PlayerAnimation>();
        if (player != null)
        {
            player2.IsMeditation = true;
        }

        rb = animator.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController player = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
        if (player != null)
        {
            player.IsMeditation = false;
        }
        PlayerAnimation player2 = GameObject.FindWithTag("Player")?.GetComponent<PlayerAnimation>();
        if (player2 != null)
        {
            player2.IsMeditation = false;
        }

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}