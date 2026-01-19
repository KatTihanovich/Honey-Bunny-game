using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CameraMovement : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera targetCamera;      // Камера, FOV которой меняем
    public float targetFOV = 80f;    // FOV при нахождении игрока в зоне
    public float defaultFOV = 40f;   // FOV, когда игрок вне зоны
    public float fovTransitionSpeed = 2f;  // Скорость перехода FOV

    private bool playerInside = false;

    void Reset()
    {
        // Автоматически назначаем главную камеру
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        if (targetCamera == null) return;

        float desiredFOV = playerInside ? targetFOV : defaultFOV;

        // Плавный переход FOV
        targetCamera.fieldOfView = Mathf.Lerp(
            targetCamera.fieldOfView,
            desiredFOV,
            Time.deltaTime * fovTransitionSpeed
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
