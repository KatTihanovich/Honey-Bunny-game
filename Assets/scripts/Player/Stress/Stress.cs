using UnityEngine;

public class Stress : MonoBehaviour
{
    private HealthNew _health;

    public float CurrentStress => _health != null ? _health.MaxHealth - _health.CurrentHealth : 0f;
    public float MaxStress => _health?.MaxHealth ?? 0f;

    public event System.Action<float> OnStressed;
    public event System.Action<float> OnStressReduced;
    public event System.Action OnMaxStressReached;

    private void Awake()
    {
        _health = GetComponent<HealthNew>();

        if (_health == null)
        {
            Debug.LogError("Stress: компонент HealthNew не найден!");
            return;
        }

        _health.OnDamaged += HandleDamaged;
        _health.OnHealed += HandleHealed;
        _health.OnDeath += HandleDeath;
    }

    private void OnDestroy()
    {
        if (_health == null) return;

        _health.OnDamaged -= HandleDamaged;
        _health.OnHealed -= HandleHealed;
        _health.OnDeath -= HandleDeath;
    }

    private void HandleDamaged(float damage)
    {
        OnStressed?.Invoke(damage);

        if (Mathf.Approximately(CurrentStress, MaxStress))
        {
            OnMaxStressReached?.Invoke();
        }
    }

    private void HandleHealed(float healed)
    {
        OnStressReduced?.Invoke(healed);
    }

    private void HandleDeath()
    {
        OnMaxStressReached?.Invoke();
    }

   

}
