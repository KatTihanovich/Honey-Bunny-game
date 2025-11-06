using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource UISource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] backgroundMusic;
    [SerializeField] private AudioClip UISound;

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
    
    private void PlayUISound()
    {
        UISource.clip = UISound;
        UISource.Play();
    }

    public void SoundControl()
    {
        AudioListener.pause = !AudioListener.pause;
    }
}
