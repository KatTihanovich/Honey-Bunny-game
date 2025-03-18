using UnityEngine;
using UnityEngine.InputSystem; // Подключаем новую систему ввода

public class InteractionManager : MonoBehaviour
{
    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Interact.performed += ctx => HandleInteraction(); // Подписка на кнопку
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void HandleInteraction()
    {
        Debug.Log("Interaction key pressed!");

        InteractionZone[] zones = FindObjectsOfType<InteractionZone>();
        FlowerInteraction[] flowerZones = FindObjectsOfType<FlowerInteraction>();

        foreach (var zone in zones)
        {
            if (zone.IsPlayerInZone())
            {
                Debug.Log("Interacting with InteractionZone: " + zone.name);
                zone.Interact();
                return;
            }
        }

        foreach (var flowerZone in flowerZones)
        {
            if (flowerZone.IsPlayerInZone())
            {
                Debug.Log("Interacting with FlowerInteraction: " + flowerZone.name);
                flowerZone.Interact();
                return;
            }
        }

        Debug.Log("Player is not in any interaction zone.");
    }
}
