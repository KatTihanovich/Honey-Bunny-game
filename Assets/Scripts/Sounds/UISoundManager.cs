// UISoundManager.cs
using UnityEngine;

namespace Game.Audio
{
    [DisallowMultipleComponent]
    public class UISoundManager : MonoBehaviour
    {
        private AudioSource _audioSource;

        private static UISoundManager _instance;
        public static UISoundManager Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            //DontDestroyOnLoad(gameObject);

            // Auto-find or add the AudioSource
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
            }
        }

        public void PlaySound()
        {
            if (_audioSource.clip == null)
            {
                Debug.LogWarning($"[{nameof(UISoundManager)}] No AudioClip assigned to the AudioSource!");
                return;
            }

            _audioSource.Stop(); 
            _audioSource.Play();
        }

        public void StopSound()
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
        }
    }
}