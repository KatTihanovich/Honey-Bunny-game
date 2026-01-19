using UnityEngine;
using System.Collections;
using Game.Audio;

public class NeckkerAttackState : IState
{
    private Coroutine _attackRoutine;
    private MonoBehaviour _coroutineRunner;
    private Vector3 _lockedScale;
    public IState ParentState { get; set; }
    public bool IsComposite => false;

    public void Enter(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.AttackFinished, false);
        _lockedScale = actor.transform.localScale;
        
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        anim?.SetBool("Run", false);
        
        _coroutineRunner = actor.GetComponent<EnemyStateMachineRunner>();
        if (_coroutineRunner != null)
        {
            _attackRoutine = _coroutineRunner.StartCoroutine(AttackRoutine(actor, blackboard));
        }
    }

    public void Tick(GameObject actor, Blackboard blackboard)
    {
        actor.transform.localScale = _lockedScale;
    }

    public void Exit(GameObject actor, Blackboard blackboard)
    {
        actor.transform.localScale = _lockedScale;
        
        if (_attackRoutine != null && _coroutineRunner != null)
        {
            _coroutineRunner.StopCoroutine(_attackRoutine);
            _attackRoutine = null;
        }
    }

    private IEnumerator AttackRoutine(GameObject actor, Blackboard blackboard)
    {
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        
        anim?.SetTrigger("Attack");
        
        // Автоматическое определение длины анимации
        float attackDuration = GetAttackAnimationLength(anim);
        
        yield return new WaitForSeconds(attackDuration + 0.2f); // +0.2 для плавности
        
        blackboard.Set(BlackboardKeys.AttackTimer, 0f);
        blackboard.Set(BlackboardKeys.AttackFinished, true);
    }

    private float GetAttackAnimationLength(Animator animator)
    {
        if (animator == null) return 1.0f;
        
        // Ищем клип "Attack" в RuntimeAnimatorController
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        if (ac == null) return 1.0f;
        
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name.Contains("NECKKER_ATTACK") || clip.name.Contains("attack"))
            {
                return clip.length;
            }
        }
        
        // Если не нашли - возвращаем значение по умолчанию
        return 1.0f;
    }
}
