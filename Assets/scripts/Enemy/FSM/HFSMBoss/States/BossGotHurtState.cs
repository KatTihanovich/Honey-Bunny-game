using UnityEngine;
using Game.Audio;

public class BossGotHurtState : IState
{
    public IState ParentState { get; set; }
    public bool IsComposite => false;

    private Animator animator;
    private float timer;

    private const float hurtDuration = 0.6f; // длительность анимации боли

    public void Enter(GameObject actor, Blackboard blackboard)
    {
        Debug.Log("[BossGotHurt] Enter");

        animator = blackboard.GetOrDefault<Animator>(BlackboardKeys.BossAnimator);

        timer = 0f;

        // Сброс флагов
        blackboard.Set(BlackboardKeys.HurtAnimationFinished, false);
        blackboard.Set(BlackboardKeys.JustHit, false);

        // Анимация + звук
        animator?.ResetTrigger("GotHit");
        animator?.SetTrigger("GotHit");

        var soundManager = blackboard.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager);
        soundManager?.PlaySound("Damage");
    }

    public void Tick(GameObject actor, Blackboard blackboard)
    {
        // Tick только для логирования, больше ничего
        if (!blackboard.GetOrDefault<bool>(BlackboardKeys.HurtAnimationFinished))
        {
            blackboard.Set(BlackboardKeys.HurtAnimationFinished, true);
            Debug.Log("[BossGotHurt] Hurt finished immediately");
        }
    }


    public void Exit(GameObject actor, Blackboard blackboard)
    {
        Debug.Log("[BossGotHurt] Exit");
        blackboard.Set(BlackboardKeys.HurtAnimationFinished, false);
    }
}
