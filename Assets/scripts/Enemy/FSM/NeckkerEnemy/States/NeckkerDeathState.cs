using UnityEngine;
using System.Collections;
using Game.Audio;

public class NeckkerDeathState : IState
{
    private float _deathSoundDelay;
    public IState ParentState { get; set; }
    public bool IsComposite => false;
    
    public NeckkerDeathState(float deathSoundDelay = 1f)
    {
        _deathSoundDelay = deathSoundDelay;
    }

    public void Enter(GameObject actor, Blackboard blackboard)
    {
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        var soundManager = blackboard.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager);
        var rb = blackboard.GetOrDefault<Rigidbody2D>(BlackboardKeys.Rigidbody);
        
        anim?.SetBool("Dead", true);
        rb.linearVelocity = Vector2.zero;
        
        if (actor.TryGetComponent<Collider2D>(out var col))
            col.enabled = false;
        
        var runner = actor.GetComponent<EnemyStateMachineRunner>();
        if (runner != null)
        {
            runner.StartCoroutine(DeathRoutine(soundManager, actor));
        }
    }

    public void Tick(GameObject actor, Blackboard blackboard) { }

    public void Exit(GameObject actor, Blackboard blackboard) { }

    private IEnumerator DeathRoutine(ISoundManager soundManager, GameObject actor)
    {
        yield return new WaitForSeconds(_deathSoundDelay);
        soundManager?.PlaySound("MobDeath");
        Object.Destroy(actor, 2f);
        EndWindow.IncreaseEnemyCount();
    }
}
