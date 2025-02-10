using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class OtherFlowerInteraction : MonoBehaviour
{
    [SerializeField] private AudioClip otherFlowerMelody;
    [SerializeField] private AudioMixerGroup otherFlowerAudioGroup;
    [SerializeField] private float fadeDuration = 1.0f; 

    private AudioSource otherFlowerAudioSource;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        otherFlowerAudioSource = gameObject.AddComponent<AudioSource>();
        otherFlowerAudioSource.clip = otherFlowerMelody;
        otherFlowerAudioSource.outputAudioMixerGroup = otherFlowerAudioGroup;
        otherFlowerAudioSource.loop = true;
        otherFlowerAudioSource.playOnAwake = false;
        otherFlowerAudioSource.volume = 0f; 
    }

    private void OnBecameVisible()
    {
        if (otherFlowerMelody != null)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeIn());
        }
    }

    private void OnBecameInvisible()
    {
        if (otherFlowerAudioSource.isPlaying)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeIn()
    {
        if (!otherFlowerAudioSource.isPlaying)
        {
            otherFlowerAudioSource.Play();
        }

        float startVolume = otherFlowerAudioSource.volume;
        float targetVolume = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            otherFlowerAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        otherFlowerAudioSource.volume = targetVolume;
    }

    private IEnumerator FadeOut()
    {
        float startVolume = otherFlowerAudioSource.volume;
        float targetVolume = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            otherFlowerAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        otherFlowerAudioSource.volume = targetVolume;
        otherFlowerAudioSource.Pause(); 
}
}