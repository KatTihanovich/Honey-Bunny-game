using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialHintTrigger : MonoBehaviour
{
    public GameObject hintUI;        
    public KeyCode keyToPress = KeyCode.E; 

    private bool isPlayerInRange = false;
    private bool hintShown = false;
    private bool hintActive = false;

    private void Start()
    {
        hintUI.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInRange && hintActive && !hintShown)
        {
            if (Input.GetKeyDown(keyToPress))
            {
                hintUI.SetActive(false);
                hintShown = true;
                ResumeGame();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hintShown && other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            ShowHintAndFreeze();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (!hintShown)
                hintUI.SetActive(false);
        }
    }

    private void ShowHintAndFreeze()
    {
        Time.timeScale = 0f;
        hintUI.SetActive(true);
        hintActive = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        hintActive = false;
    }
}
