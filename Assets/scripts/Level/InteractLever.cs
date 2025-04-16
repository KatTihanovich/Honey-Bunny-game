using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableToggle : MonoBehaviour
{
    public Animator animator;
    public GameObject doorBlocker;

    private bool isOpen = false;
    private bool playerInside = false;

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            Debug.Log("Animator автоматически присвоен: " + (animator != null));
        }

        if (doorBlocker != null)
            doorBlocker.SetActive(true); // Закрыто по умолчанию
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
                doorBlocker.SetActive(true);

            isOpen = false;
        }
        else
        {
            Debug.Log("Открываем объект");
            animator.SetTrigger("Open");

            if (doorBlocker != null)
                doorBlocker.SetActive(false);

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
