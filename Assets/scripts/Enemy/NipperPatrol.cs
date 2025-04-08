using UnityEngine;

public class NipperPatrol : PatrolBase
{
    private static readonly int Run = Animator.StringToHash("Run");
    [Header("Vision Settings")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float visionHeightOffset = 0.5f; // Adjust if enemy origin is at feet


    [Header("Patrol Point")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;
    [SerializeField] private Animator anim;

    [Header("Movement parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float waitTimeAtPoint;

    [Header("Chase parameters")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 10f;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    private bool movingLeft;
    private float waitTimer;
    private Vector3 baseScale;
    private bool isChasing;

    private void Start()
    {
        baseScale = enemy.localScale;
    }

    private void OnDisable()
    {
        if (rb != null && anim != null)
        {
            anim.SetBool(Run, false);
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Update()
    {
        if (!enemy || !player) return;

        if (CanSeePlayer())
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            Patrol();
        }
    }


    private void Patrol()
    {
        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (movingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
            {
                MoveInDirection(-1);
                anim.SetBool(Run, true);
            }
            else
            {
                anim.SetBool(Run, false);
                StartWaiting();
            }
        }
        else
        {
            if (enemy.position.x <= rightEdge.position.x)
            {
                MoveInDirection(1);
                anim.SetBool(Run, true);
            }
            else
            {
                anim.SetBool(Run, false);
                StartWaiting();
            }
        }
    }

    private void ChasePlayer()
    {
        int direction = player.position.x > enemy.position.x ? 1 : -1;

        MoveInDirection(direction);
        anim.SetBool(Run, true);
    }

    private bool CanSeePlayer()
    {
        Vector2 origin = enemy.position + new Vector3(0, visionHeightOffset);
        Vector2 direction = (player.position - enemy.position).normalized;
        float distance = Vector2.Distance(enemy.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, visionRange, playerLayer | obstacleLayer);

        if (hit.collider != null)
        {
            // If we hit the player and nothing else
            return hit.collider.CompareTag("Player");
        }

        return false;
    }


    private void MoveInDirection(int direction)
    {
        Vector3 localScale = baseScale;
        localScale.x *= direction > 0 ? 1 : -1;
        enemy.localScale = localScale;

        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    public override void StartWaiting()
    {
        rb.linearVelocity = Vector2.zero;
        movingLeft = !movingLeft;
        waitTimer = waitTimeAtPoint;
    }
}
