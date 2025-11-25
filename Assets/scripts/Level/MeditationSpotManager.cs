using UnityEngine;
using UnityEngine.InputSystem;
using Game.Audio;

public class MeditationManager : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;

    private bool isPlayerInZone = false;
    private bool isOnCooldown = false;
    private bool isMeditating = false;

    private Animator animator;
    private Animator playerAnimator;
    private HealthNew playerHealth;
    private ISoundManager _soundManager;

    private PlayerInputActions inputActions;

    private float lastHealth; // отслеживаем изменения HP

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.Interact.performed += ctx => TryMeditate();
        inputActions.Player.Interrupt.performed += ctx => StopMeditation(); // пробел для прерывания
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        _soundManager = SoundManagerNew.Instance;

        animator = GetComponent<Animator>();

        if (playerObject == null)
            playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerHealth = playerObject.GetComponent<HealthNew>();
            playerAnimator = playerObject.GetComponent<Animator>();

            lastHealth = playerHealth.CurrentHealth; // фиксируем начальное HP
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
    }

    private void Update()
    {
        if (playerHealth == null) return;

        // === ОБНОВЛЕНИЕ АНИМАЦИЙ ===
        bool healthy = Mathf.Approximately(playerHealth.CurrentHealth, playerHealth.MaxHealth);

        animator?.SetBool("Healthy", healthy);
        playerAnimator?.SetBool("Healthy", healthy);

        // === АВТО-ВЫХОД ИЗ ПЕРЕЗАРЯДКИ ПРИ ЛЮБОМ ПАДЕНИИ HP ===
        if (playerHealth.CurrentHealth < lastHealth)
        {
            if (isOnCooldown)
            {
                Debug.Log("HP dropped → meditation recharged.");
                isOnCooldown = false;
            }
        }

        lastHealth = playerHealth.CurrentHealth;
    }

    private void TryMeditate()
    {
        if (!isPlayerInZone)
            return;

        if (isOnCooldown)
        {
            Debug.Log("Meditation is on cooldown.");
            return;
        }

        if (Mathf.Approximately(playerHealth.CurrentHealth, playerHealth.MaxHealth))
        {
            Debug.Log("HP is full -> meditation not needed.");
            return;
        }

        Interact();
    }

    public void Interact()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning("Player health NULL — cannot restore HP.");
            return;
        }

        isOnCooldown = true;
        isMeditating = true;

        animator?.SetTrigger("Meditation");
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Meditation");
            _soundManager?.PlaySound("Meditation");
        }

        // лечим игрока
        playerHealth.RestoreFull();
    }

    public void StopMeditation()
    {
        if (!isMeditating) return;

        isMeditating = false;

        if (animator != null)
            animator.SetTrigger("Stop");

        if (playerAnimator != null)
            playerAnimator.SetTrigger("Stop");

        isOnCooldown = false; // сброс перезарядки, чтобы можно было начать медитацию снова

        Debug.Log("Meditation interrupted!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
        }
    }
}
