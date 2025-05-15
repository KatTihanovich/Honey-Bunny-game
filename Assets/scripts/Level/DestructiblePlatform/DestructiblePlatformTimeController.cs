using UnityEngine;
using Game.Audio;

public class DestructiblePlatformTimeController : MonoBehaviour
{
    [SerializeField] private float destroyTime = 3f; // Время до разрушения
    [SerializeField] private float respawnTime = 3f; // Время до восстановления

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
        // Запускаем бесконечный цикл разрушения и восстановления
        StartDestroyingCycle();
    }

    private void StartDestroyingCycle()
    {
        if (!isDestroySequenceRunning)
        {
            isDestroySequenceRunning = true;
            Invoke(nameof(DestroyPlatform), destroyTime);
        }
    }

    private void DestroyPlatform()
    {
        animator.SetTrigger("Break");
        _soundManager.PlaySound("BreakPlatform");
        Invoke(nameof(DisableCollider), 1f);
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

        // Запускаем следующий цикл разрушения
        StartDestroyingCycle();
    }
}
