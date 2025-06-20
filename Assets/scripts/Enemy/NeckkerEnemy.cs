using UnityEngine;

public class NeckkerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float damage = 10f;                      // Урон от атаки
    [SerializeField] private float attackDelay = 0.5f;                // Задержка до нанесения урона
    [SerializeField] private float attackCooldown = 2f;               // Время между атаками
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private BoxCollider2D attackArea;                // Коллайдер зоны атаки

    private Animator anim;
    private bool playerInRange = false;                               // Флаг, в зоне ли игрок
    private float lastAttackTime = -Mathf.Infinity;                   // Время последней атаки

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Time.time >= lastAttackTime + attackCooldown && PlayerInAttackRange())
        {
            TriggerAttack();
        }
    }

    private bool PlayerInAttackRange()
    {
        Collider2D hit = Physics2D.OverlapBox(attackArea.bounds.center, attackArea.bounds.size, 0f, playerLayer);
        playerInRange = hit != null;
        return playerInRange;
    }

    private void TriggerAttack()
    {
        if (playerInRange)
        {
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;  // Обновляем время последней атаки
            Invoke(nameof(DealDamage), attackDelay);
        }
    }

    private void DealDamage()
    {
        if (playerInRange)
        {
            Collider2D hit = Physics2D.OverlapBox(attackArea.bounds.center, attackArea.bounds.size, 0f, playerLayer);

            if (hit != null)
            {
                HealthNew playerHealth = hit.GetComponent<HealthNew>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log("Player damaged by Neckker!");
                }
            }
        }
    }

    // Для отображения зоны атаки в редакторе
    private void OnDrawGizmosSelected()
    {
        if (attackArea != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackArea.bounds.center, attackArea.bounds.size);
        }
    }
}
