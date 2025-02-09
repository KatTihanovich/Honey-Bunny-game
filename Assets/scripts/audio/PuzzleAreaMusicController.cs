using UnityEngine;
using UnityEngine.Audio;

public class PuzzleAreaMusicController : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private float reducedVolume = -20f; 
    private float defaultVolume = 0f; 

    private int playerInsideCount = 0; 

    private void Start()
    {
        if (musicMixerGroup != null)
        {
            // Get the volume from the Audio Mixer using the parameter name (assumed "MusicVolume")
            musicMixerGroup.audioMixer.GetFloat("MusicVolume", out defaultVolume);
        }
        else
        {
            Debug.LogError("No Music AudioMixerGroup assigned in PuzzleAreaMusicController!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideCount++;

            if (playerInsideCount == 1) 
            {
                LowerMusicVolume();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideCount = Mathf.Max(0, playerInsideCount - 1);

            if (playerInsideCount == 0)
            {
                RestoreMusicVolume();
            }
        }
    }

    private void LowerMusicVolume()
    {
        musicMixerGroup.audioMixer.SetFloat("MusicVolume", reducedVolume);
    }

    private void RestoreMusicVolume()
    {
        musicMixerGroup.audioMixer.SetFloat("MusicVolume", defaultVolume);
    }
}
