using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public void OnInteractionButtonClick()
    {
        // Найти все объекты с компонентом InteractionZone
        InteractionZone[] zones = FindObjectsOfType<InteractionZone>();

        foreach (var zone in zones)
        {
            if (zone.IsPlayerInZone())
            {
                zone.Interact(); // Вызвать метод взаимодействия зоны
                return; // Остановить после первой найденной зоны
            }
        }

        Debug.Log("Player is not in any interaction zone.");
    }
}