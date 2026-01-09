using UnityEngine;

public class TutorialHintTriggerNew : MonoBehaviour
{
    [Header("UI-подсказка")]
    public GameObject hintUI; // например, текст или панель в Canvas

    private void Start()
    {
        if (hintUI != null)
            hintUI.SetActive(false); // скрываем при старте
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hintUI != null)
        {
            hintUI.SetActive(true); // показать при входе
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hintUI != null)
        {
            hintUI.SetActive(false); // скрыть при выходе
        }
    }
}