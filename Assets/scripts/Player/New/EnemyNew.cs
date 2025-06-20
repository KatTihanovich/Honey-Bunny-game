using UnityEngine;

public class EnemyNew : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 5f;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] private float enemyKnockbackForce = 3f;
    private float health;

    [Header("Damage Text Settings")]
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Vector3 textOffset = new Vector3(0f, 1f, 0f);

    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 0.2f; // Длительность мигания
    [SerializeField] private int flashCount = 2; // Количество миганий

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // Для эффекта мигания
    private bool isDead;
    private Color originalColor; // Оригинальный цвет спрайта

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Получаем SpriteRenderer
        health = maxHealth;

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on Enemy!");
        }
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on Enemy!");
        }
        else
        {
            originalColor = spriteRenderer.color; // Сохраняем оригинальный цвет
        }
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        PlayerController player = other.GetComponent<PlayerController>();
    //        if (player != null)
    //        {
    //            player.TakeDamage(damage);
    //            ApplyKnockback(player);
    //        }
    //    }
    //}

    private void ApplyKnockback(PlayerController player)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
            playerRb.linearVelocity = Vector2.zero;
            playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log($"Enemy took {damage} damage. Current health: {health}");

        // Отображаем текст урона
        ShowDamageText(damage);

        // Запускаем эффект мигания
        StartCoroutine(FlashRed());

        if (health <= 0)
        {
            Die();
        }
        else
        {
            ApplyKnockbackToSelf(attackerPosition);
        }
    }

    private System.Collections.IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;

        float flashInterval = flashDuration / (flashCount * 2); // Делим на количество переключений (вкл/выкл)

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = Color.red; // Устанавливаем красный цвет
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.color = originalColor; // Возвращаем оригинальный цвет
            yield return new WaitForSeconds(flashInterval);
        }
    }

    private void ShowDamageText(int damage)
    {
        if (damageTextPrefab != null)
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                Debug.LogError("Canvas not found! Make sure there is a Canvas in the scene.");
                return;
            }

            GameObject textObj = Instantiate(damageTextPrefab, canvas.transform);
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position + textOffset);
            textObj.transform.position = screenPosition;

            //DamageText damageText = textObj.GetComponent<DamageText>();
            //if (damageText != null)
            //{
            //    damageText.SetDamage(damage);
            //}
        }
        else
        {
            Debug.LogWarning("Damage Text Prefab is not assigned in Enemy!");
        }
    }

    private void ApplyKnockbackToSelf(Vector2 attackerPosition)
    {
        Vector2 knockbackDirection = (transform.position - (Vector3)attackerPosition).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * enemyKnockbackForce, ForceMode2D.Impulse);
    }

    private void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}