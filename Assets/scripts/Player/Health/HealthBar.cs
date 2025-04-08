using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image currenthealthBar;

    private void OnEnable()
    {
        playerHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHealthBar;
    }

    private void Start()
    {
        // Set initial value
        UpdateHealthBar(playerHealth.CurrentHealth);
    }

    private void UpdateHealthBar(float currentHealth)
    {
        currenthealthBar.fillAmount = currentHealth / playerHealth.startingHealth;
    }
}
