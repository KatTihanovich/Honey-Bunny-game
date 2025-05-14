// ����: SoundManagerNew.cs
using UnityEngine;
using System.Collections.Generic;

namespace Game.Audio
{
    [DisallowMultipleComponent]
    public class SoundManagerNew : MonoBehaviour, ISoundManager
    {
        [SerializeField] private AudioSource _audioSourcePrefab;
        [SerializeField] private SoundData _soundData;

        private Queue<AudioSource> _audioSourcePool;
        private const int InitialPoolSize = 5;

        private static SoundManagerNew _instance;

        public static SoundManagerNew Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SoundManagerNew>();
                    if (_instance == null)
                    {
                        _instance = new GameObject("SoundManagerNew").AddComponent<SoundManagerNew>();
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }

        private void Initialize()
        {
            if (_soundData == null)
            {
                Debug.LogError("SoundData is not assigned in SoundManagerNew!");
            }

            _audioSourcePool = new Queue<AudioSource>(InitialPoolSize);
            for (int i = 0; i < InitialPoolSize; i++)
            {
                AddAudioSourceToPool();
            }
        }

        public void PlaySound(string soundName)
        {
            if (_soundData == null || !_soundData.TryGetSound(soundName, out AudioClip clip, out float volume, out float pitch))
            {
                Debug.LogWarning($"No sound found for {soundName}");
                return;
            }

            AudioSource source = GetAudioSource();
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
            StartCoroutine(ReturnToPoolAfterPlay(source, clip.length));
        }

        private AudioSource GetAudioSource()
        {
            if (_audioSourcePool.Count > 0)
            {
                return _audioSourcePool.Dequeue();
            }
            return AddAudioSourceToPool();
        }

        private AudioSource AddAudioSourceToPool()
        {
            AudioSource source = Instantiate(_audioSourcePrefab, Vector3.zero, Quaternion.identity, transform);
            source.playOnAwake = false;
            return source;
        }

        private System.Collections.IEnumerator ReturnToPoolAfterPlay(AudioSource source, float duration)
        {
            yield return new WaitForSeconds(duration);
            source.Stop();
            source.clip = null;
            _audioSourcePool.Enqueue(source);
        }
        public AudioSource PlaySound(string soundName, bool loop)
        {
            if (_soundData == null || !_soundData.TryGetSound(soundName, out AudioClip clip, out float volume, out float pitch))
            {
                Debug.LogWarning($"No sound found for {soundName}");
                return null;
            }

            AudioSource source = GetAudioSource();
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = loop;

            source.Play();

            if (!loop)
            {
                StartCoroutine(ReturnToPoolAfterPlay(source, clip.length));
            }

            return source;
        }

        public void StopSound(AudioSource source)
        {
            if (source == null) return;

            source.Stop();
            source.clip = null;
            source.loop = false;

            if (!_audioSourcePool.Contains(source))
            {
                _audioSourcePool.Enqueue(source);
            }
        }

    }
}