// DamageState.cs - Получение урона
using UnityEngine;

public class DamageState : EnemyState
{
    private float damageAnimationTime = 0.5f; // Длительность анимации урона
    private float timer;
    
    public override void EnterState(NeckerStateMachine enemy)
    {
        enemy.animator.SetTrigger("GotHit");
        timer = 0f;
    }
    
    public override void UpdateState(NeckerStateMachine enemy)
    {
        timer += Time.deltaTime;
        
        // После анимации урона возвращаемся к основному поведению
        if (timer >= damageAnimationTime)
        {
            float distanceToPlayer = enemy.GetDistanceToPlayer();
            
            if (distanceToPlayer <= enemy.attackRange)
            {
                enemy.SwitchState(enemy.attackState);
            }
            else if (distanceToPlayer <= enemy.detectionRange)
            {
                enemy.SwitchState(enemy.walkState);
            }
            else
            {
                enemy.SwitchState(enemy.walkState); // Патруль
            }
        }
    }
    
    public override void ExitState(NeckerStateMachine enemy)
    {
        // Ничего не делаем
    }
}
