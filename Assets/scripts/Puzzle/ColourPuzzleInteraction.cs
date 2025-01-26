using UnityEngine;
using System.Collections.Generic;

public class InteractionZone : MonoBehaviour
{
    private bool playerInZone = false; 

    private static List<string> interactionSequence = new List<string>();

    [SerializeField] private SequenceChecker sequenceChecker;

    public void Interact()
{
    if (playerInZone)
    {
        Debug.Log($"{gameObject.name} interacted with the player!");

        interactionSequence.Add(gameObject.name);
        Debug.Log("Interaction Sequence: " + string.Join(", ", interactionSequence));

        if (interactionSequence.Count == 4)
        {
            if (sequenceChecker != null)
            {
                sequenceChecker.CheckSequence();
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
}