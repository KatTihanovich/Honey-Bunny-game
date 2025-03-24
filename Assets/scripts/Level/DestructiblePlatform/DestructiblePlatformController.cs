using UnityEngine;

public class DestructiblePlatformController : MonoBehaviour
{
    [SerializeField] private float destroyTime = 1f; // Время до разрушения
    [SerializeField] private float respawnTime = 5f; // Время до восстановления

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private bool isDestroyed = false;
    private bool isDestroySequenceRunning = false;

    private void Awake()
    {
        // Сохраняем начальное состояние платформы
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
    }

    public void StartDestroying()
    {
        isDestroyed = true; // Флаг разрушения

        //Changed by mentor in estoty
        if (isDestroySequenceRunning == false)
        {
            isDestroySequenceRunning = true;
            Invoke(nameof(DestroyPlatform), destroyTime); // Задержка перед разрушением
        }   
    }

    private void DestroyPlatform()
    {
        // Отключаем платформу
        gameObject.SetActive(false);
        // Планируем восстановление через respawnTime
        Invoke(nameof(RespawnPlatform), respawnTime);
    }

    private void RespawnPlatform()
    {
        // Восстанавливаем платформу в начальное состояние
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;
        gameObject.SetActive(true); // Включаем платформу
        isDestroyed = false; // Сбрасываем флаг разрушения
        isDestroySequenceRunning = false;
    }
}
