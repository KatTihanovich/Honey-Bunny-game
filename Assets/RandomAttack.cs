using UnityEngine;

public class RandomAttack : StateMachineBehaviour
{
    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        animator.SetInteger("RandomAttack", Random.Range(0, 2));
    }
}
