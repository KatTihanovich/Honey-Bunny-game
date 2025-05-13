using System.Collections;
using UnityEngine;

public class FadeOnTrigger : MonoBehaviour
{
    public float fadeDuration = 1f;       // Время на анимацию
    public float targetAlpha = 0.2f;      // Прозрачность, которую нужно достичь

    private SpriteRenderer sr;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeToAlpha(targetAlpha));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeToAlpha(1f));
        }
    }

    private IEnumerator FadeToAlpha(float alphaTarget)
    {
        float time = 0f;
        Color originalColor = sr.color;
        float startAlpha = sr.color.a;

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, alphaTarget, time / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        // Гарантируем финальный альфа
        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alphaTarget);
    }
}
