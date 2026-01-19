public class BossRootState : CompositeState
{
    public BossRootState()
    {
        var aliveState = new BossAliveState();
        var deathState = new BossDeathState();

        AddSubState(aliveState);
        AddSubState(deathState);

        InitialState = aliveState;

        SubStateMachine.AddAnyTransition(
            new Transition(deathState, (actor, bb) =>
                bb.GetOrDefault<float>(BlackboardKeys.SelfHealth) <= 0f));
    }
}
