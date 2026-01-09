using UnityEngine;
using Enemy;

public class BossPhase1State : CompositeState
{
    public BossPhase1State()
    {
        var chase = new BossChaseState();

        AddSubState(chase);
        InitialState = chase;
    }

    public override void Enter(GameObject actor, Blackboard blackboard)
    {
        base.Enter(actor, blackboard);

        var tails = blackboard.GetOrDefault<TailBossEnemyScript[]>(BlackboardKeys.BossTails);
        if (tails == null) return;

        foreach (var tail in tails)
        {
            tail.HideImmediate();
        }
    }
}
