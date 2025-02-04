using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    private static readonly int Run = Animator.StringToHash("Run");

    [Header("Patrol Point")] [SerializeField]
    private Transform leftEdge;

    [SerializeField] private Transform rightEdge;

    [Header("Enemy")] [SerializeField] private Transform enemy;
    [SerializeField] private Animator anim;

    [Header("Movement parameters")] [SerializeField]
    private float speed;

    [SerializeField] private float waitTimeAtPoint;

    [Header("Components")] [SerializeField]
    private Rigidbody2D rb;

    private bool movingLeft;
    private float waitTimer;
    private Vector3 baseScale;

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
        if (!enemy) return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            if (rb)
            {
                rb.linearVelocity = Vector2.zero;
            }

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

    private void MoveInDirection(int direction)
    {
        Vector3 localScale = baseScale;
        localScale.x *= direction > 0 ? 1 : -1;
        enemy.localScale = localScale;

        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    public void StartWaiting()
    {
        rb.linearVelocity = Vector2.zero;
        movingLeft = !movingLeft;
        waitTimer = waitTimeAtPoint;
    }
}