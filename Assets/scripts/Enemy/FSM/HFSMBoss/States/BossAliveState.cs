using UnityEngine;

public class BossAliveState : CompositeState
{
    public BossAliveState()
    {
        var phase1 = new BossPhase1State();
        var phase2 = new BossPhase2State();
        var phase3 = new BossPhase3State();

        AddSubState(phase1);
        AddSubState(phase2);
        AddSubState(phase3);

        InitialState = phase1;

        SubStateMachine.AddTransition(phase1,
            new Transition(phase2, (actor, bb) =>
                bb.GetOrDefault<float>(BlackboardKeys.SelfHealth) < 160f));

        SubStateMachine.AddTransition(phase2,
            new Transition(phase3, (actor, bb) =>
                bb.GetOrDefault<float>(BlackboardKeys.SelfHealth) < 80f));
    }

    protected override void OnEnter(GameObject actor, Blackboard blackboard)
    {
        Debug.Log("[BossAliveState] Boss is active!");
    }
}
