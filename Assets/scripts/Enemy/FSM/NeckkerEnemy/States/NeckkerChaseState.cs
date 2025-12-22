using UnityEngine;

public class NeckkerChaseState : IState
{
    public void Enter(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.IsChasing, true);
        
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        anim?.SetBool("Run", true);
    }

    public void Tick(GameObject actor, Blackboard blackboard)
    {
        var playerTransform = blackboard.GetOrDefault<Transform>(BlackboardKeys.PlayerTransform);
        if (playerTransform == null) return;
        
        float speed = blackboard.GetOrDefault<float>(BlackboardKeys.MoveSpeed);
        
        // ВСЕГДА поворачиваемся и движемся к игроку (без остановки)
        RotateTowardsPlayer(actor, blackboard);
        MoveTowards(actor, blackboard, playerTransform.position);
        
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        anim?.SetBool("Run", true);
        
        // Обновление таймера атаки
        float attackTimer = blackboard.GetOrDefault<float>(BlackboardKeys.AttackTimer);
        attackTimer += Time.deltaTime;
        blackboard.Set(BlackboardKeys.AttackTimer, attackTimer);
    }

    public void Exit(GameObject actor, Blackboard blackboard)
    {
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        anim?.SetBool("Run", false);
    }

    private void MoveTowards(GameObject actor, Blackboard blackboard, Vector3 target)
    {
        float speed = blackboard.GetOrDefault<float>(BlackboardKeys.MoveSpeed);
        
        Vector3 direction = target - actor.transform.position;
        direction.y = 0f;
        direction = direction.normalized;
        
        actor.transform.position += new Vector3(direction.x, 0f, 0f) * speed * Time.deltaTime;
    }

    private void RotateTowardsPlayer(GameObject actor, Blackboard blackboard)
    {
        var playerTransform = blackboard.GetOrDefault<Transform>(BlackboardKeys.PlayerTransform);
        if (playerTransform == null) return;
        
        Vector3 baseScale = blackboard.GetOrDefault<Vector3>(BlackboardKeys.BaseScale);
        Vector3 direction = playerTransform.position - actor.transform.position;
        
        Vector3 localScale = baseScale;
        localScale.x = direction.x > 0 ? Mathf.Abs(baseScale.x) : -Mathf.Abs(baseScale.x);
        actor.transform.localScale = localScale;
    }
}
