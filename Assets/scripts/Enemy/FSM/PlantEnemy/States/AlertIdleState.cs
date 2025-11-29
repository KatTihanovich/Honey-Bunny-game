using UnityEngine;

public class AlertIdleState : IState
{
    private readonly EnemyStateMachineRunner _runner;
    private readonly Blackboard _bb;

    public AlertIdleState(EnemyStateMachineRunner runner, Blackboard bb)
    {
        _runner = runner;
        _bb = bb;
    }

    public void Enter(){}

    public void Tick()
    {
        if (_bb.GetOrDefault<bool>(BlackboardKeys.IsDead)) return;

        var plant = _runner.GetComponent<PlantAI>();
        var next = plant.ChooseIdleState();

        if (next.GetType() != typeof(AlertIdleState))
        {
            _runner.ChangeState(next);
            return;
        }

        bool inRange = _bb.GetOrDefault<bool>(BlackboardKeys.IsPlayerInRange);
        if (inRange && _runner.GetCurrentState() is not AttackState)
        {
            _runner.ChangeState(new AttackState(_runner, _bb));
        }
    }


    public void Exit() { }
}