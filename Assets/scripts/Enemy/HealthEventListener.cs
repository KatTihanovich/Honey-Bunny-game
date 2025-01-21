using UnityEngine;

public class HealthEventListener : MonoBehaviour
{
    [SerializeField] private Health health; // Ссылка на компонент Health
    private bool canMove = true;

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnHealthChanged += HandleHealthChanged;
            health.OnDeath += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnHealthChanged -= HandleHealthChanged;
            health.OnDeath -= HandleDeath;
        }
    }

    private void HandleHealthChanged(float currentHealth)
    {
        if (currentHealth > 0)
        {
            Debug.Log("Health changed: " + currentHealth);

            if (health.skeletonAnimation != null && health.hit != null)
            {
                health.skeletonAnimation.state.SetAnimation(0, health.hit, false).TimeScale = 1f;
            }
        }
    }

    private void HandleDeath()
    {
        Debug.Log("Character is dead.");
        // Запускаем анимацию смерти
        health.skeletonAnimation.state.SetAnimation(0, health.death, false).TimeScale = 1f;
        Invoke(nameof(DestroyObject), 2f);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
