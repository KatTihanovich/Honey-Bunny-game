using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class BridgeEnemyStateController : MonoBehaviour
{
    [Header("Player Reference (авто-поиск по тегу 'Player')")]
    [SerializeField] private HealthNew playerHealth;

    [Header("State Thresholds")]
    [SerializeField] private float awakeThreshold = 70f;
    [SerializeField] private float stressedThreshold = 50f;

    private Animator animator;
    private Collider2D enemyCollider;

    private bool isAwake = false;
    private bool isStressed = false;
    private Coroutine disableColliderCoroutine;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (enemyCollider == null)
        {
            enemyCollider = GetComponent<Collider2D>();
        }
    }

    private void Start()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<HealthNew>();
            }

            if (playerHealth == null)
                Debug.LogWarning("Player HealthNew не найден! Скрипт BridgeEnemyStateController не будет работать корректно.");
        }
    }

    private void Update()
    {
        if (playerHealth == null || playerHealth.IsDead)
        {
            ResetState();
            return;
        }

        float health = playerHealth.CurrentHealth;

        // STRESSED
        if (health <= stressedThreshold)
        {
            if (!isStressed)
            {
                SetAwake(true);
                SetStressed(true);
                StartDisableColliderWithDelay();
            }
        }
        // AWAKE
        else if (health <= awakeThreshold)
        {
            if (!isAwake || isStressed)
            {
                SetAwake(true);
                SetStressed(false);
                CancelDisableCoroutine();
                EnableCollider();
            }
        }
        // NORMAL
        else
        {
            if (isAwake || isStressed)
            {
                ResetState();
            }
        }
    }

    private void SetAwake(bool value)
    {
        isAwake = value;
        animator.SetBool("Awake", value);
    }

    private void SetStressed(bool value)
    {
        isStressed = value;
        animator.SetBool("Stressed", value);
    }

    private void ResetState()
    {
        SetAwake(false);
        SetStressed(false);
        CancelDisableCoroutine();
        EnableCollider();
    }

    private void StartDisableColliderWithDelay()
    {
        if (disableColliderCoroutine != null)
            StopCoroutine(disableColliderCoroutine);

        disableColliderCoroutine = StartCoroutine(DisableColliderAfterDelay());
    }

    private void CancelDisableCoroutine()
    {
        if (disableColliderCoroutine != null)
        {
            StopCoroutine(disableColliderCoroutine);
            disableColliderCoroutine = null;
        }
    }

    private IEnumerator DisableColliderAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        if (isStressed) // всё ещё стресс
        {
            DisableCollider();
        }
    }

    private void DisableCollider()
    {
        if (enemyCollider != null)
            enemyCollider.enabled = false;
    }

    private void EnableCollider()
    {
        if (enemyCollider != null)
            enemyCollider.enabled = true;
    }
}
