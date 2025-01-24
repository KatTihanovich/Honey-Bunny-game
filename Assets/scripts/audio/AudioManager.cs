using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource FXSource;
    [SerializeField] AudioSource UISource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip background;
    [SerializeField] public AudioClip death;
    [SerializeField] public AudioClip checkpoint;
    [SerializeField] public AudioClip ui;
    [SerializeField] public AudioClip nipperAttack;
    [SerializeField] public AudioClip nickerAttack;
    [SerializeField] public AudioClip playerAttack;
    [SerializeField] public AudioClip jump;
    [SerializeField] public AudioClip destroyPlatform;
    [SerializeField] public AudioClip damage;

      [Header("Volume Settings")]
    public SoundManager soundManager;

    public static AudioManager instance; 

    private void Awake(){
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }
    private void Start(){
        if (PlayerPrefs.HasKey("musicVolume")){
            soundManager.LoadValue();
        }
        MusicSource.clip = background;
        MusicSource.Play();
    }

    public void PlaySFX(AudioClip clip){
        FXSource.PlayOneShot(clip);
    }

    public void PlayUI(){
        UISource.PlayOneShot(ui);
    }

    public void SoundControl(){
        if(AudioListener.pause == true){
            AudioListener.pause = false;
        } else {
            AudioListener.pause = true;
        }
    }
}
