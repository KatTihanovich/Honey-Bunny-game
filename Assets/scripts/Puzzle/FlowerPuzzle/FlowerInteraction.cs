using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.Collections;

public class FlowerInteraction : MonoBehaviour
{
    private bool playerInZone = false;
    private static List<string> interactionSequence = new List<string>();

    [SerializeField] private FlowerSequenceChecker sequenceChecker;
    [SerializeField] private List<int> correctSequence;

    private Animator anim;
    private bool isBusy = false;

    [SerializeField] private AudioClip interactionSound;
    [SerializeField] private AudioMixerGroup audioMixerGroup; 
    [SerializeField] private float volume = 1.0f;
    [SerializeField] private GameObject noteObject; 

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (playerInZone && !isBusy)
        {
            isBusy = true;

            anim.SetTrigger("Klick");
            Debug.Log($"{gameObject.name} interacted with the player!");

            interactionSequence.Add(gameObject.name);
            Debug.Log("Interaction Sequence: " + string.Join(", ", interactionSequence));

            PlayInteractionSound();
            StartCoroutine(ShowNoteObject());

            if (sequenceChecker != null)
            {
                StartCoroutine(InteractionCooldownAndCheck());
            }
            else
            {
                Debug.LogError("SequenceChecker is not assigned in InteractionZone!");
                isBusy = false;
            }
        }
    }

    private IEnumerator InteractionCooldownAndCheck()
    {
        float animationLength = GetAnimationClipLength("Klick");
        yield return new WaitForSeconds(animationLength);

        sequenceChecker.CheckSequence(this);
        isBusy = false;
    }

    private float GetAnimationClipLength(string clipName)
    {
        if (anim.runtimeAnimatorController != null)
        {
            foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
            {
                if (clip.name == clipName)
                {
                    return clip.length;
                }
            }
        }
        Debug.LogWarning($"Анимация '{clipName}' не найдена. Используется длительность по умолчанию.");
        return 1f; // резервная длительность
    }

    private IEnumerator ShowNoteObject()
    {
        if (noteObject != null)
        {
            noteObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            noteObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Note object is not assigned.");
        }
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
        else
        {
            Debug.LogWarning($"No sound assigned for {gameObject.name}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            Debug.Log($"Player entered {gameObject.name}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            Debug.Log($"Player left {gameObject.name}");
        }
    }

    public bool IsPlayerInZone()
    {
        return playerInZone;
    }

    public static List<string> GetInteractionSequence()
    {
        return interactionSequence;
    }

    public static void ResetInteractionSequence()
    {
        interactionSequence.Clear();
        Debug.Log("Interaction sequence reset.");
    }

    public void TriggerWinAnimation()
    {
        anim.ResetTrigger("Lose");
        anim.ResetTrigger("Klick");
        anim.SetTrigger("Win");
    }

    public void TriggerLoseAnimation()
    {
        anim.ResetTrigger("Win");
        anim.ResetTrigger("Klick");
        anim.SetTrigger("Lose");
    }
}
