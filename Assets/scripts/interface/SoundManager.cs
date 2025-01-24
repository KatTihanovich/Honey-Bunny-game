using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioMixerGroup Mixer;
    public Slider MusicSlider;

    private void Start(){
        if (PlayerPrefs.HasKey("musicVolume")){
            LoadValue();
        }
        else 
        {
            changeVolume();
        }
    }
    public void changeVolume(){
        float volume = MusicSlider.value;
        Mixer.audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void LoadValue(){
        MusicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        changeVolume();
    }
}
