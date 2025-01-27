using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SequenceChecker : MonoBehaviour
{
    [SerializeField] private List<string> correctSequence = new List<string>(); // Correct interaction sequence
    private List<string> playerSequence = new List<string>(); // Player's interaction sequence

    [SerializeField] private GameObject objectToHide; // Object to hide upon correct sequence

    [SerializeField] private float colorChangeDuration = 0.5f; // Duration for color feedback
    [SerializeField] private List<GameObject> interactableObjects = new List<GameObject>(); // Objects to change color

    // Method to check the interaction sequence
    public void CheckSequence()
    {
        // Get the current sequence from InteractionZone
        playerSequence = InteractionZone.GetInteractionSequence();

        // Check if the sequence length is less than the correct sequence
        if (playerSequence.Count < correctSequence.Count)
        {
            Debug.Log("Sequence is incomplete. Keep interacting with objects.");
            return;
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

        if (isCorrect)
        {
            Debug.Log("Puzzle solved! Correct sequence entered.");

            // Change objects to green for feedback
            StartCoroutine(ChangeObjectsColor(Color.green));

            // Hide the object if assigned
            if (objectToHide != null)
            {
                objectToHide.gameObject.SetActive(false);
                Debug.Log("Object is now invisible and intangible.");
            }
            else
            {
                Debug.LogWarning("No object assigned to hide.");
            }
        }
        else
        {
            Debug.Log("Incorrect sequence! Resetting sequence.");

            // Change objects to red for feedback
            StartCoroutine(ChangeObjectsColor(Color.red));

            // Reset the interaction sequence
            InteractionZone.ResetInteractionSequence();
        }
    }

    // Coroutine to change objects' colors temporarily
    private IEnumerator ChangeObjectsColor(Color color)
    {
        List<SpriteRenderer> renderers = new List<SpriteRenderer>();

        // Collect SpriteRenderers from all interactable objects
        foreach (GameObject obj in interactableObjects)
        {
            if (obj.TryGetComponent(out SpriteRenderer renderer))
            {
                renderers.Add(renderer);
                renderer.color = color; // Change to the specified color
            }
        }

        // Wait for the specified duration
        yield return new WaitForSeconds(colorChangeDuration);

        // Reset objects to their original colors
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.color = Color.white; // Assuming original color is white
        }
    }

    public void LogPlayerSequence()
    {
        Debug.Log("Player Sequence: " + string.Join(", ", playerSequence));
    }
}
