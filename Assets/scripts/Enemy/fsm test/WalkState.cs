// WalkState.cs - Патрулирование и преследование
using UnityEngine;

public class WalkState : EnemyState
{
    private bool isChasing = false;
    
    public override void EnterState(NeckerStateMachine enemy)
    {
        enemy.animator.SetTrigger("Walk");
    }
    
    public override void UpdateState(NeckerStateMachine enemy)
    {
        float distanceToPlayer = enemy.GetDistanceToPlayer();
        
        // Проверяем, видим ли игрока
        if (distanceToPlayer <= enemy.detectionRange)
        {
            isChasing = true;
            ChasePlayer(enemy);
            
            // Если достаточно близко - атакуем
            if (distanceToPlayer <= enemy.attackRange)
            {
                enemy.SwitchState(enemy.attackState);
                return;
            }
        }
        else
        {
            isChasing = false;
            Patrol(enemy);
        }
    }
    
    private void Patrol(NeckerStateMachine enemy)
    {
        // Движение в текущем направлении
        if (enemy.movingRight)
        {
            enemy.transform.position += Vector3.right * enemy.moveSpeed * Time.deltaTime;
            enemy.FlipTowards(enemy.transform.position + Vector3.right);
            
            // Достигли правой границы
            if (enemy.transform.position.x >= enemy.rightBound.x)
            {
                enemy.SwitchState(enemy.idleState);
            }
        }
        else
        {
            enemy.transform.position += Vector3.left * enemy.moveSpeed * Time.deltaTime;
            enemy.FlipTowards(enemy.transform.position + Vector3.left);
            
            // Достигли левой границы
            if (enemy.transform.position.x <= enemy.leftBound.x)
            {
                enemy.SwitchState(enemy.idleState);
            }
        }
    }
    
    private void ChasePlayer(NeckerStateMachine enemy)
    {
        Vector3 direction = (enemy.player.position - enemy.transform.position).normalized;
        enemy.transform.position += direction * enemy.moveSpeed * Time.deltaTime;
        enemy.FlipTowards(enemy.player.position);
    }
    
    public override void ExitState(NeckerStateMachine enemy)
    {
        isChasing = false;
    }
}
