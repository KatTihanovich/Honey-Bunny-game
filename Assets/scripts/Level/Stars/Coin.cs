using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;
    [SerializeField] private AudioClip coinCollectSound;
    [SerializeField] private float volume = 1.0f;
    [SerializeField] private float pitchVariation = 0.1f;
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
            anim.SetTrigger("PickUp");
            CoinManager.Instance.AddCoins(coinValue); 
            PlaySound();
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
    private void KillIt()
    {
        Debug.Log("Destoy!");
        Destroy(gameObject);
    }
}
