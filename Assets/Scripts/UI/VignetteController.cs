using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class VignetteController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume postProcessingVolume;

    [Header("Intensity Settings")]
    [SerializeField] private float lowHealthIntensityOn50 = 0.7f;
    [SerializeField] private float lowHealthIntensityOn70 = 0.6f;
    [SerializeField] private float transitionDuration = 1f;

    private Vignette _vignette;
    private Coroutine _currentTransition;

    private void Start()
    {
        if (postProcessingVolume.profile.TryGet(out Vignette vignette))
        {
            _vignette = vignette;
            _vignette.intensity.value = 0f;
        }
        else
        {
            Debug.LogError("VignetteController: No Vignette found in volume profile.");
        }
    }

    public void EnableVignette()
    {
        StartVignetteTransition(lowHealthIntensityOn70);
    }

    public void DisableVignette()
    {
        StartVignetteTransition(0f);
    }

    public void VignetteTurnHarder()
    {
        StartVignetteTransition(lowHealthIntensityOn50);
    }

    public void VignetteTurnLighter()
    {
        StartVignetteTransition(lowHealthIntensityOn70);
    }

    private void StartVignetteTransition(float targetIntensity)
    {
        if (_vignette == null) return;

        if (_currentTransition != null)
            StopCoroutine(_currentTransition);

        _currentTransition = StartCoroutine(TransitionVignetteIntensity(targetIntensity));
    }

    private IEnumerator TransitionVignetteIntensity(float targetIntensity)
    {
        float startIntensity = _vignette.intensity.value;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            _vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, t);
            yield return null;
        }

        _vignette.intensity.value = targetIntensity;
        _currentTransition = null;
    }
}