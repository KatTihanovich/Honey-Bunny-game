using UnityEngine;

public class HealthNew : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
    private float _startHealth = 100f;

    [SerializeField, Range(0f, 100f)]
    private float _maxHealth = 100f;
    public float MaxHealth => _maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0f;

    public System.Action OnDeath;
    public event System.Action OnDamageTaken;
    public System.Action<float> OnDamaged;
    public System.Action<float> OnHealed;

    private void Awake()
    {
        CurrentHealth = _startHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Max(CurrentHealth, 0f);
        OnDamaged?.Invoke(amount);
        OnDamageTaken?.Invoke();
       

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
       
            CurrentHealth = 0;
            OnDeath?.Invoke();
        }
    }

    public void RestoreFull()
    {
        
            float actualHealed = _maxHealth - CurrentHealth;
            CurrentHealth = _maxHealth;
            OnHealed?.Invoke(actualHealed);
        
        
    }

    public void SetMaxHealth(float newMax)
    {
        if (newMax <= 0f) return;
        _maxHealth = newMax;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, _maxHealth);
    }
}
