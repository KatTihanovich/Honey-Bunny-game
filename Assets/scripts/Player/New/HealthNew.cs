using UnityEngine;

public class HealthNew : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    public float MaxHealth => _maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0f;

    public System.Action OnDeath;
    public System.Action<float> OnDamaged;
    public System.Action<float> OnHealed;

    private void Awake()
    {
        CurrentHealth = _maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0f);
        OnDamaged?.Invoke(amount);
        Debug.Log(gameObject.name +" получил урон"+"| осталось: " + CurrentHealth);

        if (IsDead)
            OnDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f) return;

        float prevHealth = CurrentHealth;
        CurrentHealth = Mathf.Min(CurrentHealth + amount, _maxHealth);
        float actualHealed = CurrentHealth - prevHealth;

        if (actualHealed > 0f)
            OnHealed?.Invoke(actualHealed);
    }

    public void Kill()
    {
        if (!IsDead)
        {
            Debug.Log("Смерть");
            CurrentHealth = 0;
            OnDeath?.Invoke();
        }
    }

    public void RestoreFull()
    {
        if (!IsDead)
            CurrentHealth = _maxHealth;
    }

    public void SetMaxHealth(float newMax)
    {
        if (newMax <= 0f) return;
        _maxHealth = newMax;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, _maxHealth);
    }
}
