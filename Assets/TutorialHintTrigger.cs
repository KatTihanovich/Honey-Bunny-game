using UnityEngine;

public class TutorialHintTrigger : MonoBehaviour
{
    public GameObject hintUI;        
    public KeyCode keyToPress = KeyCode.E; 
    public KeyCode anotherKeyToPress; 

    public bool allowSecondKey = false; 

    private bool isPlayerInRange = false;
    private bool hintShown = false;
    private bool hintActive = false;

    private PlayerController playerController;
    private PlayerAnimation playerAnimation;


    private void Start()
    {
        hintUI.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInRange && hintActive && !hintShown)
        {
            bool pressedKey1 = Input.GetKeyDown(keyToPress);
            bool pressedKey2 = allowSecondKey && Input.GetKeyDown(anotherKeyToPress);

            if (pressedKey1 || pressedKey2)
            {
                hintUI.SetActive(false);
                hintShown = true;
                ResumePlayer();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hintShown && other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            playerController = other.GetComponent<PlayerController>();
            playerAnimation = other.GetComponent<PlayerAnimation>(); 
            if (playerController != null)
            {
                ShowHintAndFreezePlayer();
            }
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

    private void ShowHintAndFreezePlayer()
    {
        hintUI.SetActive(true);
        hintActive = true;
        if (playerAnimation != null)
            playerAnimation.PlayIdle(); 

        playerController._isFrozen = true;
    }

    private void ResumePlayer()
    {
        hintActive = false;
        if (playerController != null)
            playerController._isFrozen = false;

        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            FindObjectOfType<PauseMenu>()?.Pause();
        }

    }
}
