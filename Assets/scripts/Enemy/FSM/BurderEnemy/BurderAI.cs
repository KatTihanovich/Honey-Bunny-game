using UnityEngine;
using Game.Audio;

[RequireComponent(typeof(HealthNew))]
[RequireComponent(typeof(EnemyStateMachineRunner))]
public class BurderAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private bool alwaysChasePlayer = false;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float agroDistance = 5f;
    [SerializeField] private float lostDistance = 7f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float rangeX = 1.5f;
    [SerializeField] private float rangeY = 1.2f;
    [SerializeField] private float colliderDistanceX = 0.5f;
    [SerializeField] private float colliderDistanceY = 0f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private CapsuleCollider2D capsuleCollider;
    [SerializeField] private LayerMask playerLayer;

    [Header("Movement Settings")]
    [SerializeField] private float walkDistance = 4f;
    [SerializeField] private float runDistance = 2f;

    [Header("Flee Settings")]
    [SerializeField] private float fleeDistance = 15f;
    [SerializeField] private float fleeSpeed = 10f;
    [SerializeField] private float fleeDelay = 0.5f;

    [Header("Thorn Spawn")]
    [SerializeField] private bool canSpawnThorns = true;
    [SerializeField] private float thornCooldown = 3f;
    [SerializeField] private GameObject thornPrefab;

    [Header("References")]
    public Animator animator;

    private HealthNew selfHealth;
    private ISoundManager soundManager;
    private Transform playerTransform;
    private EnemyStateMachineRunner _runner;
    private Vector3 baseScale;

    private BurderPatrolState _patrolState;
    private BurderWalkState _walkState;
    private BurderRunState _runState;
    private BurderAttackState _attackState;
    private BurderHurtState _hurtState;
    private BurderFleeState _fleeState;
    private BurderDeathState _deathState;

    private void Awake()
    {
        selfHealth = GetComponent<HealthNew>();
        _runner = GetComponent<EnemyStateMachineRunner>();
        baseScale = transform.localScale;

        selfHealth.OnDamaged += HandleDamaged;
        selfHealth.OnDeath += HandleDeath;
    }

    private void Start()
    {
        soundManager = SoundManagerNew.Instance;
        
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }

        InitializeBlackboard();
        InitializeStates();
        SetupTransitions();

        _runner.SetInitialState(_patrolState);
    }

    private void InitializeBlackboard()
    {
        var bb = _runner.Blackboard;

        bb.Set(BlackboardKeys.Animator, animator);
        bb.Set(BlackboardKeys.SelfHealth, selfHealth);
        bb.Set(BlackboardKeys.PlayerTransform, playerTransform);
        bb.Set(BlackboardKeys.SoundManager, soundManager);
        bb.Set(BlackboardKeys.BaseScale, baseScale);

        bb.Set(BlackboardKeys.MoveSpeed, moveSpeed);
        bb.Set(BlackboardKeys.AgroDistance, agroDistance);
        bb.Set(BlackboardKeys.LostDistance, lostDistance);
        bb.Set(BlackboardKeys.WalkDistance, walkDistance);
        bb.Set(BlackboardKeys.RunDistance, runDistance);
        bb.Set(BlackboardKeys.AttackCooldown, attackCooldown);
        bb.Set(BlackboardKeys.AttackRange, attackRange);
        bb.Set(BlackboardKeys.AttackDamage, damage);
        bb.Set(BlackboardKeys.AttackRangeX, rangeX);
        bb.Set(BlackboardKeys.AttackRangeY, rangeY);
        bb.Set(BlackboardKeys.ColliderDistanceX, colliderDistanceX);
        bb.Set(BlackboardKeys.ColliderDistanceY, colliderDistanceY);
        bb.Set(BlackboardKeys.BoxCollider, capsuleCollider);
        bb.Set(BlackboardKeys.PlayerLayer, playerLayer);

        bb.Set(BlackboardKeys.PointA, pointA);
        bb.Set(BlackboardKeys.PointB, pointB);
        bb.Set(BlackboardKeys.CurrentPatrolPoint, pointA);

        bb.Set(BlackboardKeys.FleeDistance, fleeDistance);
        bb.Set(BlackboardKeys.FleeSpeed, fleeSpeed);
        bb.Set(BlackboardKeys.FleeDelay, fleeDelay);

        bb.Set(BlackboardKeys.CanSpawnThorns, canSpawnThorns);
        bb.Set(BlackboardKeys.ThornCooldown, thornCooldown);
        bb.Set(BlackboardKeys.ThornPrefab, thornPrefab);

        bb.Set(BlackboardKeys.IsDead, false);
        bb.Set(BlackboardKeys.IsChasing, false);
        bb.Set(BlackboardKeys.AttackTimer, 0f);
        bb.Set(BlackboardKeys.ThornTimer, 0f);
        bb.Set(BlackboardKeys.AttackFinished, false);
        bb.Set(BlackboardKeys.HurtFinished, false);
        bb.Set(BlackboardKeys.FleeFinished, false);
        bb.Set(BlackboardKeys.ThornSpawnFinished, true);
        bb.Set(BlackboardKeys.AlwaysChasePlayer, alwaysChasePlayer);
    }

    private void InitializeStates()
    {
        _patrolState = new BurderPatrolState();
        _walkState = new BurderWalkState();
        _runState = new BurderRunState();
        _attackState = new BurderAttackState();
        _hurtState = new BurderHurtState();
        _fleeState = new BurderFleeState();
        _deathState = new BurderDeathState();
    }

    private void SetupTransitions()
    {
        // Any state -> Death
        _runner.AddAnyTransition(new Transition(_deathState, (actor, bb) =>
            bb.GetOrDefault<bool>(BlackboardKeys.IsDead)
        ));

        // Hurt -> Flee
        _runner.AddTransition(_hurtState, new Transition(_fleeState, (actor, bb) =>
            bb.GetOrDefault<bool>(BlackboardKeys.HurtFinished)
        ));

        // Flee -> Patrol (when flee finished)
        _runner.AddTransition(_fleeState, new Transition(_patrolState, (actor, bb) =>
            bb.GetOrDefault<bool>(BlackboardKeys.FleeFinished)
        ));

        // Patrol -> Walk (player detected at agro distance)
        _runner.AddTransition(_patrolState, new Transition(_walkState, (actor, bb) =>
        {
            bool alwaysChase = bb.GetOrDefault<bool>(BlackboardKeys.AlwaysChasePlayer);
            if (alwaysChase) return false;
            
            return PlayerInSight(actor, bb) && GetDistanceToPlayer(actor, bb) <= bb.GetOrDefault<float>(BlackboardKeys.AgroDistance);
        }));

        // Walk -> Run (close distance)
        _runner.AddTransition(_walkState, new Transition(_runState, (actor, bb) =>
            GetDistanceToPlayer(actor, bb) <= bb.GetOrDefault<float>(BlackboardKeys.RunDistance)
        ));

        // Walk -> Patrol (lost player)
        _runner.AddTransition(_walkState, new Transition(_patrolState, (actor, bb) =>
            GetDistanceToPlayer(actor, bb) > bb.GetOrDefault<float>(BlackboardKeys.LostDistance)
        ));

        // Run -> Attack (in attack range and cooldown ready)
        _runner.AddTransition(_runState, new Transition(_attackState, (actor, bb) =>
        {
            float attackTimer = bb.GetOrDefault<float>(BlackboardKeys.AttackTimer);
            float cooldown = bb.GetOrDefault<float>(BlackboardKeys.AttackCooldown);
            float distance = GetDistanceToPlayer(actor, bb);
            float attackRange = bb.GetOrDefault<float>(BlackboardKeys.AttackRange);
            
            return attackTimer >= cooldown && distance <= attackRange;
        }));

        // Run -> Walk (player moved away)
        _runner.AddTransition(_runState, new Transition(_walkState, (actor, bb) =>
        {
            float distance = GetDistanceToPlayer(actor, bb);
            float walkDist = bb.GetOrDefault<float>(BlackboardKeys.WalkDistance);
            float runDist = bb.GetOrDefault<float>(BlackboardKeys.RunDistance);
            
            return distance > runDist && distance <= walkDist;
        }));

        // Attack -> Run (attack finished and player still close)
        _runner.AddTransition(_attackState, new Transition(_runState, (actor, bb) =>
        {
            bool attackFinished = bb.GetOrDefault<bool>(BlackboardKeys.AttackFinished);
            float distance = GetDistanceToPlayer(actor, bb);
            float runDist = bb.GetOrDefault<float>(BlackboardKeys.RunDistance);
            
            return attackFinished && distance <= runDist;
        }));

        // Attack -> Walk (attack finished and player at walk distance)
        _runner.AddTransition(_attackState, new Transition(_walkState, (actor, bb) =>
        {
            bool attackFinished = bb.GetOrDefault<bool>(BlackboardKeys.AttackFinished);
            float distance = GetDistanceToPlayer(actor, bb);
            float attackRange = bb.GetOrDefault<float>(BlackboardKeys.AttackRange);
            float walkDist = bb.GetOrDefault<float>(BlackboardKeys.WalkDistance);
            
            return attackFinished && distance > attackRange && distance <= walkDist;
        }));

        // Attack -> Patrol (attack finished and player far)
        _runner.AddTransition(_attackState, new Transition(_patrolState, (actor, bb) =>
        {
            bool attackFinished = bb.GetOrDefault<bool>(BlackboardKeys.AttackFinished);
            float distance = GetDistanceToPlayer(actor, bb);
            float lostDist = bb.GetOrDefault<float>(BlackboardKeys.LostDistance);
            
            return attackFinished && distance > lostDist;
        }));
    }

    private float GetDistanceToPlayer(GameObject actor, Blackboard bb)
    {
        var playerTransform = bb.GetOrDefault<Transform>(BlackboardKeys.PlayerTransform);
        if (playerTransform == null) return Mathf.Infinity;
        
        return Vector2.Distance(actor.transform.position, playerTransform.position);
    }

private bool PlayerInSight(GameObject actor, Blackboard bb)
{
    var playerTransform = bb.GetOrDefault<Transform>(BlackboardKeys.PlayerTransform);
    if (playerTransform == null) return false;
    
    var playerHealth = playerTransform.GetComponent<HealthNew>();
    if (playerHealth == null || playerHealth.IsDead) return false;
    
    // Просто проверяем, что игрок существует и жив
    bb.Set(BlackboardKeys.PlayerHealth, playerHealth);
    return true;
}


    private void HandleDamaged(float damage)
    {
        if (_runner.Blackboard.GetOrDefault<bool>(BlackboardKeys.IsDead)) return;
        _runner.ChangeState(_hurtState);
    }

    private void HandleDeath()
    {
        var bb = _runner.Blackboard;
        if (bb.GetOrDefault<bool>(BlackboardKeys.IsDead)) return;
        bb.Set(BlackboardKeys.IsDead, true);
    }

    public void SpawnThorn()
    {
        var bb = _runner.Blackboard;
        GameObject thornPrefab = bb.GetOrDefault<GameObject>(BlackboardKeys.ThornPrefab);
        
        if (thornPrefab != null)
        {
            Instantiate(thornPrefab, transform.position, Quaternion.identity);
        }

        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null && rend.isVisible)
        {
            soundManager?.PlaySound("BurderSpawn");
        }

        bb.Set(BlackboardKeys.ThornSpawnFinished, true);
    }

    public void BurderAttack()
    {

    }

    private void OnDrawGizmosSelected()
    {
        if (capsuleCollider == null) return;
        
        Gizmos.color = Color.magenta;
        Vector2 origin = capsuleCollider.bounds.center + 
                        transform.up * transform.localScale.y * colliderDistanceY +
                        transform.right * transform.localScale.x * colliderDistanceX;
        Vector2 size = new Vector2(capsuleCollider.bounds.size.x * rangeX, capsuleCollider.bounds.size.y * rangeY);
        Gizmos.DrawWireCube(origin, size);
    }
}
