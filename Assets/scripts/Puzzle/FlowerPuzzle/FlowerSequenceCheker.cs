using UnityEngine;
using System.Collections.Generic;

public class FlowerSequenceChecker : MonoBehaviour
{
    [SerializeField] private List<string> correctSequence = new List<string>(); 
    [SerializeField] private GameObject objectToDeActivate; 
    [SerializeField] private List<FlowerInteraction> flowerZones = new List<FlowerInteraction>(); 

    private void OnEnable()
    {
        ResetPuzzle(); 
    }
    public bool CheckSequence(FlowerInteraction triggeringObject)
{
    List<string> playerSequence = FlowerInteraction.GetInteractionSequence();
    Debug.Log("Current Player Sequence: " + string.Join(", ", playerSequence));
    Debug.Log("Correct Sequence: " + string.Join(", ", correctSequence));

    if (playerSequence.Count < correctSequence.Count)
    {
        Debug.Log("Sequence is incomplete. Keep interacting.");
        return false;
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

    // Вызываем анимацию победы или проигрыша для всех объектов
    foreach (var zone in flowerZones)
    {
        if (isCorrect)
        {
            zone.TriggerWinAnimation();
        }
        else
        {
            zone.TriggerLoseAnimation();
        }
    }

    // Дополнительные действия для победы
    if (isCorrect)
    {
        Debug.Log("Puzzle solved!");
        if (objectToDeActivate != null)
        {
            Animator sleepingFlowerAnimator = objectToDeActivate.GetComponent<Animator>();
            if (sleepingFlowerAnimator != null)
            {
                sleepingFlowerAnimator.SetTrigger("Awake"); 
                Debug.Log("Sleeping flower awakened!");
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
    }
    else
    {
        Debug.Log("Incorrect sequence! Resetting.");
        FlowerInteraction.ResetInteractionSequence();
    }

    return isCorrect;
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
            objectToDeActivate.GetComponent<BoxCollider2D>().isTrigger = true; 
       }
}