using UnityEngine;
using System.Collections;
using Game.Audio;

public class BurderFleeState : IState
{
    private Coroutine _fleeRoutine;
    private MonoBehaviour _coroutineRunner;
    private float _fleeTimer = 0f;

    public void Enter(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.FleeFinished, false);
        _fleeTimer = 0f;
        
        _coroutineRunner = actor.GetComponent<EnemyStateMachineRunner>();
        if (_coroutineRunner != null)
        {
            _fleeRoutine = _coroutineRunner.StartCoroutine(FleeRoutine(actor, blackboard));
        }
    }

    public void Tick(GameObject actor, Blackboard blackboard)
    {
        Vector3 fleeDirection = blackboard.GetOrDefault<Vector3>(BlackboardKeys.FleeDirection);
        float fleeSpeed = blackboard.GetOrDefault<float>(BlackboardKeys.FleeSpeed);
        float fleeDistance = blackboard.GetOrDefault<float>(BlackboardKeys.FleeDistance);
        
        _fleeTimer += Time.deltaTime;
        
        actor.transform.position += fleeDirection * fleeSpeed * Time.deltaTime;
        
        if (_fleeTimer >= fleeDistance / fleeSpeed)
        {
            blackboard.Set(BlackboardKeys.FleeFinished, true);
        }
    }

    public void Exit(GameObject actor, Blackboard blackboard)
    {
        if (_fleeRoutine != null && _coroutineRunner != null)
        {
            _coroutineRunner.StopCoroutine(_fleeRoutine);
            _fleeRoutine = null;
        }
    }

    private IEnumerator FleeRoutine(GameObject actor, Blackboard blackboard)
    {
        var soundManager = blackboard.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager);
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        float fleeDelay = blackboard.GetOrDefault<float>(BlackboardKeys.FleeDelay);
        
        soundManager?.PlaySound("BurderRun");
        anim?.SetTrigger("Switch_to_run");
        
        yield return new WaitForSeconds(fleeDelay);
        
        var playerTransform = blackboard.GetOrDefault<Transform>(BlackboardKeys.PlayerTransform);
        if (playerTransform != null)
        {
            Vector3 fleeDirection = (actor.transform.position - playerTransform.position).normalized;
            blackboard.Set(BlackboardKeys.FleeDirection, fleeDirection);
            
            RotateTowardsDirection(actor, blackboard, fleeDirection);
        }
        
        anim?.SetTrigger("Run");
    }

    private void RotateTowardsDirection(GameObject actor, Blackboard blackboard, Vector3 direction)
    {
        Vector3 baseScale = blackboard.GetOrDefault<Vector3>(BlackboardKeys.BaseScale);
        Vector3 localScale = baseScale;
        localScale.x = direction.x > 0 ? Mathf.Abs(baseScale.x) : -Mathf.Abs(baseScale.x);
        actor.transform.localScale = localScale;
    }
}
