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

    // 🔥 Новые поля
    public BoxCollider2D boxCollider;
    public EdgeCollider2D edgeCollider;

    private bool isOpen = false;
    private bool playerInside = false;
    private bool isBusy = false;

    private ISoundManager _soundManager;
    public WaveMovement waveObject;


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

        // Проверяем наличие коллайдеров
        if (boxCollider == null) Debug.LogWarning("BoxCollider2D НЕ назначен!");
        if (edgeCollider == null) Debug.LogWarning("EdgeCollider2D НЕ назначен!");
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

        float waitTime = animator.GetCurrentAnimatorStateInfo(0).length;
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
//                doorCollider.isTrigger = false;
            }

            // 🔥 Вернуть коллайдеры в состояние закрытой двери
            if (boxCollider) boxCollider.enabled = true;
            if (edgeCollider) edgeCollider.enabled = false;

            isOpen = false;
        }
        else
        {
            Debug.Log("Открываем объект");
            animator.SetTrigger("Open");

            if (doorBlocker != null)
            {
                doorAnimator.SetTrigger("Open");
//                doorCollider.isTrigger = true;
            }

            waveObject?.Activate();

            // 🔥 Запускаем корутину переключения коллайдеров
            StartCoroutine(SwitchCollidersDelayed());

            isOpen = true;
        }

        _soundManager.PlaySound("Lever");
    }

    // 🔥 Корутина переключения коллайдеров
    private IEnumerator SwitchCollidersDelayed()
    {
        // Ждем 1 секунду
        yield return new WaitForSeconds(6.30f);

        if (boxCollider) boxCollider.enabled = false;
        if (edgeCollider) edgeCollider.enabled = true;
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
