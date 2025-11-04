using UnityEngine;
using System.Collections;
using Game.Audio;

[RequireComponent(typeof(HealthNew))]
public class MonsterAI : MonoBehaviour
{
    [Header("Settings")]
    public Animator animator;
    public float attackDamage = 10f;
    public float attackCooldown = 3f;
    public float attackDelay = 0.5f;

    [Header("Attack Zone Collider")]
    [SerializeField] private Collider2D attackZoneCollider;

    private HealthNew playerHealth;
    private HealthNew selfHealth;
    private ISoundManager soundManager;

    private Coroutine attackCoroutine;
    private bool isPlayerInRange = false;
    private bool isDead = false;

    private void Awake()
    {
        selfHealth = GetComponent<HealthNew>();

        if (selfHealth != null)
        {
            selfHealth.OnDeath += HandleDeath;
            selfHealth.OnDamaged += HandleDamaged;
        }
    }

    private void Start()
    {
        soundManager = SoundManagerNew.Instance;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<HealthNew>();
        }

        if (animator == null)
        {
            Debug.LogError("Animator не установлен в MonsterAI.");
        }

        if (attackZoneCollider == null)
        {
            Debug.LogError("AttackZoneCollider не установлен!");
        }
    }

    private void Update()
{
    if (isDead || playerHealth == null || playerHealth.IsDead)
        return;

    float health = playerHealth.CurrentHealth;

    // Добавленный лог
    Debug.Log($"[MonsterAI] Player HP: {health}, IsStressed: {health <= 70f}, IsMad: {health <= 50f}");

    animator.SetBool("IsMad", health <= 50f);
    animator.SetBool("IsStressed", health <= 70f);

    if (isPlayerInRange && health <= 70f && attackCoroutine == null)
    {
        attackCoroutine = StartCoroutine(PerformAttack());
    }
}

    private IEnumerator PerformAttack()
    {
        Debug.Log("Монстр начинает атаку.");
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackDelay);

        soundManager.PlaySound("WhipAttack");

        if (playerHealth != null && isPlayerInRange && !playerHealth.IsDead)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Монстр нанёс урон игроку: " + attackDamage);
        }

        yield return new WaitForSeconds(attackCooldown);
        attackCoroutine = null;
    }

    public void TakeDamage(float damage)
    {
        if (isDead || selfHealth == null || damage <= 0f)
            return;

        if (playerHealth != null && playerHealth.CurrentHealth <= 70f)
        {
            selfHealth.TakeDamage(damage);
        }
        else
        {
            Debug.Log("Монстр не получил урон — здоровье игрока выше 70.");
        }
    }

    private void HandleDamaged(float damage)
    {
        if (isDead) return;

        animator.SetTrigger("GotHit");
        soundManager.PlaySound("Damage");
        Debug.Log($"{gameObject.name} получил урон: {damage}");
    }

    private void HandleDeath()
    {
        if (isDead) return;

        isDead = true;
        animator.SetBool("Dead", true);
        soundManager.PlaySound("MobDeath");

        if (TryGetComponent<Collider2D>(out var bodyCol)) bodyCol.enabled = false;
        if (TryGetComponent<Rigidbody2D>(out var rb)) rb.linearVelocity = Vector2.zero;

        Destroy(gameObject, 4f);
        EndWindow.IncreaseEnemyCount();
    }

    public void SetPlayerInRange(bool value)
    {
        isPlayerInRange = value;
    }
}