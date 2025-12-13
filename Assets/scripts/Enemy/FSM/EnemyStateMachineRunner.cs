using UnityEngine;

public class EnemyStateMachineRunner : MonoBehaviour
{
    public Blackboard Blackboard { get; private set; }
    private StateMachine _stateMachine;

    private void Awake()
    {
        Blackboard = new Blackboard();
        _stateMachine = new StateMachine();
        _stateMachine.Initialize(gameObject, Blackboard);
    }

    private void Update()
    {
        _stateMachine.Tick();
    }

    public void ChangeState(IState state)
    {
        _stateMachine.ChangeState(state);
    }

    public void SetInitialState(IState state)
    {
        _stateMachine.ChangeState(state);
    }

    public IState GetCurrentState() => _stateMachine.CurrentState;
    
    public void AddTransition(IState fromState, Transition transition)
    {
        _stateMachine.AddTransition(fromState, transition);
    }
    
    public void AddAnyTransition(Transition transition)
    {
        _stateMachine.AddAnyTransition(transition);
    }
}
