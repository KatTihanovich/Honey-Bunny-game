using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    public Animator animator;
    public float attackDamage = 10f;
    public float attackCooldown = 3f;
    private bool isPlayerInRange = false;
    private bool canAttack = true;

    private HealthNew playerHealth;

    void Start()
{
    GameObject player = GameObject.FindGameObjectWithTag("Player");

    if (player != null)
    {
        Debug.Log("Игрок найден: " + player.name + " Активен: " + player.activeSelf);
        playerHealth = player.GetComponent<HealthNew>();

        if (playerHealth != null)
        {
            Debug.Log("Компонент HealthNew найден на объекте игрока!");
        }
        else
        {
            Debug.LogError("Не найден компонент HealthNew на объекте игрока!");
        }
    }
    else
    {
        Debug.LogError("Не найден объект с тегом Player!");
    }
}



    void Update()
    {
        if (playerHealth == null || playerHealth.IsDead)
            return;

        float health = playerHealth.CurrentHealth;

        // Анимации в зависимости от здоровья игрока
        if (health < 50f)
        {
            animator.SetBool("IsMad", true);
            animator.SetBool("IsStressed", true);
        }
        else if (health < 70f)
        {
            animator.SetBool("IsMad", false);
            animator.SetBool("IsStressed", true);
        }
        else
        {
            animator.SetBool("IsMad", false);
            animator.SetBool("IsStressed", false);
        }

        // Атака игрока, если он в пределах зоны
        if (isPlayerInRange && health < 50f && canAttack)
        {
            animator.SetTrigger("Attack");
            canAttack = false; // блокируем повторную атаку
        }
    }

    public void DealDamage()
    {
        if (playerHealth != null && isPlayerInRange && !playerHealth.IsDead)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Монстр нанёс урон: " + attackDamage);
        }

        // запускаем перезарядку
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
