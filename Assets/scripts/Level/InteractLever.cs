using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableToggle : MonoBehaviour
{
    public Animator animator;
    public GameObject doorBlocker;
    private Animator doorAnimator;     
    private Collider2D doorCollider; 
    private bool isOpen = false;
    private bool playerInside = false;

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            Debug.Log("Animator автоматически присвоен: " + (animator != null));
        }

        if (doorBlocker != null){
            doorAnimator = doorBlocker.GetComponent<Animator>();
            doorCollider = doorBlocker.GetComponent<Collider2D>();
            doorBlocker.SetActive(true);
            doorCollider.isTrigger = false;

        }
                else
            Debug.LogWarning("doorBlocker не назначен!");
    }

    private void Update()
{
    if (playerInside && Keyboard.current.fKey.wasPressedThisFrame)
    {
        Debug.Log("Игрок нажал F внутри зоны объекта.");
        Toggle();
    }
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
            doorAnimator.SetTrigger("Close");
            doorCollider.isTrigger = false; // Закрыто по умолчанию
            Debug.Log("Закрыли дверь.");

            isOpen = false;
        }
        else
        {
            Debug.Log("Открываем объект");
            animator.SetTrigger("Open");

            if (doorBlocker != null)
            doorAnimator.SetTrigger("Open");
            doorCollider.isTrigger = true; // Закрыто по умолчанию
            Debug.Log("Открыли дверь.");

            isOpen = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
{
    Debug.Log("Что-то вошло в триггер: " + collision.gameObject.name);

    if (collision.CompareTag("Player"))
    {
        playerInside = true;
        Debug.Log("Игрок вошёл в зону взаимодействия.");
    }
}


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("Игрок вышел из зоны взаимодействия.");
        }
    }
}
