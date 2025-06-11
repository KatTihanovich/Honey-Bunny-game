using UnityEngine;

public class Stress : MonoBehaviour
{
    private HealthNew _health;

    public float CurrentStress => _health != null ? _health.MaxHealth - _health.CurrentHealth : 0f;
    public float MaxStress => _health?.MaxHealth ?? 0f;

    public event System.Action<float> OnStressed;
    public event System.Action<float> OnStressReduced;
    public event System.Action OnMaxStressReached;

    [Header("Optional Visual / Gameplay Effects")]
    [SerializeField] private CameraShake _cameraShake;
    [SerializeField] private VignetteController _vignetteController;
    [SerializeField] private PlayerController _playerController;

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

    private void Start()
    {

        ApplyStressEffects();
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
        ApplyStressEffects();

        if (Mathf.Approximately(CurrentStress, MaxStress))
        {
            OnMaxStressReached?.Invoke();
        }
    }

    private void HandleHealed(float healed)
    {
        OnStressReduced?.Invoke(healed);
        ApplyStressEffects();
    }

    private void HandleDeath()
    {
        OnMaxStressReached?.Invoke();
    }

    private void ApplyStressEffects()
    {
        float normalized = CurrentStress / MaxStress;
        Debug.Log("CurrentStress: " + CurrentStress);

        if (normalized >= 0.3f)
        {
            _vignetteController?.EnableVignette();

            if (normalized >= 0.5f)
            {
                _cameraShake?.StartShaking();
                _vignetteController?.VignetteTurnHarder();
                _playerController?.SlowModeEnable();
            }
            else
            {
                _cameraShake?.StopShaking();
                _vignetteController?.VignetteTurnLighter();
                _playerController?.SlowModeDesable();
            }
        }
        else
        {
            _cameraShake?.StopShaking();
            _vignetteController?.DisableVignette();
            _playerController?.SlowModeDesable();
        }
    }
}
