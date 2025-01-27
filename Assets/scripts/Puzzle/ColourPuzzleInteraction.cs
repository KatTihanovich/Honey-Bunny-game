using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractionZone : MonoBehaviour
{
    private bool playerInZone = false;

    private static List<string> interactionSequence = new List<string>();

    [SerializeField] private SequenceChecker sequenceChecker;

    public float scaleFactor = 1.5f; // How much to scale up when interacting
    public float scaleDuration = 0.2f; // Duration of the scaling effect

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale; // Store the original scale of the object
    }

    public void Interact()
    {
        if (playerInZone)
        {
            Debug.Log($"{gameObject.name} interacted with the player!");

            // Add to the interaction sequence
            interactionSequence.Add(gameObject.name);
            Debug.Log("Interaction Sequence: " + string.Join(", ", interactionSequence));

            // Trigger the scaling effect
            StartCoroutine(ScaleEffect());

            // Check the sequence if the interaction count reaches 4
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

    private IEnumerator ScaleEffect()
    {
        // Scale up
        float elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleFactor, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale * scaleFactor; // Ensure it's fully scaled

        // Scale back to original
        elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale * scaleFactor, originalScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale; // Ensure it's back to the original scale
    }
}
