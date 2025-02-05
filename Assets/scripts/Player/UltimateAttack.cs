using UnityEngine;

namespace Player
{
    public class UltimateAttack : StateMachineBehaviour
    {
        private static readonly int Attack = Animator.StringToHash("RandomAttack");
    
        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            animator.SetInteger(Attack, Random.Range(2, 4));
        }
    }
}