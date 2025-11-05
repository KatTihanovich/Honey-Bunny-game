// EnemyState.cs - Базовый абстрактный класс для всех состояний
public abstract class EnemyState
{
    public abstract void EnterState(NeckerStateMachine enemy);
    public abstract void UpdateState(NeckerStateMachine enemy);
    public abstract void ExitState(NeckerStateMachine enemy);
}
