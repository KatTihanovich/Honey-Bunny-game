// DeathState.cs - Смерть моба
using UnityEngine;

public class DeathState : EnemyState
{
    public override void EnterState(NeckerStateMachine enemy)
    {
        enemy.animator.SetTrigger("Dead");
        
        // Отключаем коллайдер и другие компоненты
        Collider2D collider = enemy.GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;
        
        // Уничтожаем объект через 2 секунды
        Object.Destroy(enemy.gameObject, 2f);
    }
    
    public override void UpdateState(NeckerStateMachine enemy)
    {
        // Ничего не делаем, моб мёртв
    }
    
    public override void ExitState(NeckerStateMachine enemy)
    {
        // Ничего не делаем
    }
}
