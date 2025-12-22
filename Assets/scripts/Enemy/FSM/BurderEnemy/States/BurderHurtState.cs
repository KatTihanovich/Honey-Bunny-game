using UnityEngine;
using System.Collections;
using Game.Audio;

public class BurderHurtState : IState
{
    private Coroutine _hurtRoutine;
    private MonoBehaviour _coroutineRunner;

    public void Enter(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.HurtFinished, false);
        
        _coroutineRunner = actor.GetComponent<EnemyStateMachineRunner>();
        if (_coroutineRunner != null)
        {
            _hurtRoutine = _coroutineRunner.StartCoroutine(HurtRoutine(actor, blackboard));
        }
    }

    public void Tick(GameObject actor, Blackboard blackboard) { }

    public void Exit(GameObject actor, Blackboard blackboard)
    {
        if (_hurtRoutine != null && _coroutineRunner != null)
        {
            _coroutineRunner.StopCoroutine(_hurtRoutine);
            _hurtRoutine = null;
        }
    }

    private IEnumerator HurtRoutine(GameObject actor, Blackboard blackboard)
    {
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        var soundManager = blackboard.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager);
        
        anim?.SetTrigger("Damage");
        soundManager?.PlaySound("Damage");
        
        yield return new WaitForSeconds(0.3f);
        
        blackboard.Set(BlackboardKeys.HurtFinished, true);
    }
}
