using UnityEngine;

public class EnemyStateMachineRunner : MonoBehaviour
{
    public Blackboard Blackboard { get; private set; }
    private StateMachine _stateMachine;

    private IState _pendingState;     
    private bool _hasPending;

    private void Awake()
    {
        Blackboard = new Blackboard();
        _stateMachine = new StateMachine();
    }

    private void Update()
    {
        _stateMachine.Tick();

        if (_hasPending)
        {
            _stateMachine.ChangeState(_pendingState);
            _pendingState = null;
            _hasPending = false;
        }
    }

    public void ChangeState(IState state)
    {
        _pendingState = state;
        _hasPending = true;
    }

    public void SetInitialState(IState state)
    {
        _stateMachine.ChangeState(state);
    }

    public IState GetCurrentState() => _stateMachine.CurrentState;
}
