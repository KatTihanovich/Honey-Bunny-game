using UnityEngine;
using Game.Audio;

public class DestructiblePlatformTimeController : MonoBehaviour
{
    [Header("Timing Settings")]
    [SerializeField] private float delayBeforeDestroy = 1f; // Задержка перед разрушением
    [SerializeField] private float destroyTime = 3f;         // Время до разрушения после старта
    [SerializeField] private float breakAnimationDelay = 1f; // Задержка перед отключением коллайдера
    [SerializeField] private float respawnTime = 3f;         // Время до восстановления

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private bool isDestroySequenceRunning = false;

    private Animator animator;
    private Collider2D platformCollider;
    private ISoundManager _soundManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        platformCollider = GetComponent<Collider2D>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
        _soundManager = SoundManagerNew.Instance;
    }

    private void Start()
    {
        StartDestroyingCycle();
    }

    private void StartDestroyingCycle()
    {
        if (!isDestroySequenceRunning)
        {
            isDestroySequenceRunning = true;
            Invoke(nameof(BeginDestructionSequence), delayBeforeDestroy);
        }
    }

    private void BeginDestructionSequence()
    {
        Invoke(nameof(DestroyPlatform), destroyTime);
    }

    private void DestroyPlatform()
    {
        animator.SetTrigger("Break");
        _soundManager.PlaySound("BreakPlatform");
        Invoke(nameof(DisableCollider), breakAnimationDelay);
        Invoke(nameof(RespawnPlatform), respawnTime);
    }

    private void DisableCollider()
    {
        platformCollider.enabled = false;
    }

    private void RespawnPlatform()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;
        platformCollider.enabled = true;
        animator.SetTrigger("Collect");
        isDestroySequenceRunning = false;

        StartDestroyingCycle();
    }
}
