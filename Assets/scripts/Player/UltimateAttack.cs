using UnityEngine;
using UnityEngine.Animations;

namespace Player
{
    public class UltimateAttack : StateMachineBehaviour
    {
        
        public GameObject bunny;
        
        private static readonly int Attack = Animator.StringToHash("RandomAttack");
        
        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            animator.SetInteger(Attack, Random.Range(0, 2));
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
        {
            PlayerMovement playerMovement = animator.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ApplyUltimateDamage();
            }
        }
    }
}