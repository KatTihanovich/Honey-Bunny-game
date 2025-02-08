using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractionZone : MonoBehaviour
{
    private bool playerInZone = false;

    private static List<string> interactionSequence = new List<string>();

    [SerializeField] private SequenceChecker sequenceChecker;
    [SerializeField] private List<int> correctSequence; // Assuming correctSequence is a list of integers

    private Vector3 originalScale;

    private Animator anim;

    private void Start()
    {
        originalScale = transform.localScale; // Store the original scale of the object
        anim = GetComponent<Animator>();
    }

    public void Interact()
{
    if (playerInZone)
    {
        Debug.Log($"{gameObject.name} interacted with the player!");

        // Добавляем в последовательность
        interactionSequence.Add(gameObject.name);
        Debug.Log("Interaction Sequence: " + string.Join(", ", interactionSequence));

        // Запускаем анимацию взаимодействия
        anim.SetTrigger("Klick");

        // Проверяем последовательность
        if (interactionSequence.Count >= 5)
        {
            if (sequenceChecker != null)
            {
                bool isCorrect = sequenceChecker.CheckSequence();
            }
            else
            {
                Debug.LogError("SequenceChecker is not assigned in InteractionZone!");
            }
        }
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
        return new List<string>(interactionSequence);
    }

    public static void ResetInteractionSequence()
    {
        interactionSequence.Clear();
        Debug.Log("Interaction sequence reset.");
    }

    public void TriggerWinAnimation()
    {
        anim.SetTrigger("Win");
    }

    public void TriggerLoseAnimation()
    {
        anim.SetTrigger("Lose");
    }
}