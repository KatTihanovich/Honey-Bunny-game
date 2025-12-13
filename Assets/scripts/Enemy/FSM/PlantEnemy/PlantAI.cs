using UnityEngine;
using Game.Audio;

[RequireComponent(typeof(HealthNew))]
[RequireComponent(typeof(EnemyStateMachineRunner))]
public class PlantAI : MonoBehaviour
{
    [Header("Settings")]
    public Animator animator;
    public float attackDamage = 10f;
    public float attackCooldown = 2f;
    public float attackDelay = 1f;
    public float deathSoundDelay = 2f;

    [Header("Attack Zone Collider")]
    [SerializeField] private Collider2D attackZoneCollider;

    private HealthNew playerHealth;
    private HealthNew selfHealth;
    private ISoundManager soundManager;
    private EnemyStateMachineRunner _runner;
    
    private SafeIdleState _safeIdleState;
    private RageIdleState _rageIdleState;
    private AttackState _attackState;
    private HurtState _hurtState;
    private DeathState _deathState;

    private void Awake()
    {
        selfHealth = GetComponent<HealthNew>();
        _runner = GetComponent<EnemyStateMachineRunner>();
        selfHealth.OnDeath += HandleDeath;
        selfHealth.OnDamaged += HandleDamaged;
    }

    private void Start()
    {
        soundManager = SoundManagerNew.Instance;
        playerHealth = GameObject.FindGameObjectWithTag("Player")?.GetComponent<HealthNew>();

        var bb = _runner.Blackboard;
        bb.Set(BlackboardKeys.Animator, animator);
        bb.Set(BlackboardKeys.SelfHealth, selfHealth);
        bb.Set(BlackboardKeys.PlayerHealth, playerHealth);
        bb.Set(BlackboardKeys.SoundManager, soundManager);
        bb.Set(BlackboardKeys.AttackDamage, attackDamage);
        bb.Set(BlackboardKeys.AttackCooldown, attackCooldown);
        bb.Set(BlackboardKeys.AttackDelay, attackDelay);
        bb.Set(BlackboardKeys.IsPlayerInRange, false);
        bb.Set(BlackboardKeys.IsDead, false);
        bb.Set(BlackboardKeys.HurtAnimationFinished, false);
        bb.Set(BlackboardKeys.AttackFinished, false);

        _safeIdleState = new SafeIdleState();
        _rageIdleState = new RageIdleState();
        _attackState = new AttackState();
        _hurtState = new HurtState();
        _deathState = new DeathState(deathSoundDelay);

        SetupTransitions();
        _runner.SetInitialState(GetInitialIdleState());
    }

    private void SetupTransitions()
    {
        _runner.AddAnyTransition(new Transition(_deathState, (actor, bb) =>
            bb.GetOrDefault<bool>(BlackboardKeys.IsDead)
        ));

        _runner.AddTransition(_safeIdleState, new Transition(_rageIdleState, (actor, bb) =>
        {
            var playerHp = bb.GetOrDefault<HealthNew>(BlackboardKeys.PlayerHealth);
            return playerHp != null && !playerHp.IsDead && playerHp.CurrentHealth <= 70f;
        }));

        _runner.AddTransition(_rageIdleState, new Transition(_safeIdleState, (actor, bb) =>
        {
            var playerHp = bb.GetOrDefault<HealthNew>(BlackboardKeys.PlayerHealth);
            return playerHp == null || playerHp.IsDead || playerHp.CurrentHealth > 70f;
        }));

        _runner.AddTransition(_rageIdleState, new Transition(_attackState, (actor, bb) =>
            bb.GetOrDefault<bool>(BlackboardKeys.IsPlayerInRange) && !bb.GetOrDefault<bool>(BlackboardKeys.IsDead)
        ));

        _runner.AddTransition(_attackState, new Transition(_safeIdleState, (actor, bb) =>
        {
            if (!bb.GetOrDefault<bool>(BlackboardKeys.AttackFinished)) return false;
            
            var playerHp = bb.GetOrDefault<HealthNew>(BlackboardKeys.PlayerHealth);
            return playerHp == null || playerHp.IsDead || playerHp.CurrentHealth > 70f;
        }));

        _runner.AddTransition(_attackState, new Transition(_rageIdleState, (actor, bb) =>
        {
            if (!bb.GetOrDefault<bool>(BlackboardKeys.AttackFinished)) return false;
            
            var playerHp = bb.GetOrDefault<HealthNew>(BlackboardKeys.PlayerHealth);
            return playerHp != null && !playerHp.IsDead && playerHp.CurrentHealth <= 70f;
        }));

        _runner.AddTransition(_hurtState, new Transition(_safeIdleState, (actor, bb) =>
        {
            if (!bb.GetOrDefault<bool>(BlackboardKeys.HurtAnimationFinished)) return false;
            var playerHp = bb.GetOrDefault<HealthNew>(BlackboardKeys.PlayerHealth);
            return playerHp == null || playerHp.IsDead || playerHp.CurrentHealth > 70f;
        }));

        _runner.AddTransition(_hurtState, new Transition(_rageIdleState, (actor, bb) =>
        {
            if (!bb.GetOrDefault<bool>(BlackboardKeys.HurtAnimationFinished)) return false;
            var playerHp = bb.GetOrDefault<HealthNew>(BlackboardKeys.PlayerHealth);
            return playerHp != null && !playerHp.IsDead && playerHp.CurrentHealth <= 70f;
        }));
    }

    private IState GetInitialIdleState()
    {
        if (playerHealth == null || playerHealth.IsDead)
            return _safeIdleState;

        float hp = playerHealth.CurrentHealth;
        if (hp > 70f)
            return _safeIdleState;
        else
            return _rageIdleState;
    }

    public void SetPlayerInRange(bool value)
    {
        _runner.Blackboard.Set(BlackboardKeys.IsPlayerInRange, value);
    }

    public void TakeDamage(float damage)
    {
        if (_runner.Blackboard.GetOrDefault<bool>(BlackboardKeys.IsDead)) return;
        
        if (playerHealth != null && playerHealth.CurrentHealth > 70f)
        {
            Debug.Log("Монстр не получил урон — здоровье игрока выше 70.");
            return;
        }

        selfHealth.TakeDamage(damage);
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
}
