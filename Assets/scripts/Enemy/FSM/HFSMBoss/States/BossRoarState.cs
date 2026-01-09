using System.Collections;
using UnityEngine;
using Game.Audio;

public class BossRoarState : IState
{
    private Coroutine spawnRoutine;
    public IState ParentState { get; set; }
    public bool IsComposite => false;
    
    public  void Enter(GameObject actor, Blackboard blackboard)
    {
        Debug.Log("[BossRoar] Босс призывает миньонов!");
        
        blackboard.Set(BlackboardKeys.IsRoaring, true);
        blackboard.Set(BlackboardKeys.CanTakeDamage, false);
        
        var animator = blackboard.GetOrDefault<Animator>(BlackboardKeys.BossAnimator);
        var health = actor.GetComponent<HealthNew>();
        
        if (health != null) health.enabled = false;
        
        animator?.ResetTrigger("Attack");
        animator?.Play("Idle", 0);
        animator?.SetTrigger("Roar");
        
        var runner = actor.GetComponent<MonoBehaviour>();
        if (runner != null)
        {
            spawnRoutine = runner.StartCoroutine(SpawnEnemiesDuringRoar(actor, blackboard, runner));
        }
    }
    
    public  void Tick(GameObject actor, Blackboard blackboard)
    {
    }
    
    public void Exit(GameObject actor, Blackboard blackboard)
    {
        Debug.Log("[BossRoar] Вызов миньонов завершён");
        
        var health = actor.GetComponent<HealthNew>();
        if (health != null) health.enabled = true;
        
        blackboard.Set(BlackboardKeys.IsRoaring, false);
        blackboard.Set(BlackboardKeys.CanTakeDamage, true);
        blackboard.Set(BlackboardKeys.HasRoared, true);
    }
    
    private IEnumerator SpawnEnemiesDuringRoar(GameObject actor, Blackboard blackboard, MonoBehaviour runner)
    {
        var enemyPrefabs = blackboard.GetOrDefault<GameObject[]>(BlackboardKeys.EnemyPrefabs);
        var spawnPoints = blackboard.GetOrDefault<Transform[]>(BlackboardKeys.SpawnPoints);
        var roarDuration = blackboard.GetOrDefault<float>(BlackboardKeys.RoarDuration, 30f);
        var spawnInterval = blackboard.GetOrDefault<float>(BlackboardKeys.SpawnInterval, 8f);
        var maxEnemies = blackboard.GetOrDefault<int>(BlackboardKeys.MaxEnemies, 6);
        
        if (enemyPrefabs == null || enemyPrefabs.Length == 0 || 
            spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[BossRoar] Не настроены префабы или точки спавна!");
            blackboard.Set(BlackboardKeys.IsRoaring, false);
            yield break;
        }
        
        float elapsed = 0f;
        int spawnedCount = 0;
        
        while (elapsed < roarDuration && spawnedCount < maxEnemies)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            Object.Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log($"[BossRoar] Спавн врага {enemyPrefab.name} в точке {spawnPoint.name}");
            
            spawnedCount++;
            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }
        
        var animator = blackboard.GetOrDefault<Animator>(BlackboardKeys.BossAnimator);
        animator?.SetTrigger("Hide");
        
        yield return new WaitForSeconds(1f);
        
        blackboard.Set(BlackboardKeys.IsRoaring, false);
        blackboard.Set(BlackboardKeys.HasRoared, true);
        Debug.Log("[BossRoar] Спавн миньонов завершён");
    }
    
    public static void PlayRoar(GameObject actor, Blackboard blackboard)
    {
        var soundManager = blackboard.GetOrDefault<ISoundManager>(BlackboardKeys.SoundManager);
        soundManager?.PlaySound("TolikRoar");
    }
}
