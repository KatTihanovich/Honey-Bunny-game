using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
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

    private void Awake(){
        DontDestroyOnLoad(gameObject);
    }
    private void Start(){
        musicSource.clip = background;
        musicSource.Play();
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
