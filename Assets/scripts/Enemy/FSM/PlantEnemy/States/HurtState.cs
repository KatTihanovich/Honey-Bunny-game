using UnityEngine;
using System.Collections;
using Game.Audio;

public class HurtState : IState
{
    private Coroutine _hurtRoutine;
    private MonoBehaviour _coroutineRunner;

    public void Enter(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.HurtAnimationFinished, false);
        _coroutineRunner = actor.GetComponent<MonoBehaviour>();
        if (_coroutineRunner != null)
        {
            _hurtRoutine = _coroutineRunner.StartCoroutine(HurtRoutine(blackboard));
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

    private IEnumerator HurtRoutine(Blackboard blackboard)
    {
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        var sound = blackboard.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager);

        anim?.SetTrigger("GotHit");
        sound?.PlaySound("Damage");
        
        yield return new WaitForSeconds(0.3f);
        
        blackboard.Set(BlackboardKeys.HurtAnimationFinished, true);
    }
}
