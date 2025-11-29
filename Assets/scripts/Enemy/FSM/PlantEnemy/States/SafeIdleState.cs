using UnityEngine;

public class SafeIdleState : IState
{
    private readonly EnemyStateMachineRunner _runner;
    private readonly Blackboard _bb;

    public SafeIdleState(EnemyStateMachineRunner runner, Blackboard bb)
    {
        _runner = runner;
        _bb = bb;
    }

   public void Enter() { } 

    public void Tick()
    {
        if (_bb.GetOrDefault<bool>(BlackboardKeys.IsDead)) return;

        var plant = _runner.GetComponent<PlantAI>();
        var next = plant.ChooseIdleState();

        if (next.GetType() != typeof(SafeIdleState))
        {
            _runner.ChangeState(next);
            return;
        }
    }


    public void Exit() { }
}