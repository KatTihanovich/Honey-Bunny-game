using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SequenceChecker : MonoBehaviour
{
    [SerializeField] private List<string> correctSequence = new List<string>();
    [SerializeField] private GameObject objectToHide;
    [SerializeField] private Animator objectAnimator;
    [SerializeField] private string animationTrigger = "Open";
    [SerializeField] private List<InteractionZone> interactableZones;

    [Header("Audio Settings")]
    [SerializeField] private AudioMixerGroup audioMixerGroup;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private float volume = 1.0f;

    private int currentStep = 0;
    private float animationDuration = 2f;

    private void OnEnable() => ResetPuzzle();

    public bool ValidateStep(string name)
    {
        if (currentStep >= correctSequence.Count || correctSequence[currentStep] != name)
        {
            Debug.Log($"Wrong input: {name}. Resetting.");
            ResetPuzzle();
            return false;
        }

        currentStep++;
        return true;
    }

    public bool IsSequenceComplete() => currentStep >= correctSequence.Count;

    public void OnPuzzleCompleted()
    {
        Debug.Log("Puzzle completed!");
        PlaySound(winSound);

        if (objectAnimator != null)
        {
            objectAnimator.SetTrigger(animationTrigger);
            Invoke(nameof(HideObject), animationDuration);
        }
    }

    private void HideObject()
    {
        if (objectToHide != null)
            objectToHide.SetActive(false);
    }

    public void ResetPuzzle()
    {
        InteractionZone.ResetInteractionSequence();
        currentStep = 0;

        if (objectToHide != null)
            objectToHide.SetActive(true);

        foreach (var zone in interactableZones)
        {
            if (zone.columnAnimator != null)
                zone.columnAnimator.SetBool("IsOpened", false);
            if (zone.symbolAnimator != null)
                zone.symbolAnimator.SetBool("IsLighted", false);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioMixerGroup != null)
        {
            GameObject tempAudio = new GameObject("TempAudioSource");
            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.volume = volume;
            audioSource.Play();

            Destroy(tempAudio, clip.length);
        }
    }
}
