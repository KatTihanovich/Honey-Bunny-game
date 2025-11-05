// IdleState.cs - Ожидание на краю зоны
using UnityEngine;

public class IdleState : EnemyState
{
    private float idleTimer;
    
    public override void EnterState(NeckerStateMachine enemy)
    {
        enemy.animator.SetTrigger("Idle");
        idleTimer = 0f;
    }
    
    public override void UpdateState(NeckerStateMachine enemy)
    {
        idleTimer += Time.deltaTime;
        
        // Проверяем игрока
        float distanceToPlayer = enemy.GetDistanceToPlayer();
        if (distanceToPlayer <= enemy.detectionRange)
        {
            enemy.SwitchState(enemy.walkState);
            return;
        }
        
        // После секунды возвращаемся к патрулированию
        if (idleTimer >= enemy.idleTime)
        {
            enemy.movingRight = !enemy.movingRight; // Меняем направление
            enemy.SwitchState(enemy.walkState);
        }
    }
    
    public override void ExitState(NeckerStateMachine enemy)
    {
        // Ничего не делаем
    }
}
