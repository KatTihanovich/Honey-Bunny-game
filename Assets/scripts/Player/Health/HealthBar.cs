using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private Stress _stress;

    [SerializeField] private CameraShake _cameraShake;
    [SerializeField] private VignetteController _vignetteController;
    [SerializeField] private PlayerController _playerController;

    private void Start()
    {
        if (_stress == null)
            Debug.LogError("StressBar: _stress не назначен!");

        _stress.OnStressed += UpdateBar;
        _stress.OnStressReduced += UpdateBar;
        _stress.OnMaxStressReached += HandleMaxStress;

        UpdateBar(0);
    }

    private void OnDestroy()
    {
        _stress.OnStressed -= UpdateBar;
        _stress.OnStressReduced -= UpdateBar;
        _stress.OnMaxStressReached -= HandleMaxStress;
    }

    private void UpdateBar(float _)
    {
        float normalized = _stress.CurrentStress / _stress.MaxStress;
        Debug.Log("CurrentStress: " + _stress.CurrentStress);
        _fillImage.fillAmount = Mathf.Clamp01(normalized);

        if (normalized >= 0.3f)
        {
            _vignetteController.EnableVignette();

            if (normalized >= 0.5f)
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
            _cameraShake.StopShaking();
            _playerController.SlowModeDesable();
            _vignetteController.DisableVignette();
        }
    }

    private void HandleMaxStress()
    {
        UpdateBar(0);
        Debug.Log("Игрок перегружен стрессом!");
    }
}
