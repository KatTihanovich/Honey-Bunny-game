// AttackState.cs - Атака игрока
using UnityEngine;

public class AttackState : EnemyState
{
    private float attackCooldown = 1.5f;
    private float attackTimer;
    
    public override void EnterState(NeckerStateMachine enemy)
    {
        enemy.animator.SetTrigger("Attack");
        attackTimer = 0f;
        
        // Разворачиваемся к игроку
        if (enemy.player != null)
            enemy.FlipTowards(enemy.player.position);
    }
    
    public override void UpdateState(NeckerStateMachine enemy)
    {
        attackTimer += Time.deltaTime;
        
        // После анимации атаки проверяем дистанцию
        if (attackTimer >= attackCooldown)
        {
            float distanceToPlayer = enemy.GetDistanceToPlayer();
            
            if (distanceToPlayer > enemy.attackRange && distanceToPlayer <= enemy.detectionRange)
            {
                // Игрок отошёл - преследуем
                enemy.SwitchState(enemy.walkState);
            }
            else if (distanceToPlayer > enemy.detectionRange)
            {
                // Игрок ушёл - возвращаемся к патрулю
                enemy.SwitchState(enemy.walkState);
            }
            else
            {
                // Продолжаем атаковать
                enemy.animator.SetTrigger("Attack");
                attackTimer = 0f;
            }
        }
    }
    
    public override void ExitState(NeckerStateMachine enemy)
    {
        // Ничего не делаем
    }
}
