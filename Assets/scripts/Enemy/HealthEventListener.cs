using UnityEngine;

public class HealthEventListener : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Animator anim; // Animator доступен через инспектор
    private bool canMove = true;

    private void Start()
    {
        if (health != null)
        {
            health.OnHealthChanged += HandleHealthChanged; // Подпишемся на событие изменения здоровья
        }
    }

    private void HandleHealthChanged(float currentHealth)
    {
        if (currentHealth > 0)
        {
            Debug.Log("Health changed: " + currentHealth);
            anim.SetTrigger("GotHit");
        }
        else
        {
            anim.SetTrigger("Dead");
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        Debug.Log("Character is dead.");
        Invoke(nameof(DestroyObject), 2f);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
