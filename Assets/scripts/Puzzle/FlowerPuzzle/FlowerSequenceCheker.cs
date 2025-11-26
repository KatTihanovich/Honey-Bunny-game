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

    public Transform focusObject; 
    public FocusSettingsForObject focusSettings;

    private void OnEnable()
    {
        ResetPuzzle(); 
    }

    public bool CheckSequence(FlowerInteraction triggeringObject)
{
    List<string> playerSequence = FlowerInteraction.GetInteractionSequence();
    Debug.Log("Current Player Sequence: " + string.Join(", ", playerSequence));
    Debug.Log("Correct Sequence: " + string.Join(", ", correctSequence));

    // Проверка префикса — сравниваем только введённую часть
    for (int i = 0; i < playerSequence.Count; i++)
    {
        if (i >= correctSequence.Count || playerSequence[i] != correctSequence[i])
        {
            // Debug.Log("Incorrect input at step " + i + ". Resetting.");
            // PlaySound(loseClip, audioGroup);

            // // Проигрываем анимации неудачи
            // foreach (var zone in flowerZones)
            //     zone.TriggerLoseAnimation();

            FlowerInteraction.ResetInteractionSequence();
            return false;
        }
    }

    // Если последовательность пока корректна, но ещё не закончена
    if (playerSequence.Count < correctSequence.Count)
    {
        Debug.Log("So far so good. Continue inputting the sequence.");
        return false;
    }

    // Если последовательность полностью совпала
    Debug.Log("Puzzle solved!");
    PlaySound(winClip, audioGroup);
    EndWindow.puzzlesSolved++;

    foreach (var zone in flowerZones)
        zone.TriggerWinAnimation();

    if (objectToDeActivate != null)
    {
        Animator sleepingFlowerAnimator = objectToDeActivate.GetComponent<Animator>();
        if (sleepingFlowerAnimator != null)
        {
            sleepingFlowerAnimator.SetTrigger("Awake");
            Debug.Log("Sleeping flower awakened!");
            
            CameraFocus cam = Camera.main.GetComponent<CameraFocus>();
            StartCoroutine(cam.FocusOnObject(focusObject, focusSettings));
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

    return true;
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
