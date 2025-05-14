using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Game.Audio;

public class Coin : MonoBehaviour
{
    private static readonly int PickUp = Animator.StringToHash("PickUp");
    [SerializeField] private int coinValue = 1;
    [SerializeField] private Transform collectionBarTransform; 
    [SerializeField] private Image collectionBarImage; 
    public Sprite defaultSprite; 
    public Sprite collectedSprite;
    [SerializeField] private float scaleUpAmount = 3f; 
    [SerializeField] private float rotationAmount = 60f; 
    [SerializeField] private float animationDuration = 0.4f;

    private Animator anim;
    private bool isCollected = false;

    private string _coinID;
    private ISoundManager _soundManager;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        _soundManager = SoundManagerNew.Instance;
    }

    private void Start()
    {
        _coinID = $"{gameObject.scene.name}_{transform.position}";

        if (CoinManager.Instance.IsCoinCollected(_coinID))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            Debug.Log("Collected!");
            anim.SetTrigger(PickUp);
            CoinManager.Instance.AddCoins(coinValue);
            CoinManager.Instance.MarkCoinCollected(_coinID);
            _soundManager.PlaySound("Star");
            StartCoroutine(AnimateCollectionEffect());
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