using UnityEngine;

namespace Level.Stick
{
    public class StickCollectable : MonoBehaviour
    {
        private static readonly int PickUpTrigger = Animator.StringToHash("PickUp");
        
        private GameObject receiver;
        
        public Animator animator;
        private bool isCollected;
        
        [SerializeField] private AudioClip coinCollectSound;
        [SerializeField] private float volume = 1.0f;
        [SerializeField] private float pitchVariation = 0.1f;

        void Start()
        {
            receiver = GameObject.Find("Bunny");
            if (receiver == null)
            {
                Debug.LogError("[STICK] Bunny not found!!!");
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !isCollected)
            {
                isCollected = true;
                Debug.Log("Игрок подобрал палку");
                animator.SetTrigger(PickUpTrigger);
                receiver?.SendMessage("OnStickCollected", SendMessageOptions.DontRequireReceiver);
                PlaySound();
                Destroy(gameObject, coinCollectSound ? coinCollectSound.length : 2.0f);
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
    }
}
