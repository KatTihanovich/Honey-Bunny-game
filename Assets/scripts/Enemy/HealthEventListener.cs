using UnityEngine;

public class HealthEventListener : MonoBehaviour
{
    private static readonly int GotHit = Animator.StringToHash("GotHit");
    private static readonly int Dead = Animator.StringToHash("Dead");
    [SerializeField] private Health health;
    [SerializeField] private Animator anim; // Animator доступен через инспектор
    private readonly bool canMove = true;

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
            anim.SetTrigger(GotHit);
        }
        else
        {
            anim.SetTrigger(Dead);
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
