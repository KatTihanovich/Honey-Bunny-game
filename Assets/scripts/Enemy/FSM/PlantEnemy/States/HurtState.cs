using UnityEngine;
using System.Collections;
using Game.Audio;

public class HurtState : IState
{
    private readonly EnemyStateMachineRunner _runner;
    private readonly Blackboard _bb;
    private readonly IState _nextState;
    private Coroutine _hurtRoutine;

    public HurtState(EnemyStateMachineRunner runner, Blackboard bb, IState nextState)
    {
        _runner = runner;
        _bb = bb;
        _nextState = nextState;
    }

    public void Enter()
    {
        _hurtRoutine = _runner.StartCoroutine(HurtRoutine());
    }

    public void Tick() { }

    public void Exit()
    {
        if (_hurtRoutine != null)
        {
            _runner.StopCoroutine(_hurtRoutine);
            _hurtRoutine = null;
        }
    }

    private IEnumerator HurtRoutine()
    {
        var anim = _bb.GetOrDefault<Animator>(BlackboardKeys.Animator);
        var sound = _bb.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager);

        anim?.SetTrigger("GotHit");
        sound?.PlaySound("Damage");

        yield return new WaitForSeconds(0.3f);

        var plant = _runner.GetComponent<PlantAI>();
        _runner.ChangeState(plant.ChooseIdleState());
    }
}
