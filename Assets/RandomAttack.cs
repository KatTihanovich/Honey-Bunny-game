using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class RandomAttack : StateMachineBehaviour
{
    private static readonly int Attack = Animator.StringToHash("RandomAttack");

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        Debug.Log("RandomAttack");
        animator.SetInteger(Attack, Random.Range(0, 2));
    }
}