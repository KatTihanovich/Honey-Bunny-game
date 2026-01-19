using UnityEngine;
using Game.Audio;
using Enemy;

public class BossDeathState : IState
{
    public IState ParentState { get; set; }
    public bool IsComposite => false;
    public void Enter(GameObject actor, Blackboard blackboard)
    {
        Debug.Log("[BossDeathState] Босс побеждён!");
        
        var animator = blackboard.GetOrDefault<Animator>(BlackboardKeys.BossAnimator);
        var soundManager = blackboard.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager);
        var tails = blackboard.GetOrDefault<TailBossEnemyScript[]>(BlackboardKeys.BossTails);
        
        if (tails != null)
        {
            foreach (var tail in tails)
            {
                tail?.SetDie();
            }
        }
        
        animator?.SetTrigger("Dead");
        soundManager?.PlaySound("BossDie");
        
        // Сохранение прогресса
        PlayerPrefs.SetInt("BossDefeated", 1);
        PlayerPrefs.Save();
        
        // Завершение уровня
        var tracker = Object.FindObjectOfType<LevelProgressTracker>();
        if (tracker != null)
        {
            tracker.OnLevelComplete();
        }
    }
    
    public void Tick(GameObject actor, Blackboard blackboard) { }
    public void Exit(GameObject actor, Blackboard blackboard) { }
}
