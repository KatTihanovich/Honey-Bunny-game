using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private HealthNew _health;

    private void Start()
    {
        if (_health == null)
            Debug.LogError("HealthBar: _health не назначен!");

        _health.OnDamaged += UpdateBar;
        _health.OnHealed += UpdateBar;
        _health.OnDeath += HandleDeath;

        UpdateBar(1); 
    }

    private void OnDestroy()
    {
 
        _health.OnDamaged -= UpdateBar;
        _health.OnHealed -= UpdateBar;
        _health.OnDeath -= HandleDeath;
    }

    private void UpdateBar(float _)
    {
        float normalized = _health.CurrentHealth / _health.MaxHealth;
        Debug.Log("CurrentHealth: " + _health.CurrentHealth);
        _fillImage.fillAmount = Mathf.Clamp01(normalized);
    }

    private void HandleDeath()
    {
        UpdateBar(0);
        Debug.Log("Player died. Updating health bar.");
    }
}
