using UnityEngine;
using Spine.Unity;
using Game.Audio;

public class DestructiblePlatformController : MonoBehaviour
{
    [SerializeField] private float destroyTime = 1f; // Время до разрушения
    [SerializeField] private float respawnTime = 5f; // Время до восстановления

    [SerializeField] private LayerMask _playerLayer; // Слой игрока, чтобы проверять, наступил ли на платформу
    [SerializeField] private Transform _raycastOrigin; // Точка, откуда будет исходить Raycast (например, центр платформы)

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private bool isDestroyed = false;
    private bool isDestroySequenceRunning = false;

    private Animator animator;
    private Collider2D platformCollider;
    private ISoundManager _soundManager;


    private void Awake()
    {
        // Сохраняем начальное состояние платформы
        animator = GetComponent<Animator>();
        platformCollider = GetComponent<Collider2D>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
        _soundManager = SoundManagerNew.Instance;
    }

    private void Update()
    {
        // Проверяем, наступил ли игрок на платформу
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        Vector2 boxSize = new Vector2(4.8f, 1f);
        Vector2 boxCenter = _raycastOrigin.position + new Vector3(0f, 0.5f, 0f); // Центр области проверки

        Collider2D playerCollider = Physics2D.OverlapBox(boxCenter, boxSize, 0f, _playerLayer);

        if (playerCollider != null)
        {
            // Игрок находится в области, запускаем разрушение
            if (!isDestroyed && !isDestroySequenceRunning)
            {
                StartDestroying();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализируем область OverlapBox в редакторе для удобства
        if (_raycastOrigin != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_raycastOrigin.position + new Vector3(0f, 0.5f, 0f), new Vector3(5f, 1f, 0f)); // Отображаем область
        }
    }


    public void StartDestroying()
    {
        isDestroyed = true; 
        if (isDestroySequenceRunning == false)
        {
            isDestroySequenceRunning = true;
            Invoke(nameof(DestroyPlatform), destroyTime); // Задержка перед разрушением
        }
    }

    private void DestroyPlatform()
    {
        animator.SetTrigger("Break");
        _soundManager.PlaySound("BreakPlatform");
        // Отключаем платформу
        Invoke(nameof(DisableCollider), 1f);
        // Планируем восстановление через respawnTime
        Invoke(nameof(RespawnPlatform), respawnTime);

        Vector3 spawnPosition = transform.position + new Vector3(-2.5f, -0.63f, 0f);
    }

    private void DisableCollider()
    {
        platformCollider.enabled = false;
    }

    private void RespawnPlatform()
    {
        // Возвращаем платформу
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;
        platformCollider.enabled = true;
        isDestroyed = false;
        isDestroySequenceRunning = false;
        animator.SetTrigger("Collect"); 

    }

}
