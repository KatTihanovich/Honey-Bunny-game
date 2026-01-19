using UnityEngine;
using Enemy;
using Game.Audio;

public class BossAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private GameObject attackArea;
    [SerializeField] private TailBossEnemyScript[] tails;
    
    [Header("Settings")]
    [SerializeField] private float appearDuration = 0.9f;
    [SerializeField] private float disappearDuration = 0.9f;
    [SerializeField] private float activePhaseDuration = 5f;
    [SerializeField] private float hiddenDuration = 10f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackDamage = 1f;
    
    [Header("ROAR Settings")]
    [SerializeField] private GameObject[] listEnemy;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float roarDuration = 30f;
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    
    private Renderer bossRenderer;
    private Vector3 initialScale;
    private HealthNew health;
    private GameObject player;
    private HealthNew playerHealth;
    private ISoundManager soundManager;
    private BossAttackArea bossAttackArea;
    private EnemyStateMachineRunner stateMachineRunner;
    
    private void Awake()
    {
        // Получение компонентов
        animator = animator ? animator : GetComponent<Animator>();
        boxCollider = boxCollider ? boxCollider : GetComponent<BoxCollider2D>();
        health = GetComponent<HealthNew>();
        bossRenderer = GetComponentInChildren<Renderer>();
        initialScale = transform.localScale;
        bossAttackArea = GetComponentInChildren<BossAttackArea>();
        
        // Найти все хвосты если не назначены
        if (tails == null || tails.Length == 0)
        {
            tails = FindObjectsOfType<TailBossEnemyScript>();
        }
        
        // Получение или создание FSM Runner
        stateMachineRunner = GetComponent<EnemyStateMachineRunner>();
        if (stateMachineRunner == null)
        {
            stateMachineRunner = gameObject.AddComponent<EnemyStateMachineRunner>();
        }
    }
    
    private void Start()
    {
        // Получение игрока и систем
        soundManager = SoundManagerNew.Instance;
        player = FindFirstObjectByType<PlayerController>()?.gameObject;
        
        if (player == null)
        {
            Debug.LogError("[BossAI] Игрок не найден!");
            return;
        }
        
        playerHealth = player.GetComponent<HealthNew>();
        
        // Инициализация Blackboard
        InitializeBlackboard();
        
        // Создание и запуск HFSM
        var rootState = new BossRootState();
        stateMachineRunner.SetInitialState(rootState);
        
        // Подписка на события здоровья
        SubscribeToHealthEvents();
        
        Debug.Log("[BossAI] Инициализация завершена. HFSM запущен.");
    }
    
    private void InitializeBlackboard()
    {
        var blackboard = stateMachineRunner.Blackboard;
        
        // Компоненты босса
        blackboard.Set(BlackboardKeys.BossAnimator, animator);
        blackboard.Set(BlackboardKeys.BossBoxCollider, boxCollider);
        blackboard.Set(BlackboardKeys.BossRenderer, bossRenderer);
        blackboard.Set(BlackboardKeys.BossAttackArea, attackArea);
        blackboard.Set(BlackboardKeys.BossTails, tails);
        blackboard.Set(BlackboardKeys.BossInitialScale, initialScale);
        blackboard.Set(BlackboardKeys.BossSpawnPoint, spawnPoint);
        blackboard.Set(BlackboardKeys.AttackAreaScript, bossAttackArea);
        
        // Ссылки на игрока
        blackboard.Set(BlackboardKeys.PlayerTransform, player.transform);
        blackboard.Set(BlackboardKeys.PlayerHealth, playerHealth);
        
        // Системы
        blackboard.Set(BlackboardKeys.SoundManager, soundManager);
        blackboard.Set(BlackboardKeys.Animator, animator);
        
        // Здоровье босса
        blackboard.Set(BlackboardKeys.SelfHealth, health.CurrentHealth);
        
        // Параметры атаки
        blackboard.Set(BlackboardKeys.AttackDamage, attackDamage);
        blackboard.Set(BlackboardKeys.AttackCooldown, attackCooldown);
        
        // Длительности фаз
        blackboard.Set(BlackboardKeys.AppearDuration, appearDuration);
        blackboard.Set(BlackboardKeys.DisappearDuration, disappearDuration);
        blackboard.Set(BlackboardKeys.ActivePhaseDuration, activePhaseDuration);
        blackboard.Set(BlackboardKeys.HiddenDuration, hiddenDuration);
        blackboard.Set(BlackboardKeys.RoarDuration, roarDuration);
        
        // Спавн миньонов
        blackboard.Set(BlackboardKeys.EnemyPrefabs, listEnemy);
        blackboard.Set(BlackboardKeys.SpawnPoints, spawnPoints);
        blackboard.Set(BlackboardKeys.SpawnInterval, 8f);
        blackboard.Set(BlackboardKeys.MaxEnemies, 6);
        
        // Начальные флаги
        blackboard.Set(BlackboardKeys.IsFirstAppear, true);
        blackboard.Set(BlackboardKeys.IsVisible, false);
        blackboard.Set(BlackboardKeys.CanTakeDamage, false);
        blackboard.Set(BlackboardKeys.IsRoaring, false);
        blackboard.Set(BlackboardKeys.HasRoared, false);
        blackboard.Set(BlackboardKeys.HurtAnimationFinished, true);
        blackboard.Set(BlackboardKeys.AttackFinished, true);
        
        // Таймеры и счётчики
        blackboard.Set(BlackboardKeys.ActiveTimer, 0f);
        blackboard.Set(BlackboardKeys.AttackTimer, 0f);
        blackboard.Set(BlackboardKeys.DamageTakenThisPhase, 0);
        blackboard.Set(BlackboardKeys.HitsDoneThisPhase, 0);
    }
    
    private void SubscribeToHealthEvents()
    {
        var blackboard = stateMachineRunner.Blackboard;
        
        health.OnDamageTaken += () =>
        {
            blackboard.Set(BlackboardKeys.SelfHealth, health.CurrentHealth);

            bool canTakeDamage = blackboard.GetOrDefault<bool>(BlackboardKeys.CanTakeDamage);
            bool isVisible     = blackboard.GetOrDefault<bool>(BlackboardKeys.IsVisible);

            if (canTakeDamage && isVisible)
            {
                blackboard.Set(BlackboardKeys.JustHit, true);
            }
        };

        
        health.OnDeath += () => 
        {
            blackboard.Set(BlackboardKeys.SelfHealth, 0f);
            Debug.Log("[BossAI] Босс погиб!");
        };
    }
    
    // ========== Animation Events ==========
    // Эти методы вызываются через Animation Events в аниматоре
    

    public void PlayRoar()
    {
        BossRoarState.PlayRoar(gameObject, stateMachineRunner.Blackboard);
    }
    
    // ========== Debug Methods ==========
    
    private void OnDrawGizmosSelected()
    {
        // Визуализация зоны атаки
        if (attackArea != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackArea.transform.position, 2.5f);
        }
        
        // Визуализация точки спавна
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
        }
        
        // Визуализация точек спавна миньонов
        if (spawnPoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireSphere(point.position, 0.3f);
                }
            }
        }
    }
}
