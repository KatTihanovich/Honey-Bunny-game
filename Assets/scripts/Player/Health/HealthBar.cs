using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private Stress _stress;

    private void Start()
    {
        if (_stress == null)
        {
            Debug.LogError("HealthBar: _stress не назначен!");
            return;
        }

        _stress.OnStressed += UpdateBar;
        _stress.OnStressReduced += UpdateBar;
        _stress.OnMaxStressReached += HandleMaxStress;

        UpdateBar(0);
    }

    private void OnDestroy()
    {
        if (_stress == null) return;

        _stress.OnStressed -= UpdateBar;
        _stress.OnStressReduced -= UpdateBar;
        _stress.OnMaxStressReached -= HandleMaxStress;
    }

    private void UpdateBar(float _)
    {
        float normalized = _stress.CurrentStress / _stress.MaxStress;
        _fillImage.fillAmount = Mathf.Clamp01(normalized);
    }

    private void HandleMaxStress()
    {
        UpdateBar(0);
        Debug.Log("Игрок перегружен стрессом!");
    }
}
