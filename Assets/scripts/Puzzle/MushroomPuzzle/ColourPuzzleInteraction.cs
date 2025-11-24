using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class InteractionZone : MonoBehaviour
{
    private bool playerInZone = false;
    private static List<string> interactionSequence = new List<string>();
    private static bool puzzleSolved = false; // ✅ защита от сброса после победы
    private static string lastInteractedObjectName = ""; // ✅ защита от спама одного и того же гриба

    [SerializeField] private SequenceChecker sequenceChecker;
    private Animator anim;

    [SerializeField] private AudioClip interactionSound;
    [SerializeField] private AudioMixerGroup audioMixerGroup;
    [SerializeField] private float volume = 1.0f;

    private bool isAnimating = false;

    [Header("External Animations")]
    [SerializeField] public Animator columnAnimator;
    [SerializeField] public Animator symbolAnimator;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (!playerInZone || isAnimating || puzzleSolved) return;

        // Защита от спама: если игрок нажал тот же объект, что и последний, ничего не делаем
        if (gameObject.name == lastInteractedObjectName) return;

        StartCoroutine(HandleInteraction());
    }

    private IEnumerator HandleInteraction()
    {
        isAnimating = true;

        anim.SetTrigger("Klick");
        PlayInteractionSound();

        yield return new WaitForSeconds(0.75f);

        bool isCorrect = sequenceChecker.ValidateStep(gameObject.name);

        if (isCorrect)
        {
            interactionSequence.Add(gameObject.name);
            lastInteractedObjectName = gameObject.name;

            LightUpSymbol();
            OpenColumn();

            if (sequenceChecker.IsSequenceComplete())
            {
                puzzleSolved = true;
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

    public bool IsPlayerInZone() => playerInZone;

    public static void ResetInteractionSequence()
    {
        interactionSequence.Clear();
        puzzleSolved = false; // ✅ разрешаем повтор после перезапуска
        lastInteractedObjectName = ""; // сброс последнего объекта
        Debug.Log("Interaction sequence reset.");
    }

    public static List<string> GetInteractionSequence() => new List<string>(interactionSequence);

    public void TriggerWinAnimation()
    {
        anim.ResetTrigger("Klick");
        anim.SetTrigger("Win");

        if (symbolAnimator != null)
            symbolAnimator.SetBool("IsLighted", true);
        if (columnAnimator != null)
            columnAnimator.SetBool("IsOpened", true);
    }
}
