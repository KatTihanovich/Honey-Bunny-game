//Updated
using UnityEngine;
using System.Collections;
using Game.Audio;

public class DeathState : IState
{
    private float _deathSoundDelay;
    private Coroutine _deathRoutine;
    private MonoBehaviour _coroutineRunner;

    public DeathState(float deathSoundDelay = 2f)
    {
        _deathSoundDelay = deathSoundDelay;
    }

    public void Enter(GameObject actor, Blackboard blackboard)
    {
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        var sound = blackboard.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager);

        anim?.SetBool("Dead", true);
        
        if (actor.TryGetComponent(out Collider2D bodyCol))
            bodyCol.enabled = false;
        
        if (actor.TryGetComponent(out Rigidbody2D rb))
            rb.linearVelocity = Vector2.zero;

        _coroutineRunner = actor.GetComponent<MonoBehaviour>();
        if (_coroutineRunner != null)
        {
            _deathRoutine = _coroutineRunner.StartCoroutine(DeathRoutine(sound, actor));
        }
    }

    public void Tick(GameObject actor, Blackboard blackboard) { }

    public void Exit(GameObject actor, Blackboard blackboard) { }

    private IEnumerator DeathRoutine(ISoundManager sound, GameObject actor)
    {
        yield return new WaitForSeconds(_deathSoundDelay);
        sound?.PlaySound("MobDeath");
        Object.Destroy(actor, 4f);
        EndWindow.IncreaseEnemyCount();
    }
}
