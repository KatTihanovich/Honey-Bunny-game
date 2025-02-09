using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SequenceChecker : MonoBehaviour
{
    [SerializeField] private List<string> correctSequence = new List<string>(); 
    [SerializeField] private GameObject objectToHide; 
    [SerializeField] private Animator objectAnimator; 
    [SerializeField] private string animationTrigger = "Open";
    [SerializeField] private List<InteractionZone> interactableZones = new List<InteractionZone>(); 

    [Header("Audio Settings")]
    [SerializeField] private AudioMixerGroup audioMixerGroup; // 🎵 Mixer Group
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private float volume = 1.0f;

    private float animationDuration = 2f; 

    private void OnEnable()
    {
        ResetPuzzle(); 
    }

    public bool CheckSequence(InteractionZone triggeringObject)
    {
        List<string> playerSequence = InteractionZone.GetInteractionSequence();

        Debug.Log("Current Player Sequence: " + string.Join(", ", playerSequence));

        if (playerSequence.Count < correctSequence.Count)
        {
            Debug.Log("Sequence is incomplete. Keep interacting.");
            return false;
        }

        bool isCorrect = true;
        for (int i = 0; i < correctSequence.Count; i++)
        {
            if (playerSequence[i] != correctSequence[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            Debug.Log("Puzzle solved!");
            PlaySound(winSound);  // 🎵 Play Win Sound

            foreach (var zone in interactableZones)
                zone.TriggerWinAnimation();

            if (objectAnimator != null)
            {
                objectAnimator.SetTrigger(animationTrigger);
                Invoke(nameof(HideObject), animationDuration);
            }
            else
            {
                Debug.LogWarning("No object assigned to animate.");
            }
        }
        else
        {
            Debug.Log("Incorrect sequence! Resetting.");
            PlaySound(loseSound); // 🎵 Play Lose Sound

            foreach (var zone in interactableZones)
                zone.TriggerLoseAnimation();

            InteractionZone.ResetInteractionSequence();
        }

        return isCorrect;
    }

    private void HideObject()
    {
        if (objectToHide != null)
            objectToHide.SetActive(false);
    }

    public void ResetPuzzle()
    {
        InteractionZone.ResetInteractionSequence();
        Debug.Log("Puzzle sequence reset on restart.");

        if (objectToHide != null)
            objectToHide.SetActive(true);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioMixerGroup != null)
        {
            GameObject tempAudio = new GameObject("TempAudioSource");
            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.outputAudioMixerGroup = audioMixerGroup; // 🎛️ Route to Mixer
            audioSource.volume = volume;
            audioSource.Play();

            Destroy(tempAudio, clip.length);
        }
        else
        {
            Debug.LogWarning("AudioClip or AudioMixerGroup is missing!");
        }
    }
}
