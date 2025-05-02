using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MeditationManager : MonoBehaviour
{
    private bool isPlayerInZone = false;
    private bool hasMeditatedOnce = false;
    private Animator animator;
    private HealthNew _health;
    private PlayerController _player;

    [SerializeField] private float _delayAfterAnimation = 1f;
    [SerializeField] private Animator secondaryAnimator;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Interact.performed += ctx => TryMeditate();
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
        if (animator == null)
            Debug.LogWarning("Animator not found on this object.");

        if (secondaryAnimator == null)
            Debug.LogWarning("Secondary Animator is not assigned.");
    }

    private void TryMeditate()
    {
        if (isPlayerInZone && !hasMeditatedOnce)
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

        if (secondaryAnimator != null)
            secondaryAnimator.SetTrigger("Meditation");

        if (_health != null)
        {
            Debug.Log("Restoring full health.");
            _health.RestoreFull();
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
            _player = other.GetComponent<PlayerController>();
            _health = other.GetComponent<HealthNew>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            _player = null;
            _health = null;
        }
    }
}
