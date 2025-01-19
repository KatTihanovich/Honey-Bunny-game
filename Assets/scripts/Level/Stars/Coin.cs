using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1; 
    [SerializeField] private AudioClip coinCollectSound;
    [SerializeField] private float volume = 1.0f; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            CoinManager.Instance.AddCoins(coinValue);
            PlaySound();
            Destroy(gameObject);
        }
    }

    private void PlaySound()
    {
        if (coinCollectSound != null)
        {
            AudioSource.PlayClipAtPoint(coinCollectSound, transform.position, volume);
        }
    }
}
