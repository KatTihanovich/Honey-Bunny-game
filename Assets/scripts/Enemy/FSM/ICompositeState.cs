public interface ICompositeState : IState
{
    StateMachine SubStateMachine { get; }
    IState InitialState { get; }
    void AddSubState(IState state);
}
