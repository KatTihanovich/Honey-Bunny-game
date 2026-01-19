using UnityEngine;
using Enemy;

public class BossSurfaceState : CompositeState
{
    private BossAttackState attack;
    private BossGotHurtState hurt;
    private BossIdleState idle;

    public BossSurfaceState()
    {
        attack = new BossAttackState();
        hurt   = new BossGotHurtState();
        idle   = new BossIdleState();

        AddSubState(attack);
        AddSubState(hurt);
        AddSubState(idle);

        // Surface → Attack
        SubStateMachine.AddTransition(
            idle,
            new Transition(attack, (actor, bb) =>
            {
                var area = bb.GetOrDefault<BossAttackArea>(BlackboardKeys.AttackAreaScript);
                return area != null && area.PlayerInside;
            })
        );

        // Attack → Idle
        SubStateMachine.AddTransition(
            attack,
            new Transition(idle, (actor, bb) =>
                bb.GetOrDefault<bool>(BlackboardKeys.AttackFinished))
        );

        // Any → Hurt
        SubStateMachine.AddAnyTransition(
            new Transition(hurt, (actor, bb) =>
                bb.GetOrDefault<bool>(BlackboardKeys.JustHit) &&
                bb.GetOrDefault<bool>(BlackboardKeys.CanTakeDamage))
        );

        // Hurt → Idle
        SubStateMachine.AddTransition(
            hurt,
            new Transition(idle, (actor, bb) =>
                bb.GetOrDefault<bool>(BlackboardKeys.HurtAnimationFinished))
        );

        InitialState = idle;
    }

    public override void Enter(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.AttackFinished, false);
        blackboard.Set(BlackboardKeys.JustHit, false);
        blackboard.Set(BlackboardKeys.IsVisible, true);
        blackboard.Set(BlackboardKeys.CanTakeDamage, true);

        SubStateMachine.Initialize(actor, blackboard);
        SubStateMachine.ChangeState(InitialState);
    }

    public override void Tick(GameObject actor, Blackboard blackboard)
    {
        base.Tick(actor, blackboard);
    }

    public override void Exit(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.IsVisible, false);
        blackboard.Set(BlackboardKeys.CanTakeDamage, false);
    }
}
