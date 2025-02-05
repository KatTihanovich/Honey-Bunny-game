using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UIButtonSound : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private float volume = 1.0f;
    [SerializeField] private AudioMixerGroup audioMixerGroup; 

    public void PlayClickSound()
    {
        if (buttonSound != null && audioMixerGroup != null)
        {
            GameObject tempAudio = new GameObject("TempButtonSound");
            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();

            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.clip = buttonSound;
            audioSource.volume = volume;
            audioSource.Play();

            Destroy(tempAudio, buttonSound.length);
        }
    }


}
