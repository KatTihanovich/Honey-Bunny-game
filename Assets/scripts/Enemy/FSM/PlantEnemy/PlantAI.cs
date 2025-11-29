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

        _runner.SetInitialState(ChooseIdleState());
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
        var nextState = new HurtState(_runner, _runner.Blackboard, ChooseIdleState());
        _runner.ChangeState(nextState);
    }

    private void HandleDeath()
    {
        var bb = _runner.Blackboard;
        if (bb.GetOrDefault<bool>(BlackboardKeys.IsDead)) return;

        bb.Set(BlackboardKeys.IsDead, true);
        _runner.ChangeState(new DeathState(_runner, bb, deathSoundDelay));
    }

    public IState ChooseIdleState()
    {
        var bb = _runner.Blackboard;
        var playerHp = bb.GetOrDefault<HealthNew>(BlackboardKeys.PlayerHealth);
        var anim = bb.GetOrDefault<Animator>(BlackboardKeys.Animator);

        if (playerHp == null || playerHp.IsDead)
        {
            anim?.SetBool("IsStressed", false);
            anim?.SetBool("IsMad", false);
            return new SafeIdleState(_runner, bb);
        }

        float hp = playerHp.CurrentHealth;

        if (hp > 70f)
        {
            anim?.SetBool("IsStressed", false);
            anim?.SetBool("IsMad", false);
            return new SafeIdleState(_runner, bb);
        }
        else if (hp > 50f)
        {
            anim?.SetBool("IsStressed", true);
            anim?.SetBool("IsMad", false);
            return new AlertIdleState(_runner, bb);
        }
        else
        {
            anim?.SetBool("IsStressed", true);
            anim?.SetBool("IsMad", true);
            return new RageIdleState(_runner, bb);
        }
    }
}
