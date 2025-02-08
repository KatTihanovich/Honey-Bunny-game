using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Coin : MonoBehaviour
{
    private static readonly int PickUp = Animator.StringToHash("PickUp");
    [SerializeField] private int coinValue = 1;
    [SerializeField] private AudioClip coinCollectSound;
    [SerializeField] private float volume = 1.0f;
    [SerializeField] private float pitchVariation = 0.1f;
    [SerializeField] private Transform collectionBarTransform; 
    [SerializeField] private Image collectionBarImage; 
    public Sprite defaultSprite; 
    public Sprite collectedSprite;
    [SerializeField] private float scaleUpAmount = 3f; 
    [SerializeField] private float rotationAmount = 60f; 
    [SerializeField] private float animationDuration = 0.4f;

    private Animator anim;
    private bool isCollected = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            Debug.Log("Collected!");
            anim.SetTrigger(PickUp);
            CoinManager.Instance.AddCoins(coinValue); 
            PlaySound();
            StartCoroutine(AnimateCollectionEffect());
        }
    }

    private void PlaySound()
    {
        if (coinCollectSound != null)
        {
            GameObject tempSoundObject = new GameObject("CoinSound");
            AudioSource audioSource = tempSoundObject.AddComponent<AudioSource>();
            
            audioSource.clip = coinCollectSound;
            audioSource.volume = volume;
            audioSource.pitch = 1.0f + Random.Range(-pitchVariation, pitchVariation);
            
            audioSource.Play();
            Destroy(tempSoundObject, coinCollectSound.length / audioSource.pitch);
        }
    }

    private IEnumerator AnimateCollectionEffect()
    {
        if (collectionBarTransform == null)
        {
            Debug.LogWarning("Collection Bar Image is not assigned!");
            yield break;
        }
        collectionBarImage.sprite = collectedSprite;

        Vector3 originalScale = collectionBarTransform.localScale;
        Quaternion originalRotation = collectionBarTransform.rotation;

        Vector3 targetScale = originalScale * scaleUpAmount;
        Quaternion targetRotation = Quaternion.Euler(0, 0, rotationAmount);

        float elapsedTime = 0f;

        // Scale Up & Rotate
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            collectionBarTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            collectionBarTransform.rotation = Quaternion.Lerp(originalRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset to original size & rotation
        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            collectionBarTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            collectionBarTransform.rotation = Quaternion.Lerp(targetRotation, originalRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        collectionBarTransform.localScale = originalScale;
        collectionBarTransform.rotation = originalRotation;
        collectionBarImage.sprite = defaultSprite;
    }

    private void KillIt()
    {
        Debug.Log("Destroy!");
        Destroy(gameObject);
    }
}