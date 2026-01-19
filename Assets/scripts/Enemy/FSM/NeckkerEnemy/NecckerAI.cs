using UnityEngine;
using Game.Audio;

[RequireComponent(typeof(HealthNew))]
[RequireComponent(typeof(EnemyStateMachineRunner))]
[RequireComponent(typeof(Rigidbody2D))]
public class NeckkerAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTimeAtPoint = 1f;

    [Header("Vision Settings")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float visionHeightOffset = 0.5f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseStopDistance = 3f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackRangeX = 1.5f;
    [SerializeField] private float attackRangeY = 1.2f;
    [SerializeField] private float colliderDistanceX = 0.5f;
    [SerializeField] private float colliderDistanceY = 0f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Contact Damage")]
    [SerializeField] private float contactDamageDelay = 1.5f;

    [Header("Clone Settings")]
    [SerializeField] private bool canClone = true;
    [SerializeField] private float cloneOffset = 2f;
    [SerializeField] private GameObject mobPrefab;

    [Header("Audio Settings")]
    [SerializeField] public AudioClip attackSound;
    [SerializeField] private float volume = 0.05f;
    [SerializeField] public float deathSoundDelay = 1f;

    [Header("References")]
    public Animator animator;

    private HealthNew selfHealth;
    private ISoundManager soundManager;
    private Transform playerTransform;
    private EnemyStateMachineRunner _runner;
    private Rigidbody2D rb;
    private Vector3 baseScale;

    private bool isClone = false;
    private bool hasCloned = false;
    private float contactDamageCooldown = 0f;

    private NeckkerPatrolState _patrolState;
    private NeckkerChaseState _chaseState;
    private NeckkerAttackState _attackState;
    private NeckkerHurtState _hurtState;
    private NeckkerCloneState _cloneState;
    private NeckkerDeathState _deathState;

    private void Awake()
    {
        selfHealth = GetComponent<HealthNew>();
        _runner = GetComponent<EnemyStateMachineRunner>();
        rb = GetComponent<Rigidbody2D>();
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

    private void Update()
    {
        contactDamageCooldown += Time.deltaTime;
        _runner.Blackboard.Set(BlackboardKeys.ContactDamageCooldown, contactDamageCooldown);
    }

    private void InitializeBlackboard()
    {
        var bb = _runner.Blackboard;

        bb.Set(BlackboardKeys.Animator, animator);
        bb.Set(BlackboardKeys.SelfHealth, selfHealth);
        bb.Set(BlackboardKeys.PlayerTransform, playerTransform);
        bb.Set(BlackboardKeys.SoundManager, soundManager);
        bb.Set(BlackboardKeys.BaseScale, baseScale);
        bb.Set(BlackboardKeys.Rigidbody, rb);

        bb.Set(BlackboardKeys.LeftEdge, leftEdge);
        bb.Set(BlackboardKeys.RightEdge, rightEdge);
        bb.Set(BlackboardKeys.MoveSpeed, speed);
        bb.Set(BlackboardKeys.WaitTimeAtPoint, waitTimeAtPoint);
        bb.Set(BlackboardKeys.MovingLeft, true);
        bb.Set(BlackboardKeys.WaitTimer, 0f);

        bb.Set(BlackboardKeys.VisionRange, visionRange);
        bb.Set(BlackboardKeys.VisionHeightOffset, visionHeightOffset);
        bb.Set(BlackboardKeys.PlayerLayer, playerLayer);
        bb.Set(BlackboardKeys.ObstacleLayer, obstacleLayer);

        bb.Set(BlackboardKeys.ChaseStopDistance, chaseStopDistance);

        bb.Set(BlackboardKeys.AttackCooldown, attackCooldown);
        bb.Set(BlackboardKeys.AttackRangeX, attackRangeX);
        bb.Set(BlackboardKeys.AttackRangeY, attackRangeY);
        bb.Set(BlackboardKeys.ColliderDistanceX, colliderDistanceX);
        bb.Set(BlackboardKeys.ColliderDistanceY, colliderDistanceY);
        bb.Set(BlackboardKeys.AttackDamage, damage);
        bb.Set(BlackboardKeys.BoxCollider, boxCollider);
        bb.Set(BlackboardKeys.AttackTimer, 0f);
        bb.Set(BlackboardKeys.AttackFinished, false);

        bb.Set(BlackboardKeys.ContactDamageDelay, contactDamageDelay);
        bb.Set(BlackboardKeys.ContactDamageCooldown, 0f);

        bb.Set(BlackboardKeys.CanClone, canClone && !isClone);
        bb.Set(BlackboardKeys.IsClone, isClone);
        bb.Set(BlackboardKeys.HasCloned, hasCloned);
        bb.Set(BlackboardKeys.CloneOffset, cloneOffset);
        bb.Set(BlackboardKeys.MobPrefab, mobPrefab);
        bb.Set(BlackboardKeys.CloneFinished, false);

        bb.Set(BlackboardKeys.IsDead, false);
        bb.Set(BlackboardKeys.IsChasing, false);
        bb.Set(BlackboardKeys.HurtFinished, false);
    }

    private void InitializeStates()
    {
        _patrolState = new NeckkerPatrolState();
        _chaseState = new NeckkerChaseState();
        _attackState = new NeckkerAttackState();
        _hurtState = new NeckkerHurtState();
        _cloneState = new NeckkerCloneState();
        _deathState = new NeckkerDeathState(deathSoundDelay);
    }

    private void SetupTransitions()
    {
        // Any state -> Death (highest priority)
        _runner.AddAnyTransition(new Transition(_deathState, (actor, bb) =>
            bb.GetOrDefault<bool>(BlackboardKeys.IsDead)
        ));

        // Any state -> Clone (when HP < 50% and can clone)
        _runner.AddAnyTransition(new Transition(_cloneState, (actor, bb) =>
        {
            if (bb.GetOrDefault<bool>(BlackboardKeys.IsDead)) return false;
            if (!bb.GetOrDefault<bool>(BlackboardKeys.CanClone)) return false;
            if (bb.GetOrDefault<bool>(BlackboardKeys.HasCloned)) return false;
            
            var health = bb.GetOrDefault<HealthNew>(BlackboardKeys.SelfHealth);
            return health != null && health.CurrentHealth < health.MaxHealth * 0.5f;
        }));

        // Clone -> Patrol (after cloning finished)
        _runner.AddTransition(_cloneState, new Transition(_patrolState, (actor, bb) =>
            bb.GetOrDefault<bool>(BlackboardKeys.CloneFinished)
        ));

        // Hurt -> Patrol (after hurt animation)
        _runner.AddTransition(_hurtState, new Transition(_patrolState, (actor, bb) =>
            bb.GetOrDefault<bool>(BlackboardKeys.HurtFinished)
        ));

        // Patrol -> Chase (player visible) - БЕЗ проверки границ патруля
        _runner.AddTransition(_patrolState, new Transition(_chaseState, (actor, bb) =>
            CanSeePlayer(actor, bb)
        ));

        // Chase -> Patrol (player not visible) - БЕЗ проверки границ патруля
        _runner.AddTransition(_chaseState, new Transition(_patrolState, (actor, bb) =>
            !CanSeePlayer(actor, bb)
        ));

        // Chase -> Attack (player in attack range and cooldown ready)
        _runner.AddTransition(_chaseState, new Transition(_attackState, (actor, bb) =>
        {
            float attackTimer = bb.GetOrDefault<float>(BlackboardKeys.AttackTimer);
            float cooldown = bb.GetOrDefault<float>(BlackboardKeys.AttackCooldown);
            
            return attackTimer >= cooldown && PlayerInAttackRange(actor, bb);
        }));

        // Attack -> Chase (attack finished and player still visible)
        _runner.AddTransition(_attackState, new Transition(_chaseState, (actor, bb) =>
        {
            bool attackFinished = bb.GetOrDefault<bool>(BlackboardKeys.AttackFinished);
            return attackFinished && CanSeePlayer(actor, bb);
        }));

        // Attack -> Patrol (attack finished and player not visible)
        _runner.AddTransition(_attackState, new Transition(_patrolState, (actor, bb) =>
        {
            bool attackFinished = bb.GetOrDefault<bool>(BlackboardKeys.AttackFinished);
            return attackFinished && !CanSeePlayer(actor, bb);
        }));
    }

    private bool CanSeePlayer(GameObject actor, Blackboard bb)
    {
        var playerTransform = bb.GetOrDefault<Transform>(BlackboardKeys.PlayerTransform);
        if (playerTransform == null) return false;

        float visionRange = bb.GetOrDefault<float>(BlackboardKeys.VisionRange);
        float visionHeightOffset = bb.GetOrDefault<float>(BlackboardKeys.VisionHeightOffset);
        LayerMask playerLayer = bb.GetOrDefault<LayerMask>(BlackboardKeys.PlayerLayer);
        LayerMask obstacleLayer = bb.GetOrDefault<LayerMask>(BlackboardKeys.ObstacleLayer);

        Vector2 origin = actor.transform.position + new Vector3(0, visionHeightOffset);
        Vector2 direction = (playerTransform.position - actor.transform.position).normalized;
        float distance = Vector2.Distance(actor.transform.position, playerTransform.position);

        if (distance > visionRange) return false;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, visionRange, playerLayer | obstacleLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            var playerHealth = hit.collider.GetComponent<HealthNew>();
            bb.Set(BlackboardKeys.PlayerHealth, playerHealth);
            return playerHealth != null && !playerHealth.IsDead;
        }

        return false;
    }

    private bool PlayerInAttackRange(GameObject actor, Blackboard bb)
    {
        var boxCollider = bb.GetOrDefault<BoxCollider2D>(BlackboardKeys.BoxCollider);
        float rangeX = bb.GetOrDefault<float>(BlackboardKeys.AttackRangeX);
        float rangeY = bb.GetOrDefault<float>(BlackboardKeys.AttackRangeY);
        float colliderDistanceX = bb.GetOrDefault<float>(BlackboardKeys.ColliderDistanceX);
        float colliderDistanceY = bb.GetOrDefault<float>(BlackboardKeys.ColliderDistanceY);
        LayerMask playerLayer = bb.GetOrDefault<LayerMask>(BlackboardKeys.PlayerLayer);

        Vector2 origin = boxCollider.bounds.center + 
                        actor.transform.up * actor.transform.localScale.y * colliderDistanceY +
                        actor.transform.right * actor.transform.localScale.x * colliderDistanceX;
        Vector2 size = new Vector2(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY);

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0, Vector2.zero, 0, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            var playerHealth = hit.transform.GetComponent<HealthNew>();
            bb.Set(BlackboardKeys.PlayerHealth, playerHealth);
            return playerHealth != null && !playerHealth.IsDead;
        }

        return false;
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

    public void SetAsClone()
    {
        isClone = true;
        if (_runner != null && _runner.Blackboard != null)
        {
            _runner.Blackboard.Set(BlackboardKeys.CanClone, false);
            _runner.Blackboard.Set(BlackboardKeys.IsClone, true);
        }
    }

    // Called from Animation Event
    public void NECKKER_ATTACK()
    {
        var bb = _runner.Blackboard;
        var boxCollider = bb.GetOrDefault<BoxCollider2D>(BlackboardKeys.BoxCollider);
        float rangeX = bb.GetOrDefault<float>(BlackboardKeys.AttackRangeX);
        float rangeY = bb.GetOrDefault<float>(BlackboardKeys.AttackRangeY);
        float colliderDistanceX = bb.GetOrDefault<float>(BlackboardKeys.ColliderDistanceX);
        float colliderDistanceY = bb.GetOrDefault<float>(BlackboardKeys.ColliderDistanceY);
        LayerMask playerLayer = bb.GetOrDefault<LayerMask>(BlackboardKeys.PlayerLayer);
        float damage = bb.GetOrDefault<float>(BlackboardKeys.AttackDamage);

        Vector2 origin = boxCollider.bounds.center + 
                        transform.up * transform.localScale.y * colliderDistanceY +
                        transform.right * transform.localScale.x * colliderDistanceX;
        Vector2 size = new Vector2(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY);

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, Vector2.zero, 0f, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            HealthNew health = hit.collider.GetComponent<HealthNew>();
            if (health != null && !health.IsDead)
            {
                PlayAttackSound();
                health.TakeDamage(damage);
                Debug.Log("Neckker hit player: " + hit.collider.name);
            }
        }
        else
        {
            Debug.Log("Neckker attack missed.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_runner.Blackboard.GetOrDefault<bool>(BlackboardKeys.IsDead)) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            float contactCooldown = _runner.Blackboard.GetOrDefault<float>(BlackboardKeys.ContactDamageCooldown);
            float contactDelay = _runner.Blackboard.GetOrDefault<float>(BlackboardKeys.ContactDamageDelay);
            
            if (contactCooldown >= contactDelay)
            {
                HealthNew playerHealth = collision.gameObject.GetComponent<HealthNew>();
                if (playerHealth != null && !playerHealth.IsDead)
                {
                    PlayAttackSound();
                    playerHealth.TakeDamage(damage);
                    Debug.Log("Player took contact damage!");
                    _runner.Blackboard.Set(BlackboardKeys.ContactDamageCooldown, 0f);
                    contactDamageCooldown = 0f;
                }
            }
        }
    }

    private void PlayAttackSound()
    {
        if (attackSound != null)
        {
            soundManager?.PlaySound("NeckkerAttack");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (boxCollider == null) return;

        Gizmos.color = Color.red;
        Vector2 origin = boxCollider.bounds.center + 
                        transform.up * transform.localScale.y * colliderDistanceY +
                        transform.right * transform.localScale.x * colliderDistanceX;
        Vector2 size = new Vector2(boxCollider.bounds.size.x * attackRangeX, boxCollider.bounds.size.y * attackRangeY);
        Gizmos.DrawWireCube(origin, size);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, visionHeightOffset), visionRange);
    }
}
