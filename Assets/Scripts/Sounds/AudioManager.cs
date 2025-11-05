using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource MusicSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] backgroundMusic;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBackgroundMusic(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBackgroundMusic(scene.buildIndex);
    }

    private void PlayBackgroundMusic(int sceneIndex)
    {
        AudioClip newClip = backgroundMusic[sceneIndex];

        if (MusicSource.clip != newClip) 
        {
            MusicSource.clip = newClip;
            MusicSource.Play();
        }
    }

    public void SoundControl()
    {
        AudioListener.pause = !AudioListener.pause;
    }
}
