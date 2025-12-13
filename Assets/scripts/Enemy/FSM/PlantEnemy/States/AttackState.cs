using UnityEngine;
using System.Collections;
using Game.Audio;

public class AttackState : IState
{
    private Coroutine _attackRoutine;
    private MonoBehaviour _coroutineRunner;

    public void Enter(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.AttackFinished, false);
        _coroutineRunner = actor.GetComponent<MonoBehaviour>();
        if (_coroutineRunner != null)
        {
            _attackRoutine = _coroutineRunner.StartCoroutine(AttackRoutine(actor, blackboard));
        }
    }

    public void Tick(GameObject actor, Blackboard blackboard) { }

    public void Exit(GameObject actor, Blackboard blackboard)
    {
        if (_attackRoutine != null && _coroutineRunner != null)
        {
            _coroutineRunner.StopCoroutine(_attackRoutine);
            _attackRoutine = null;
        }
    }

    private IEnumerator AttackRoutine(GameObject actor, Blackboard blackboard)
    {
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        var sound = blackboard.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager);
        var playerHp = blackboard.GetOrDefault<HealthNew>(BlackboardKeys.PlayerHealth);
        float delay = blackboard.GetOrDefault<float>(BlackboardKeys.AttackDelay);
        float cooldown = blackboard.GetOrDefault<float>(BlackboardKeys.AttackCooldown);
        float damage = blackboard.GetOrDefault<float>(BlackboardKeys.AttackDamage);

        anim?.SetTrigger("Attack");
        yield return new WaitForSeconds(delay);

        sound?.PlaySound("WhipAttack");
        if (playerHp != null && !playerHp.IsDead)
        {
            playerHp.TakeDamage(damage);
        }

        yield return new WaitForSeconds(cooldown);
        
        blackboard.Set(BlackboardKeys.AttackFinished, true);
    }
}
