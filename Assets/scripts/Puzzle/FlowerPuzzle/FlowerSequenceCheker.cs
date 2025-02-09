using UnityEngine;
using UnityEngine.Audio; 
using System.Collections.Generic;

public class FlowerSequenceChecker : MonoBehaviour
{
    [SerializeField] private List<string> correctSequence = new List<string>(); 
    [SerializeField] private GameObject objectToDeActivate; 
    [SerializeField] private List<FlowerInteraction> flowerZones = new List<FlowerInteraction>(); 

    [SerializeField] private AudioMixerGroup audioGroup; 
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip;

    private void OnEnable()
    {
        ResetPuzzle(); 
    }

    public bool CheckSequence(FlowerInteraction triggeringObject)
    {
        List<string> playerSequence = FlowerInteraction.GetInteractionSequence();
        Debug.Log("Current Player Sequence: " + string.Join(", ", playerSequence));
        Debug.Log("Correct Sequence: " + string.Join(", ", correctSequence));

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

        // Play animations based on correctness
        foreach (var zone in flowerZones)
        {
            if (isCorrect)
                zone.TriggerWinAnimation();
            else
                zone.TriggerLoseAnimation();
        }

        // Additional actions if the sequence is correct
        if (isCorrect)
        {
            Debug.Log("Puzzle solved!");
            PlaySound(winClip, audioGroup);

            if (objectToDeActivate != null)
            {
                Animator sleepingFlowerAnimator = objectToDeActivate.GetComponent<Animator>();
                if (sleepingFlowerAnimator != null)
                {
                    sleepingFlowerAnimator.SetTrigger("Awake"); 
                    Debug.Log("Sleeping flower awakened!");
                }
                else
                {
                    Debug.LogWarning("Animator not found on ObjectToDeActivate.");
                }
                objectToDeActivate.GetComponent<BoxCollider2D>().isTrigger = true;
            }
            else
            {
                Debug.LogWarning("No object assigned to deactivate.");
            }
        }
        else
        {
            Debug.Log("Incorrect sequence! Resetting.");
            PlaySound(loseClip, audioGroup);
            FlowerInteraction.ResetInteractionSequence();
        }

        return isCorrect;
    }

    private void PlaySound(AudioClip clip, AudioMixerGroup audioGroup)
    {
        if (clip == null || audioGroup == null)
        {
            Debug.LogWarning("Missing AudioClip or AudioMixerGroup!");
            return;
        }

        GameObject tempAudioSource = new GameObject("TempAudioSource");
        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();

        audioSource.outputAudioMixerGroup = audioGroup;
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(tempAudioSource, clip.length);
    }

    public void LogPlayerSequence()
    {
        List<string> playerSequence = FlowerInteraction.GetInteractionSequence();
        Debug.Log("Player Sequence: " + string.Join(", ", playerSequence));
    }

    public void ResetPuzzle()
    {
        FlowerInteraction.ResetInteractionSequence(); // Clears the interaction history
        Debug.Log("Puzzle sequence reset on restart.");
    
        if (objectToDeActivate != null)
            objectToDeActivate.GetComponent<BoxCollider2D>().isTrigger = false; 
    }
}
