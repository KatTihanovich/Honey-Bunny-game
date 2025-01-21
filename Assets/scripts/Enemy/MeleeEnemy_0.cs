using Spine.Unity;
using UnityEngine;

public class MeleeEnemy_0 : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float rangeX;
    [SerializeField] private float rangeY;
    [SerializeField] private float colliderDistanceX;
    [SerializeField] private float colliderDistanceY;
    [SerializeField] private int damage;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;



    [Header("Movement Settings")]
    [SerializeField] private bool rotateTowardsPlayer = true;
    [SerializeField] private EnemyPatrol patrolScript;

    [Header("Animation Settings")]
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, walking, attacking;
    private string currentAnimation;

    private Rigidbody2D rb;
    private Health playerHealth;
    private float cooldownTimer = Mathf.Infinity;
    private bool isAttacking = false;
    private Vector3 baseScale;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        baseScale = transform.localScale; // Сохраняем базовый масштаб
    }

    private void Update()
    {
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        cooldownTimer += Time.fixedDeltaTime;

        float velocityX = rb.linearVelocity.x;
        if (Mathf.Abs(velocityX) > 0.1f)
        {
            SetAnimation(walking, true, 1f);
        }
        else
        {
            SetAnimation(idle, true, 1f);
        }

        if (PlayerInSight() && cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0;
            StartAttack();
        }

        if (rotateTowardsPlayer && PlayerInSight())
        {
            Vector3 direction = playerHealth.transform.position - transform.position;
            Vector3 localScale = baseScale;
            if (direction.x > 0)
                localScale.x = Mathf.Abs(baseScale.x);
            else
                localScale.x = -Mathf.Abs(baseScale.x);

            transform.localScale = localScale;
        }
    }

    private void StartAttack()
    {
        if (patrolScript != null)
        {
            patrolScript.StopMovement(); // Остановить движение во время атаки
        }

        isAttacking = true;
        SetAnimation(attacking, false, 1f);

        // После завершения анимации атаки, сбрасываем флаг
        skeletonAnimation.state.Complete += OnAttackComplete;
    }

    private void OnAttackComplete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == attacking.name)
        {
            isAttacking = false;

            if (patrolScript != null)
            {
                patrolScript.ResumeMovement(); // Возобновить движение
            }

            skeletonAnimation.state.Complete -= OnAttackComplete;
        }
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.up * transform.localScale.y * colliderDistanceY + transform.right * transform.localScale.x * colliderDistanceX,
            new Vector3(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            playerHealth = hit.transform.GetComponent<Health>();
            return playerHealth != null;
        }

        playerHealth = null;
        return false;
    }

    public void DamagePlayer()
    {
        if (PlayerInSight() && playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("Player damaged by enemy!");
        }
        else
        {
            Debug.Log("Player not in sight, no damage dealt.");
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.up * transform.localScale.y * colliderDistanceY + transform.right * transform.localScale.x * colliderDistanceX,
            new Vector3(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY, boxCollider.bounds.size.z));
    }

    private void SetAnimation(AnimationReferenceAsset animation, bool loop, float timescale)
    {
        if (animation == null || animation.name == currentAnimation)
        {
            return;
        }

        skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timescale;
        currentAnimation = animation.name;
    }


}
