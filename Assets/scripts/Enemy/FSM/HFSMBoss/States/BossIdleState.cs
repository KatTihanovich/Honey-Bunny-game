using UnityEngine;

public class BossIdleState : IState
{
    public IState ParentState { get; set; }
    public bool IsComposite => false;

    public void Enter(GameObject actor, Blackboard blackboard)
    {
    }

    public void Tick(GameObject actor, Blackboard blackboard)
    {
        var player = blackboard.GetOrDefault<Transform>(BlackboardKeys.PlayerTransform);
        if (player == null) return;

        Vector3 scale = actor.transform.localScale;
        if (player.position.x > actor.transform.position.x)
            scale.x = Mathf.Abs(scale.x);
        else
            scale.x = -Mathf.Abs(scale.x);

        actor.transform.localScale = scale;
    }

    public void Exit(GameObject actor, Blackboard blackboard)
    {
    }
}
