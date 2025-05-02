using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class OtherFlowerInteraction : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip otherFlowerMelody;
    [SerializeField] private AudioMixerGroup otherFlowerAudioGroup;
    [SerializeField] private float fadeDuration = 1.0f;

    [Header("Visual Elements")]
    [SerializeField] private GameObject[] visualObjects; // 4 картинки

    [Header("Trigger Settings")]
    [SerializeField] private Collider2D triggerZone; // Зона с триггером

    private AudioSource otherFlowerAudioSource;
    private Coroutine fadeCoroutine;
    private Coroutine visualCoroutine;
    private bool isPlayerInside = false;

    private void Start()
    {
        otherFlowerAudioSource = gameObject.AddComponent<AudioSource>();
        otherFlowerAudioSource.clip = otherFlowerMelody;
        otherFlowerAudioSource.outputAudioMixerGroup = otherFlowerAudioGroup;
        otherFlowerAudioSource.loop = true;
        otherFlowerAudioSource.playOnAwake = false;
        otherFlowerAudioSource.volume = 0f;

        // Деактивируем картинки при старте
        foreach (var obj in visualObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // Убедимся, что коллайдер - триггер
        if (triggerZone != null)
        {
            triggerZone.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Entered trigger with: {other.name}");
        if (other.CompareTag("Player") && !isPlayerInside)
        {
            isPlayerInside = true;

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeIn());

            if (visualCoroutine != null)
                StopCoroutine(visualCoroutine);
            visualCoroutine = StartCoroutine(PlayVisualSequence());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPlayerInside)
        {
            isPlayerInside = false;

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOut());

            if (visualCoroutine != null)
                StopCoroutine(visualCoroutine);

            // Выключаем картинки
            foreach (var obj in visualObjects)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }

    private IEnumerator FadeIn()
    {
        if (!otherFlowerAudioSource.isPlaying)
            otherFlowerAudioSource.Play();

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

    private IEnumerator PlayVisualSequence()
{
    int index = 0;
    while (isPlayerInside)
    {
        for (int i = 0; i < visualObjects.Length; i++)
        {
            if (visualObjects[i] != null)
                visualObjects[i].SetActive(i == index);
        }

        index++;

        if (index >= visualObjects.Length)
        {
            index = 0;

            // Все 4 ноты показаны — делаем паузу
            yield return new WaitForSeconds(2f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
}
}
