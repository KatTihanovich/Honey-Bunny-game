using UnityEngine;
using System.Collections;

public class MonsterAI : MonoBehaviour
{
    public Animator animator;
    public float attackDamage = 10f;
    public float attackCooldown = 3f;
    public float attackDelay = 0.5f; // задержка до нанесения урона после начала анимации

    private HealthNew playerHealth;
    private bool isPlayerInRange = false;
    private Coroutine attackCoroutine;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerHealth = player.GetComponent<HealthNew>();
            if (playerHealth == null)
            {
                Debug.LogError("Компонент HealthNew не найден на объекте игрока.");
            }
        }
        else
        {
            Debug.LogError("Игрок с тегом 'Player' не найден.");
        }

        if (animator == null)
        {
            Debug.LogError("Animator не привязан к MonsterAI.");
        }
    }

    void Update()
    {
        if (playerHealth == null || playerHealth.IsDead)
            return;

        float health = playerHealth.CurrentHealth;

        // Эмоциональные состояния монстра
        animator.SetBool("IsMad", health <= 50f);
        animator.SetBool("IsStressed", health <= 70f);

        // Атака, если игрок рядом и здоровья у него мало
        if (isPlayerInRange && health <= 70f && attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
{
    Debug.Log("Атака запущена");
    animator.SetTrigger("Attack");

    yield return new WaitForSeconds(attackDelay);

    bool tookDamage = false;

    if (playerHealth != null && isPlayerInRange && !playerHealth.IsDead)
    {
        Debug.Log("Монстр собирается нанести урон: " + attackDamage);

        try
        {
            if (playerHealth.gameObject != null)
            {
                playerHealth.TakeDamage(attackDamage);
                tookDamage = true;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Ошибка при атаке: " + ex.Message);
        }
    }

    // Подождать 1 кадр и проверить, жив ли игрок
    yield return null;

    if (tookDamage && playerHealth != null && playerHealth.gameObject != null)
    {
        Debug.Log("Монстр завершил атаку");
    }
    else
    {
        Debug.Log("Объект игрока был уничтожен или null после атаки");
    }

    yield return new WaitForSeconds(attackCooldown);

    attackCoroutine = null;
    Debug.Log("Атака завершена, можно атаковать снова");
}




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Игрок вошёл в зону атаки");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Игрок покинул зону атаки");
        }
    }
}
