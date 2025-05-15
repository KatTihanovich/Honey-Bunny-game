using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private readonly Vector3 offset = new(0f, 3f, -25f);
    private const float SmoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;
    private CameraShake cameraShake;
    private CameraInputHandler inputHandler; // 🔹 Добавлено

    private void Awake()
    {
        cameraShake = GetComponent<CameraShake>();

        if (inputHandler == null) // 🔹 Добавлено
        {
            inputHandler = FindObjectOfType<CameraInputHandler>();
        }

        if (cameraShake == null)
        {
            cameraShake = FindObjectOfType<CameraShake>();
        }

        if (cameraShake == null)
        {
            Debug.LogWarning("CameraShake component not found in the scene.");
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            // 🔹 Получаем вертикальное смещение от инпут-хендлера
            float verticalOffset = inputHandler != null ? inputHandler.VerticalOffset : 0f;

            // 🔹 Добавляем вертикальное смещение к базовому offset
            var targetPosition = target.position + offset + new Vector3(0f, verticalOffset, 0f);

            Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);

            if (cameraShake != null && cameraShake.IsShaking)
            {
                smoothPosition += Random.insideUnitSphere * cameraShake.intensity;
            }

            transform.position = smoothPosition;
        }
    }
}
