using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class InteractionZone : MonoBehaviour
{
    private bool playerInZone = false;
    private static List<string> interactionSequence = new List<string>();

    [SerializeField] private SequenceChecker sequenceChecker;
    [SerializeField] private List<int> correctSequence;

    private Animator anim;

    // 🎵 Sound variables
    [SerializeField] private AudioClip interactionSound;
    [SerializeField] private AudioMixerGroup audioMixerGroup; 
    [SerializeField] private float volume = 1.0f;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (playerInZone)
        {
            Debug.Log($"{gameObject.name} interacted with the player!");

            // Add to sequence
            interactionSequence.Add(gameObject.name);
            Debug.Log("Interaction Sequence: " + string.Join(", ", interactionSequence));

            // Play animation
            anim.SetTrigger("Klick");

            // 🎵 Play sound
            PlayInteractionSound();

            // Check sequence
            if (interactionSequence.Count >= 5)
            {
                if (sequenceChecker != null)
                {
                StartCoroutine(WaitForClickAndCheckSequence());
                }
                else
                {
                    Debug.LogError("SequenceChecker is not assigned in InteractionZone!");
                }
            }
        }
    }

    private void PlayInteractionSound()
    {
        if (interactionSound != null && audioMixerGroup != null) {
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

private IEnumerator WaitForClickAndCheckSequence()
    {
        // Ожидаем 0.5 секунды (время проигрывания анимации клика)
        yield return new WaitForSeconds(0.75f);

        // Передаем ссылку на текущий объект в SequenceChecker
        sequenceChecker.CheckSequence(this);
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
        return new List<string>(interactionSequence);
    }

    public static void ResetInteractionSequence()
    {
        interactionSequence.Clear();
        Debug.Log("Interaction sequence reset.");
    }

        public void TriggerWinAnimation()
{
    anim.ResetTrigger("Lose");
    anim.ResetTrigger("Klick"); // Сбрасываем анимацию клика
    anim.SetTrigger("Win");
}

public void TriggerLoseAnimation()
{
    anim.ResetTrigger("Win");
    anim.ResetTrigger("Klick"); // Сбрасываем анимацию клика
    anim.SetTrigger("Lose");
}
}