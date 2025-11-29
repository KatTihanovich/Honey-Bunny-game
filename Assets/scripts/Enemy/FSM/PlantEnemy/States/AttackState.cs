using UnityEngine;
using System.Collections;
using Game.Audio;

public class AttackState : IState
{
    private readonly EnemyStateMachineRunner _runner;
    private readonly Blackboard _bb;
    private Coroutine _attackRoutine;

    public AttackState(EnemyStateMachineRunner runner, Blackboard bb)
    {
        _runner = runner;
        _bb = bb;
    }

    public void Enter()
    {
        _attackRoutine = _runner.StartCoroutine(AttackRoutine());
    }

    public void Tick() { }

    public void Exit()
    {
        if (_attackRoutine != null)
        {
            _runner.StopCoroutine(_attackRoutine);
            _attackRoutine = null;
        }
    }

    private IEnumerator AttackRoutine()
    {
        var anim = _bb.GetOrDefault<Animator>(BlackboardKeys.Animator);
        var sound = _bb.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager);
        var playerHp = _bb.GetOrDefault<HealthNew>(BlackboardKeys.PlayerHealth);

        float delay = _bb.GetOrDefault<float>(BlackboardKeys.AttackDelay);
        float cooldown = _bb.GetOrDefault<float>(BlackboardKeys.AttackCooldown);
        float damage = _bb.GetOrDefault<float>(BlackboardKeys.AttackDamage);

        anim?.SetTrigger("Attack");
        yield return new WaitForSeconds(delay);

        sound?.PlaySound("WhipAttack");

        if (playerHp != null && !playerHp.IsDead)
        {
            playerHp.TakeDamage(damage);
        }

        yield return new WaitForSeconds(cooldown);

        _runner.ChangeState(ChooseNextIdle());
    }

        private IState ChooseNextIdle()
    {
        var plant = _runner.GetComponent<PlantAI>();
        return plant.ChooseIdleState();
    }
}
