using UnityEngine;
using System.Collections.Generic;

public class SequenceChecker : MonoBehaviour
{
    [SerializeField] private List<string> correctSequence = new List<string>(); // Правильная последовательность
    private List<string> playerSequence = new List<string>(); // Последовательность игрока

    [SerializeField] private GameObject objectToHide; // Объект, который нужно сделать невидимым и неосязаемым

    // Метод для проверки последовательности
    public void CheckSequence()
    {
        // Получаем текущую последовательность взаимодействий
        playerSequence = InteractionZone.GetInteractionSequence();

        // Проверяем длину последовательности
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
            
            // Делаем объект невидимым и неосязаемым
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
            InteractionZone.ResetInteractionSequence(); 
        }
    }

    public void LogPlayerSequence()
    {
        Debug.Log("Player Sequence: " + string.Join(", ", playerSequence));
    }
}
