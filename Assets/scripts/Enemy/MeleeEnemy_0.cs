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
    private Health playerHealth;
    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;
    private Vector3 baseScale;

    private void Start()
    {
        baseScale = transform.localScale;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            anim.SetBool("Run", false);

            if (cooldownTimer >= attackCooldown)
            {
                patrolScript.StartWaiting();
                anim.SetTrigger("Attack");
                RotateTowardsPlayer();
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        if (rotateTowardsPlayer)
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

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.up * transform.localScale.y * colliderDistanceY + transform.right * transform.localScale.x * colliderDistanceX,
            new Vector3(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY, boxCollider.bounds.size.z),
            0, Vector2.zero, 0, playerLayer);

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
        Debug.Log("NECKKER_HIT event triggered!");
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
    public void TestSpineEvent()
{
    Debug.Log("NECKKER_HIT event received!");
}

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(
    //        boxCollider.bounds.center + transform.up * transform.localScale.y * colliderDistanceY + transform.right * transform.localScale.x * colliderDistanceX,
    //        new Vector3(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY, boxCollider.bounds.size.z));
    //}
}
