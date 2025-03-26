using UnityEngine;
using Spine.Unity;
public class DestructiblePlatformController : MonoBehaviour
{
    [SerializeField] private float destroyTime = 1f; // Время до разрушения
    [SerializeField] private float respawnTime = 5f; // Время до восстановления

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private bool isDestroyed = false;
    private bool isDestroySequenceRunning = false;

    public GameObject destroyAnimationPrefab;
    public string destroyAnimationName = "animation";

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

        Vector3 spawnPosition = transform.position + new Vector3(-2.5f, -0.63f, 0f);

    // Spawn the destroy animation at the adjusted position
    GameObject effect = Instantiate(destroyAnimationPrefab, spawnPosition, Quaternion.identity);

    // Play the destroy animation
    var skeletonAnimation = effect.GetComponent<SkeletonAnimation>();
    skeletonAnimation.AnimationState.SetAnimation(0, destroyAnimationName, false);

    // Auto-destroy effect after animation ends
    float animationDuration = skeletonAnimation.skeleton.Data.FindAnimation(destroyAnimationName).Duration;
    Destroy(effect, animationDuration);
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
