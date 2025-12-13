//Updated
using UnityEngine;

public interface IState
{
    void Enter(GameObject actor, Blackboard blackboard);
    void Exit(GameObject actor, Blackboard blackboard);
    void Tick(GameObject actor, Blackboard blackboard);
}
