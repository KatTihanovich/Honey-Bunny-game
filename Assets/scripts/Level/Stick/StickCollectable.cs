using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField] private GameObject notificationPanel;

        void Start()
        {
            receiver = GameObject.Find("Bunny");
            if (receiver == null)
            {
                Debug.LogError("[STICK] Bunny not found!!!");
            }

            if (notificationPanel != null)
            {
                notificationPanel.SetActive(false);
            }
            else
            {
                Debug.LogError("Notification panel is not assigned in StickCollectable!");
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
                
                // 🎯 Show notification
                if (notificationPanel != null)
                {
                    notificationPanel.SetActive(true);
                }
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

        public void CloseNotification()
        {
            if (notificationPanel != null)
            {
                notificationPanel.SetActive(false);
            }
        }
    }
}
