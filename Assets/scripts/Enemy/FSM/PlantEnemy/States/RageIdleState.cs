//Updated
using UnityEngine;

public class RageIdleState: IState
{
    public void Enter(GameObject actor, Blackboard blackboard)
    {
        UpdateAnimation(blackboard);
        blackboard.Set(BlackboardKeys.AttackFinished, false);
    }

    public void Tick(GameObject actor, Blackboard blackboard) 
    {
        UpdateAnimation(blackboard);
    }

    public void Exit(GameObject actor, Blackboard blackboard) { }

    private void UpdateAnimation(Blackboard blackboard)
    {
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        var playerHp = blackboard.GetOrDefault<HealthNew>(BlackboardKeys.PlayerHealth);

        if (playerHp == null || playerHp.IsDead) return;

        float hp = playerHp.CurrentHealth;
        
        if (hp > 50f) 
        {
            anim?.SetBool("IsStressed", true);
            anim?.SetBool("IsMad", false);
        }
        else 
        {
            anim?.SetBool("IsStressed", true);
            anim?.SetBool("IsMad", true);
        }
    }
}
