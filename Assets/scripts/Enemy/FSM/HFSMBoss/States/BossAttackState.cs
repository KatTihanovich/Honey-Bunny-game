using Game.Audio;
using UnityEngine;
using System.Collections;

public class BossAttackState : IState
{
    public IState ParentState { get; set; }
    public bool IsComposite => false;

    private float attackDuration = 1.29f;  
    private float hitTime = 0.28f;         
    private float timer;
    private const float attackRange = 5.5f;

    private MonoBehaviour coroutineRunner;
    private WaitForSeconds hitDelay;

    public BossAttackState()
    {
        hitDelay = new WaitForSeconds(hitTime);
    }

    public void Enter(GameObject actor, Blackboard blackboard)
    {
        Debug.Log("[BossAttack] Начало атаки");

        timer = 0f;
        blackboard.Set(BlackboardKeys.AttackFinished, false);

        blackboard.Set(BlackboardKeys.IsVisible, true);

        var animator = blackboard.GetOrDefault<Animator>(BlackboardKeys.BossAnimator);
        animator?.SetTrigger("Attack");

        coroutineRunner = actor.GetComponent<MonoBehaviour>();
        if (coroutineRunner != null)
        {
            Debug.Log("[BossAttack] Запуск задержки удара");
            coroutineRunner.StartCoroutine(HitDelayCoroutine(actor, blackboard));
        }
        else
        {
            Debug.LogWarning("[BossAttack] Нет MonoBehaviour для запуска корутины, удар не произойдёт!");
        }
    }

    private IEnumerator HitDelayCoroutine(GameObject actor, Blackboard blackboard)
    {
        yield return hitDelay; 
        DoHit(actor, blackboard);
    }

    public void Tick(GameObject actor, Blackboard blackboard)
    {
        timer += Time.deltaTime;

        if (timer >= attackDuration)
        {
            blackboard.Set(BlackboardKeys.AttackFinished, true);
        }
    }

    private void DoHit(GameObject actor, Blackboard blackboard)
    {
        var player = blackboard.GetOrDefault<Transform>(BlackboardKeys.PlayerTransform);
        var playerHealth = player?.GetComponent<HealthNew>();
        if (playerHealth == null) return;

        Debug.Log("[BossAttack] DoHit");

        var isVisible = blackboard.GetOrDefault<bool>(BlackboardKeys.IsVisible);
        var isRoaring = blackboard.GetOrDefault<bool>(BlackboardKeys.IsRoaring);
        Debug.Log($"[BossAttack] Roar status: {isRoaring}, Visible status: {isVisible}");

        if (!isVisible || isRoaring)
        {
            Debug.Log("[BossAttack] Hit cancelled, boss not visible or roaring");
            return;
        }

        float distance = Vector3.Distance(actor.transform.position, player.position);
        Debug.Log($"[BossAttack] Distance to player: {distance}");

        if (distance <= attackRange)
        {
            float damage = blackboard.GetOrDefault<float>(BlackboardKeys.AttackDamage);
            playerHealth.TakeDamage(damage);

            blackboard.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager)
                ?.PlaySound("TolikAttack");

            int hits = blackboard.GetOrDefault<int>(BlackboardKeys.HitsDoneThisPhase);
            blackboard.Set(BlackboardKeys.HitsDoneThisPhase, hits + 1);

            Debug.Log("[BossAttack] Урон нанесён");
        }
        else
        {
            Debug.Log("[BossAttack] Игрок слишком далеко, удар не нанесён");
        }
    }

    public void Exit(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.AttackFinished, true);
    }
}
