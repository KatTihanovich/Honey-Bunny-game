using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public IState CurrentState { get; private set; }
    
    private GameObject _actor;
    private Blackboard _blackboard;
    private Dictionary<IState, List<Transition>> _transitions = new();
    private List<Transition> _anyTransitions = new();
    
    public void Initialize(GameObject actor, Blackboard blackboard)
    {
        _actor = actor;
        _blackboard = blackboard;
    }
    
    public void AddTransition(IState fromState, Transition transition)
    {
        if (!_transitions.ContainsKey(fromState))
            _transitions[fromState] = new List<Transition>();
        _transitions[fromState].Add(transition);
    }
    
    public void AddAnyTransition(Transition transition)
    {
        _anyTransitions.Add(transition);
    }
    
    public void ChangeState(IState newState)
    {
        if (CurrentState == newState) return;
        CurrentState?.Exit(_actor, _blackboard);
        CurrentState = newState;
        CurrentState?.Enter(_actor, _blackboard);
    }
    
    public void Tick()
    {
        foreach (var transition in _anyTransitions)
        {
            if (transition.ShouldTransition(_actor, _blackboard))
            {
                ChangeState(transition.ToState);
                return;
            }
        }
        
        if (_transitions.ContainsKey(CurrentState))
        {
            foreach (var transition in _transitions[CurrentState])
            {
                if (transition.ShouldTransition(_actor, _blackboard))
                {
                    ChangeState(transition.ToState);
                    return;
                }
            }
        }
        
        CurrentState?.Tick(_actor, _blackboard);
    }
}
