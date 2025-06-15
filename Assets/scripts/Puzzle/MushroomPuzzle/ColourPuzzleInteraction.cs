using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class InteractionZone : MonoBehaviour
{
    private bool playerInZone = false;
    private static List<string> interactionSequence = new List<string>();

    [SerializeField] private SequenceChecker sequenceChecker;
    private Animator anim;

    [SerializeField] private AudioClip interactionSound;
    [SerializeField] private AudioMixerGroup audioMixerGroup;
    [SerializeField] private float volume = 1.0f;

    private bool isAnimating = false;

    [Header("External Animations")]
    [SerializeField] public Animator columnAnimator;   // ✅ будет виден в инспекторе
    [SerializeField] public Animator symbolAnimator;   // ✅ будет виден в инспекторе

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (!playerInZone || isAnimating) return;
        StartCoroutine(HandleInteraction());
    }

    private IEnumerator HandleInteraction()
    {
        isAnimating = true;

        anim.SetTrigger("Klick");
        PlayInteractionSound();

        yield return new WaitForSeconds(0.75f); // Ждём анимацию

        bool isCorrect = sequenceChecker.ValidateStep(gameObject.name);

        if (isCorrect)
        {
            interactionSequence.Add(gameObject.name);
            LightUpSymbol();
            OpenColumn();

            if (sequenceChecker.IsSequenceComplete())
            {
                sequenceChecker.OnPuzzleCompleted();
            }
        }
        else
        {
            ResetInteractionSequence();
        }

        isAnimating = false;
    }

    private void LightUpSymbol()
    {
        if (symbolAnimator != null)
            symbolAnimator.SetBool("IsLighted", true);
    }

    private void OpenColumn()
    {
        if (columnAnimator != null)
            columnAnimator.SetBool("IsOpened", true);
    }

    private void PlayInteractionSound()
    {
        if (interactionSound != null && audioMixerGroup != null)
        {
            GameObject tempAudio = new GameObject("TempAudioClip");
            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();

            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.clip = interactionSound;
            audioSource.volume = volume;
            audioSource.Play();

            Destroy(tempAudio, interactionSound.length);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInZone = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInZone = false;
    }

    public bool IsPlayerInZone()
    {
        return playerInZone;
    }

    public static void ResetInteractionSequence()
    {
        interactionSequence.Clear();
        Debug.Log("Interaction sequence reset.");
    }

    public static List<string> GetInteractionSequence() => new List<string>(interactionSequence);
}
