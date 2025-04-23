using UnityEngine;
using UnityEngine.InputSystem;

public class MeditationManager : MonoBehaviour
{
    private bool isPlayerInZone = false;
    private Animator animator;
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
        {
            Debug.LogWarning("Animator not found on this object.");
        }
    }

    private void TryMeditate()
    {
        if (isPlayerInZone)
        {
            Debug.Log("Meditate action triggered while player in zone.");
            Interact();
        }
        else
        {
            Debug.Log("Meditate action triggered but player is NOT in zone.");
        }
    }

    public void Interact()
    {
        if (animator != null)
        {
            Debug.Log("Triggering Meditation animation.");
            animator.SetTrigger("Meditation");
        }
        else
        {
            Debug.LogWarning("No animator found, cannot trigger animation.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            Debug.Log("Player entered meditation zone.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            Debug.Log("Player exited meditation zone.");
        }
    }
}
