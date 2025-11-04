using UnityEngine;
using UnityEngine.InputSystem;
using Game.Audio;
using System.Collections;

public class InteractableToggle : MonoBehaviour
{
    public Animator animator;
    public GameObject doorBlocker;
    private Animator doorAnimator;     
    private Collider2D doorCollider; 
    private bool isOpen = false;
    private bool playerInside = false;
    private bool isBusy = false; // <-- защита от спама
    private ISoundManager _soundManager;

    private void Start()
    {
        _soundManager = SoundManagerNew.Instance;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
            Debug.Log("Animator автоматически присвоен: " + (animator != null));
        }

        if (doorBlocker != null)
        {
            doorAnimator = doorBlocker.GetComponent<Animator>();
            doorCollider = doorBlocker.GetComponent<Collider2D>();
            doorBlocker.SetActive(true);
            doorCollider.isTrigger = false;
        }
        else
        {
            Debug.LogWarning("doorBlocker не назначен!");
        }
    }

    private void Update()
    {
        if (playerInside && Keyboard.current.eKey.wasPressedThisFrame && !isBusy)
        {
            Debug.Log("Игрок нажал E внутри зоны объекта.");
            StartCoroutine(ToggleRoutine());
        }
    }

    private IEnumerator ToggleRoutine()
    {
        isBusy = true;

        Toggle();

        // Ждём окончания анимации — можно использовать длительность анимации
        float waitTime = animator.GetCurrentAnimatorStateInfo(0).length; // <-- замените на точную длительность вашей анимации
        yield return new WaitForSeconds(waitTime);

        isBusy = false;
    }

    private void Toggle()
    {
        if (animator == null)
        {
            Debug.LogError("Animator не назначен!");
            return;
        }

        if (isOpen)
        {
            Debug.Log("Закрываем объект");
            animator.SetTrigger("Close");

            if (doorBlocker != null)
            {
                doorAnimator.SetTrigger("Close");
                doorCollider.isTrigger = false;
            }

            isOpen = false;
        }
        else
        {
            Debug.Log("Открываем объект");
            animator.SetTrigger("Open");

            if (doorBlocker != null)
            {
                doorAnimator.SetTrigger("Open");
                doorCollider.isTrigger = true;
            }

            isOpen = true;
        }

        _soundManager.PlaySound("Lever");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}