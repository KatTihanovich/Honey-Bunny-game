using UnityEngine;
using System.Collections;
using Game.Audio;

public class BurderAttackState : IState
{
    private Coroutine _attackRoutine;
    private MonoBehaviour _coroutineRunner;

    public void Enter(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.AttackFinished, false);
        
        _coroutineRunner = actor.GetComponent<EnemyStateMachineRunner>();
        if (_coroutineRunner != null)
        {
            _attackRoutine = _coroutineRunner.StartCoroutine(AttackRoutine(actor, blackboard));
        }
    }

    public void Tick(GameObject actor, Blackboard blackboard)
    {
        float attackTimer = blackboard.GetOrDefault<float>(BlackboardKeys.AttackTimer);
        attackTimer += Time.deltaTime;
        blackboard.Set(BlackboardKeys.AttackTimer, attackTimer);
    }

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
        var soundManager = blackboard.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager);
        
        anim?.SetTrigger("Attack");
        
        yield return new WaitForSeconds(0.5f);
        
        if (PlayerInSight(actor, blackboard))
        {
            var playerHealth = blackboard.GetOrDefault<HealthNew>(BlackboardKeys.PlayerHealth);
            float damage = blackboard.GetOrDefault<float>(BlackboardKeys.AttackDamage);
            
            soundManager?.PlaySound("BurderAttack");
            playerHealth?.TakeDamage(damage);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        blackboard.Set(BlackboardKeys.AttackTimer, 0f);
        blackboard.Set(BlackboardKeys.AttackFinished, true);
    }

    private bool PlayerInSight(GameObject actor, Blackboard blackboard)
    {
        var capsuleCollider = blackboard.GetOrDefault<CapsuleCollider2D>(BlackboardKeys.BoxCollider);
        float rangeX = blackboard.GetOrDefault<float>(BlackboardKeys.AttackRangeX);
        float rangeY = blackboard.GetOrDefault<float>(BlackboardKeys.AttackRangeY);
        float colliderDistanceX = blackboard.GetOrDefault<float>(BlackboardKeys.ColliderDistanceX);
        float colliderDistanceY = blackboard.GetOrDefault<float>(BlackboardKeys.ColliderDistanceY);
        LayerMask playerLayer = blackboard.GetOrDefault<LayerMask>(BlackboardKeys.PlayerLayer);

        Vector2 origin = capsuleCollider.bounds.center + 
                        actor.transform.up * actor.transform.localScale.y * colliderDistanceY +
                        actor.transform.right * actor.transform.localScale.x * colliderDistanceX;
        Vector2 size = new Vector2(capsuleCollider.bounds.size.x * rangeX, capsuleCollider.bounds.size.y * rangeY);
        
        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0, Vector2.zero, 0, playerLayer);
        
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            var playerHealth = hit.transform.GetComponent<HealthNew>();
            blackboard.Set(BlackboardKeys.PlayerHealth, playerHealth);
            return playerHealth != null && playerHealth.enabled;
        }
        
        return false;
    }
}
