using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public class LightColorZone : MonoBehaviour
{
    [Header("Light Settings")]
    public Light2D targetLight;           // Свет, цвет которого будем менять
    public Color zoneColor = Color.red;   // Цвет в зоне
    public float colorTransitionSpeed = 2f; // Скорость смены цвета

    private Color originalColor;
    private bool playerInside = false;

    void Start()
    {
        if (targetLight == null)
        {
            Debug.LogWarning("Target Light не назначен!");
        }
        else
        {
            originalColor = targetLight.color;
        }
    }

    void Update()
    {
        if (targetLight == null) return;

        Color desiredColor = playerInside ? zoneColor : originalColor;

        // Плавный переход цвета
        targetLight.color = Color.Lerp(
            targetLight.color,
            desiredColor,
            Time.deltaTime * colorTransitionSpeed
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
