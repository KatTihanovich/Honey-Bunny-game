using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class SoundManager : MonoBehaviour
{
    public AudioMixerGroup Mixer;
    public Slider MusicSlider;

    public float sliderChangeAmount = 0.05f;
    private bool isSliderSelected = false; 

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadValue();
        }
        else
        {
            changeVolume();
        }

        // Make sure the slider can be selected and navigated to
        MusicSlider.Select();
    }

    private void Update()
    {
        if (isSliderSelected)
        {
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                MusicSlider.value = Mathf.Min(MusicSlider.value + sliderChangeAmount, 1f);
                changeVolume();
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                MusicSlider.value = Mathf.Max(MusicSlider.value - sliderChangeAmount, 0f);
                changeVolume();
            }
        }
    }

    public void OnSliderSelect()
    {
        isSliderSelected = true;
    }

    // This method will be called when the user stops interacting with the slider (e.g., mouse out or press "Tab" to navigate away)
    public void OnSliderDeselect()
    {
        isSliderSelected = false;
    }

    public void changeVolume()
    {
        float volume = MusicSlider.value;
        Mixer.audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume); 
    }

    public void LoadValue()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        changeVolume();
    }
}
