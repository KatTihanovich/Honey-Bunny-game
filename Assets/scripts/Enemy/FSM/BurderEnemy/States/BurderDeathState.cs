using UnityEngine;
using Game.Audio;

public class BurderDeathState : IState
{
    public void Enter(GameObject actor, Blackboard blackboard)
    {
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        var soundManager = blackboard.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager);
        
        anim?.SetTrigger("Death");
        soundManager?.PlaySound("MobDeath");
        
        if (actor.TryGetComponent<Collider2D>(out var col))
            col.enabled = false;
        
        if (actor.TryGetComponent<Rigidbody2D>(out var rb))
            rb.linearVelocity = Vector2.zero;
        
        Object.Destroy(actor.transform.parent != null ? actor.transform.parent.gameObject : actor, 3f);
        EndWindow.IncreaseEnemyCount();
        
    }

    public void Tick(GameObject actor, Blackboard blackboard) { }

    public void Exit(GameObject actor, Blackboard blackboard) { }
}
