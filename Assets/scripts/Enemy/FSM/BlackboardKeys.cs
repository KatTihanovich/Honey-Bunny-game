public static class BlackboardKeys
{
    // ========== Общие ключи ==========
    public const string Animator = "Animator";
    public const string SelfHealth = "SelfHealth";
    public const string PlayerHealth = "PlayerHealth";
    public const string SoundManager = "SoundManager";
    public const string IsDead = "IsDead";
    
    // ========== Plant-специфичные ==========
    public const string IsPlayerInRange = "IsPlayerInRange";
    public const string AttackDamage = "AttackDamage";
    public const string AttackCooldown = "AttackCooldown";
    public const string AttackDelay = "AttackDelay";
    public const string HurtAnimationFinished = "HurtAnimationFinished";
    public const string AttackFinished = "AttackFinished";
    
    // ========== Burder-специфичные ==========
    public const string PlayerTransform = "PlayerTransform";
    public const string MoveSpeed = "MoveSpeed";
    public const string RunSpeed = "RunSpeed";
    public const string BaseScale = "BaseScale";
    public const string AgroDistance = "AgroDistance";
    public const string LostDistance = "LostDistance";
    public const string WalkDistance = "WalkDistance";
    public const string RunDistance = "RunDistance";
    public const string IsChasing = "IsChasing";
    public const string AlwaysChasePlayer = "AlwaysChasePlayer";
    public const string AttackRange = "AttackRange";
    public const string AttackRangeX = "AttackRangeX";
    public const string AttackRangeY = "AttackRangeY";
    public const string ColliderDistanceX = "ColliderDistanceX";
    public const string ColliderDistanceY = "ColliderDistanceY";
    public const string BoxCollider = "BoxCollider";
    public const string PlayerLayer = "PlayerLayer";
    public const string AttackTimer = "AttackTimer";
    public const string PointA = "PointA";
    public const string PointB = "PointB";
    public const string CurrentPatrolPoint = "CurrentPatrolPoint";
    public const string FleeDistance = "FleeDistance";
    public const string FleeSpeed = "FleeSpeed";
    public const string FleeDelay = "FleeDelay";
    public const string FleeDirection = "FleeDirection";
    public const string FleeFinished = "FleeFinished";
    public const string HurtFinished = "HurtFinished";
    public const string CanSpawnThorns = "CanSpawnThorns";
    public const string ThornCooldown = "ThornCooldown";
    public const string ThornPrefab = "ThornPrefab";
    public const string ThornTimer = "ThornTimer";
    public const string ThornSpawnFinished = "ThornSpawnFinished";
    
    // ========== Neckker-специфичные ==========
    public const string LeftEdge = "LeftEdge";
    public const string RightEdge = "RightEdge";
    public const string MovingLeft = "MovingLeft";
    public const string WaitTimeAtPoint = "WaitTimeAtPoint";
    public const string WaitTimer = "WaitTimer";
    public const string Rigidbody = "Rigidbody";
    public const string VisionRange = "VisionRange";
    public const string VisionHeightOffset = "VisionHeightOffset";
    public const string ObstacleLayer = "ObstacleLayer";
    public const string PatrolBounds = "PatrolBounds";
    public const string ChaseStopDistance = "ChaseStopDistance";
    public const string CanClone = "CanClone";
    public const string IsClone = "IsClone";
    public const string HasCloned = "HasCloned";
    public const string CloneOffset = "CloneOffset";
    public const string MobPrefab = "MobPrefab";
    public const string CloneFinished = "CloneFinished";
    public const string ContactDamageCooldown = "ContactDamageCooldown";
    public const string ContactDamageDelay = "ContactDamageDelay";

    // ========== Boss-специфичные ключи ==========
public const string BossAnimator = "BossAnimator";
public const string BossBoxCollider = "BossBoxCollider";
public const string BossRenderer = "BossRenderer";
public const string BossAttackArea = "BossAttackArea";
public const string BossTails = "BossTails";
public const string BossInitialScale = "BossInitialScale";
public const string BossSpawnPoint = "BossSpawnPoint";
public const string AttackAreaScript = "AttackAreaScript";
public const string IsFirstAppear = "IsFirstAppear";

// Таймеры и счётчики
public const string ActiveTimer = "ActiveTimer";
public const string DamageTakenThisPhase = "DamageTakenThisPhase";
public const string HitsDoneThisPhase = "HitsDoneThisPhase";

// Флаги состояний
public const string IsVisible = "IsVisible";
public const string CanTakeDamage = "CanTakeDamage";
public const string IsRoaring = "IsRoaring";
public const string HasRoared = "HasRoared";

// Настройки длительностей
public const string AppearDuration = "AppearDuration";
public const string DisappearDuration = "DisappearDuration";
public const string ActivePhaseDuration = "ActivePhaseDuration";
public const string HiddenDuration = "HiddenDuration";
public const string RoarDuration = "RoarDuration";

// Спавн миньонов
public const string EnemyPrefabs = "EnemyPrefabs";
public const string SpawnPoints = "SpawnPoints";
public const string SpawnInterval = "SpawnInterval";
public const string MaxEnemies = "MaxEnemies";

public static readonly string ChaseStateActive = "ChaseStateActive";
public static readonly string ChaseCurrentPhase = "ChaseCurrentPhase";
public static readonly string ChaseTimer = "ChaseTimer";
public const string ReturningFromAttack = "ReturningFromAttack"; 
public const string JustHit = "JustHit";                   // bool

}
