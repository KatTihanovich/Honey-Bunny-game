using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Game.Audio;

public class MeditationManager : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;

    private bool isPlayerInZone = false;
    private bool hasMeditatedOnce = false;

    private Animator animator;
    private Animator playerAnimator;
    private HealthNew playerHealth;
    private ISoundManager _soundManager;

    [SerializeField] private float _delayAfterAnimation = 3f;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Interact.performed += ctx => TryMeditate();
        _soundManager = SoundManagerNew.Instance;
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
    animator = GetComponent<Animator>();

    // Авто-поиск игрока по тегу, если не задан вручную
    if (playerObject == null)
    {
        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
        {
            playerObject = foundPlayer;
        }
        else
        {
            Debug.LogError("Игрок с тегом 'Player' не найден.");
        }
    }

    // Получаем компоненты с найденного/заданного объекта
    if (playerObject != null)
    {
        playerHealth = playerObject.GetComponent<HealthNew>();
        playerAnimator = playerObject.GetComponent<Animator>();

        if (playerHealth == null)
            Debug.LogWarning("HealthNew компонент не найден на игроке.");

        if (playerAnimator == null)
            Debug.LogWarning("Animator компонент не найден на игроке.");
    }
    else
    {
        Debug.LogError("playerObject всё ещё null. MeditationManager работать не сможет.");
    }
}



    private void TryMeditate()
    {
        if (isPlayerInZone && !hasMeditatedOnce)
        {
            Debug.Log("Meditate action triggered.");
            hasMeditatedOnce = true;
            Interact();
        }
        else if (hasMeditatedOnce)
        {
            Debug.Log("Meditation already used. No further interaction allowed.");
        }
    }

    public void Interact()
    {
        if (playerHealth.CurrentHealth != 100f)
        {
            if (animator != null)
                animator.SetTrigger("Meditation");

            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Meditation");
                _soundManager.PlaySound("Meditation");
            }

            if (playerHealth != null)
            {
                Debug.Log("Restoring full health.");
                playerHealth.RestoreFull();
            }
            else
            {
                Debug.LogWarning("Cannot restore health — HealthNew is null.");
            }

            StartCoroutine(FinishMeditationRoutine());
        }
    }

    private IEnumerator FinishMeditationRoutine()
    {
        yield return new WaitForSeconds(_delayAfterAnimation);
        Debug.Log("Meditation completed. Object remains active but cannot be reused.");
        // Здесь можно добавить анимацию покоя или эффект "пустого" алтаря
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