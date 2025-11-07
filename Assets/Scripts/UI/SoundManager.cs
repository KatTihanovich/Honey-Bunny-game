using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Mixer Groups")]
    public AudioMixerGroup MusicMixer;
    public AudioMixerGroup FXMixer;
    public AudioMixerGroup UIMixer;

    [Header("UI Sliders")]
    public Slider MusicSlider;
    public Slider FXSlider;
    public Slider UISlider;

    public float sliderChangeAmount = 0.05f;
    private bool isSliderSelected = false;

    private const string MIXER_MUSIC = "MusicVolume";
    private const string MIXER_FX = "FXVolume";
    private const string MIXER_UI = "UIVolume";

    private const string PREF_MUSIC = "musicVolume";
    private const string PREF_FX = "fxVolume";
    private const string PREF_UI = "uiVolume";

    private void Start()
    {
        MusicSlider.value = PlayerPrefs.HasKey(PREF_MUSIC) ? PlayerPrefs.GetFloat(PREF_MUSIC) : 1f;
        FXSlider.value = PlayerPrefs.HasKey(PREF_FX) ? PlayerPrefs.GetFloat(PREF_FX) : 1f;
        UISlider.value = PlayerPrefs.HasKey(PREF_UI) ? PlayerPrefs.GetFloat(PREF_UI) : 1f;

        ApplyVolumes();
    }

    private void Update()
    {
        if (isSliderSelected)
        {
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                AdjustSelectedSlider(sliderChangeAmount);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                AdjustSelectedSlider(-sliderChangeAmount);
            }
        }
    }

    private void AdjustSelectedSlider(float amount)
    {
        Slider activeSlider = EventSystem.current.currentSelectedGameObject?.GetComponent<Slider>();
        if (activeSlider != null)
        {
            activeSlider.value = Mathf.Clamp(activeSlider.value + amount, 0f, 1f);
            ApplyVolumes();
        }
    }

    public void OnSliderSelect()
    {
        isSliderSelected = true;
    }

    public void OnSliderDeselect()
    {
        isSliderSelected = false;
    }

    public void ApplyVolumes()
    {
        SetMixerVolume(MusicMixer.audioMixer, MIXER_MUSIC, MusicSlider.value);
        SetMixerVolume(FXMixer.audioMixer, MIXER_FX, FXSlider.value);
        SetMixerVolume(UIMixer.audioMixer, MIXER_UI, UISlider.value);

        PlayerPrefs.SetFloat(PREF_MUSIC, MusicSlider.value);
        PlayerPrefs.SetFloat(PREF_FX, FXSlider.value);
        PlayerPrefs.SetFloat(PREF_UI, UISlider.value);
    }

    // Called by MusicSlider OnValueChanged
    public void OnMusicSliderChanged()
    {
        float volume = MusicSlider.value;
        SetMixerVolume(MusicMixer.audioMixer, MIXER_MUSIC, volume);
        PlayerPrefs.SetFloat(PREF_MUSIC, volume);
    }

    // Called by FXSlider OnValueChanged
    public void OnFXSliderChanged()
    {
        float volume = FXSlider.value;
        SetMixerVolume(FXMixer.audioMixer, MIXER_FX, volume);
        PlayerPrefs.SetFloat(PREF_FX, volume);
    }

    // Called by UISlider OnValueChanged
    public void OnUISliderChanged()
    {
        float volume = UISlider.value;
        SetMixerVolume(UIMixer.audioMixer, MIXER_UI, volume);
        PlayerPrefs.SetFloat(PREF_UI, volume);
    }
    private void SetMixerVolume(AudioMixer mixer, string parameterName, float value)
    {
        mixer.SetFloat(parameterName, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }

    public void LoadValues()
    {
        MusicSlider.value = PlayerPrefs.GetFloat(PREF_MUSIC);
        FXSlider.value = PlayerPrefs.GetFloat(PREF_FX);
        UISlider.value = PlayerPrefs.GetFloat(PREF_UI);
        ApplyVolumes();
    }
}