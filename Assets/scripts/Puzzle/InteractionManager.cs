using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public void OnInteractionButtonClick()
    {
    // Найти все объекты с компонентом InteractionZone
    InteractionZone[] zones = FindObjectsOfType<InteractionZone>();
    FlowerInteraction[] flowerZones = FindObjectsOfType<FlowerInteraction>();

    // Проверить InteractionZone
    foreach (var zone in zones)
    {
        if (zone.IsPlayerInZone())
        {
            zone.Interact();
            return;
        }
    }

    // Проверить FlowerInteraction
    foreach (var flowerZone in flowerZones)
    {
        if (flowerZone.IsPlayerInZone())
        {
            flowerZone.Interact();
            return;
        }
    }

        Debug.Log("Player is not in any interaction zone.");
    }
}