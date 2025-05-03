using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteController : MonoBehaviour
{
    [SerializeField] private Volume postProcessingVolume;
    [SerializeField] private float lowHealthIntensity = 0.7f;

    private Vignette _vignette;

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
        if (_vignette != null)
            _vignette.intensity.value = lowHealthIntensity;
    }

    public void DisableVignette()
    {
        if (_vignette != null)
            _vignette.intensity.value = 0f;
    }
}
