using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Point")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float waitTimeAtPoint; // Время ожидания на точке патрулирования

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    private bool movingLeft;
    private bool isStopped = false;
    private float waitTimer = 0f;
    private Vector3 baseScale;

    private void Start()
    {
        baseScale = enemy.localScale; // Сохраняем базовый масштаб
    }

    private void Update()
    {
        if (isStopped)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Логика таймера ожидания
        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            rb.linearVelocity = Vector2.zero; // Остановить движение во время ожидания
            return;
        }

        // Движение в сторону левой или правой границы
        if (movingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
            {
                MoveInDirection(-1);
            }
            else
            {
                StartWaiting(); // Ожидание на левой точке
            }
        }
        else
        {
            if (enemy.position.x <= rightEdge.position.x)
            {
                MoveInDirection(1);
            }
            else
            {
                StartWaiting(); // Ожидание на правой точке
            }
        }
    }

    private void MoveInDirection(int direction)
    {
        // Устанавливаем направление, используя базовый масштаб
        Vector3 localScale = baseScale;
        localScale.x *= (direction > 0 ? 1 : -1); // Инверсия по X в зависимости от направления
        enemy.localScale = localScale;
        
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    private void StartWaiting()
    {
        DirectionChange();
        waitTimer = waitTimeAtPoint; // Устанавливаем время ожидания
    }

    private void DirectionChange()
    {
        movingLeft = !movingLeft;
    }

    public void StopMovement()
    {
        isStopped = true;
        rb.linearVelocity = Vector2.zero;
    }

    public void ResumeMovement()
    {
        isStopped = false;
    }
}
