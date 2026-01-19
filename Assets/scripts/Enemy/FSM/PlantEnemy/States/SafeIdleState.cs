//Updated
using UnityEngine;

public class SafeIdleState : IState
{
    public IState ParentState { get; set; }
    public bool IsComposite => false;
    public void Enter(GameObject actor, Blackboard blackboard)
    {
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        anim?.SetBool("IsStressed", false);
        anim?.SetBool("IsMad", false);
        
        blackboard.Set(BlackboardKeys.AttackFinished, false);
    }

    public void Tick(GameObject actor, Blackboard blackboard) { }

    public void Exit(GameObject actor, Blackboard blackboard) { }
}
