using System;
using UnityEngine;

public class Transition
{
    public IState ToState { get; private set; }
    public Func<GameObject, Blackboard, bool> Condition { get; private set; }
    
    public Transition(IState toState, Func<GameObject, Blackboard, bool> condition)
    {
        ToState = toState;
        Condition = condition;
    }
    
    public bool ShouldTransition(GameObject actor, Blackboard blackboard)
    {
        return Condition != null && Condition(actor, blackboard);
    }
}
