using UnityEngine;
using System.Collections;
using Game.Audio;

public class DeathState : IState
{
    private readonly EnemyStateMachineRunner _runner;
    private readonly Blackboard _bb;
    private readonly float _deathSoundDelay;
    private Coroutine _deathRoutine;

    public DeathState(EnemyStateMachineRunner runner, Blackboard bb, float deathSoundDelay)
    {
        _runner = runner;
        _bb = bb;
        _deathSoundDelay = deathSoundDelay;
    }

    public void Enter()
    {
        var anim = _bb.GetOrDefault<Animator>(BlackboardKeys.Animator);
        var sound = _bb.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager);
        var go = _runner.gameObject;

        anim?.SetBool("Dead", true);

        if (go.TryGetComponent<Collider2D>(out var bodyCol))
            bodyCol.enabled = false;
        if (go.TryGetComponent<Rigidbody2D>(out var rb))
            rb.linearVelocity = Vector2.zero;

        _deathRoutine = _runner.StartCoroutine(DeathRoutine(sound, go));
    }

    public void Tick() { }

    public void Exit() { }

    private IEnumerator DeathRoutine(ISoundManager sound, GameObject go)
    {
        yield return new WaitForSeconds(_deathSoundDelay);
        sound?.PlaySound("MobDeath");

        Object.Destroy(go, 4f);
        EndWindow.IncreaseEnemyCount();
    }
}
