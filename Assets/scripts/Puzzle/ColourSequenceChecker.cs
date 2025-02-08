using UnityEngine;
using System.Collections.Generic;

public class SequenceChecker : MonoBehaviour
{
    [SerializeField] private List<string> correctSequence = new List<string>(); 
    [SerializeField] private GameObject objectToHide; 
    [SerializeField] private List<InteractionZone> interactableZones = new List<InteractionZone>(); 

    public bool CheckSequence()
    {
        List<string> playerSequence = InteractionZone.GetInteractionSequence();

        // 🚀 Вывод последовательности в консоль
        Debug.Log("Current Player Sequence: " + string.Join(", ", playerSequence));

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

        // Обратная связь для всех объектов
        foreach (var zone in interactableZones)
        {
            if (isCorrect)
                zone.TriggerWinAnimation();
            else
                zone.TriggerLoseAnimation();
        }

        if (isCorrect)
        {
            Debug.Log("Puzzle solved!");
            if (objectToHide != null)
                objectToHide.SetActive(false);
            else
                Debug.LogWarning("No object assigned to hide.");
        }
        else
        {
            Debug.Log("Incorrect sequence! Resetting.");
            InteractionZone.ResetInteractionSequence();
        }

        return isCorrect;
    }

    // 📋 Новый метод для логирования последовательности в любой момент
    public void LogPlayerSequence()
    {
        List<string> playerSequence = InteractionZone.GetInteractionSequence();
        Debug.Log("Player Sequence: " + string.Join(", ", playerSequence));
    }
}