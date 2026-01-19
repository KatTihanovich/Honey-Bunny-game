using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeState : ICompositeState
{
    public StateMachine SubStateMachine { get; private set; }
    public IState InitialState { get; protected set; }
    public IState ParentState { get; set; }
    public bool IsComposite => true;

    protected List<IState> _subStates = new();
    protected GameObject _actor;
    protected Blackboard _blackboard;

    private bool _initialized;

    protected CompositeState()
    {
        SubStateMachine = new StateMachine();
    }

    public void AddSubState(IState state)
    {
        _subStates.Add(state);
        state.ParentState = this;
    }

    public virtual void Enter(GameObject actor, Blackboard blackboard)
    {
        _actor = actor;
        _blackboard = blackboard;

        if (!_initialized)
        {
            Debug.Log($"[CompositeState] Init {GetType().Name}");

            SubStateMachine.Initialize(actor, blackboard);

            if (InitialState != null)
            {
                SubStateMachine.ChangeState(InitialState);
            }
            else
            {
                Debug.LogError($"[CompositeState] {GetType().Name} не имеет InitialState!");
            }

            _initialized = true;
        }

        OnEnter(actor, blackboard);
    }

    protected virtual void OnEnter(GameObject actor, Blackboard blackboard) { }

    public virtual void Exit(GameObject actor, Blackboard blackboard)
    {
        Debug.Log($"[CompositeState] Exit {GetType().Name}");

        SubStateMachine.CurrentState?.Exit(actor, blackboard);
    }

    public virtual void Tick(GameObject actor, Blackboard blackboard)
    {
        if (SubStateMachine.CurrentState == null)
            return;

        SubStateMachine.Tick();
    }
}
