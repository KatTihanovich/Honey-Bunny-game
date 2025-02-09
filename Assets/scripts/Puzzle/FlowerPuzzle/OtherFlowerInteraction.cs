using UnityEngine;
using UnityEngine.Audio;

public class OtherFlowerInteraction : MonoBehaviour
{
    [SerializeField] private AudioClip otherFlowerMelody; 
    [SerializeField] private AudioMixerGroup otherFlowerAudioGroup; 

    private AudioSource otherFlowerAudioSource;

    private void Start()
    {
        otherFlowerAudioSource = gameObject.AddComponent<AudioSource>();
        otherFlowerAudioSource.clip = otherFlowerMelody;
        otherFlowerAudioSource.outputAudioMixerGroup = otherFlowerAudioGroup;
        otherFlowerAudioSource.loop = true;
        otherFlowerAudioSource.playOnAwake = false;
    }

    private void OnBecameVisible()
    {
        if (otherFlowerMelody != null && !otherFlowerAudioSource.isPlaying)
        {
            otherFlowerAudioSource.Play();
        }
    }

    private void OnBecameInvisible()
    {
        if (otherFlowerAudioSource.isPlaying)
        {
            otherFlowerAudioSource.Pause(); 
        }
    }
}
