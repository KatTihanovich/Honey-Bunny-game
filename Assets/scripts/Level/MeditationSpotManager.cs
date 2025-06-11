using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Game.Audio;

public class MeditationManager : MonoBehaviour
{
    [SerializeField] private GameObject playerObject; // ← Один раз привязывается игрок в инспекторе

    private bool isPlayerInZone = false;
    private bool hasMeditatedOnce = false;

    private Animator animator;
    private Animator playerAnimator;
    private HealthNew playerHealth;
    private ISoundManager _soundManager;

    [SerializeField] private float _delayAfterAnimation = 1f;

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

        if (playerObject != null)
        {
            playerHealth = playerObject.GetComponent<HealthNew>();
            playerAnimator = playerObject.GetComponent<Animator>();

            if (playerHealth == null)
                Debug.LogWarning("HealthNew component not found on playerObject.");

            if (playerAnimator == null)
                Debug.LogWarning("Animator component not found on playerObject.");
        }
        else
        {
            Debug.LogError("Player object not assigned in inspector!");
        }
    }

    private void TryMeditate()
    {
        if (isPlayerInZone && !hasMeditatedOnce && playerHealth.CurrentHealth != 100f)
        {
            Debug.Log("Meditate action triggered.");
            hasMeditatedOnce = true;
            Interact();
        }
    }

    public void Interact()
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

        StartCoroutine(DeactivateAfterDelay());
    }

    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(_delayAfterAnimation);
        Debug.Log("Deactivating meditation object.");
        gameObject.SetActive(false);
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
