using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private HealthNew _health;

    [SerializeField] private CameraShake _cameraShake;
    [SerializeField] private VignetteController _vignetteController;
    [SerializeField] private PlayerController _playerController;

    private void Start()
    {
        if (_health == null)
            Debug.LogError("HealthBar: _health �� ��������!");

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

        if (normalized <= 0.7f)
        {
            _vignetteController.EnableVignette();
            if (normalized <= 0.5f)
            {
                _cameraShake.StartShaking();
                _vignetteController.VignetteTurnHarder();
                _playerController.SlowModeEnable();
            }
            else
            {
                _cameraShake.StopShaking();
                _vignetteController.VignetteTurnLighter();
                _playerController.SlowModeDesable();
            }
        }
        else
        {
            _vignetteController.DisableVignette();
        }

    }


    private void HandleDeath()
    {
        UpdateBar(0);
        Debug.Log("Player died. Updating health bar.");
    }
}
