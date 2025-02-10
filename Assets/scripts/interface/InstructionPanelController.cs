using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;

public class InstructionPanelController : MonoBehaviour
{
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private Button okButton;
    [SerializeField] private Button backButton; 
    [SerializeField] private string mainMenuSceneName = "MainMenu"; 

    [Header("Audio Settings")]
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private float volume = 1.0f;

    private bool isPaused = false; // Tracks whether interactions are paused

    private void Start()
    {
        if (okButton != null)
        {
            okButton.onClick.AddListener(OnOkButtonClicked);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

    }

    public void ShowInstructionPanel()
    {
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);
            PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
    }

    private void OnOkButtonClicked()
    {
        if (buttonSound != null)
        {
            AudioSource.PlayClipAtPoint(buttonSound, transform.position, volume);
        } 

        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
        ResumeGame();
    }

    private void OnBackButtonClicked()
    {
        if (buttonSound != null)
        {
            AudioSource.PlayClipAtPoint(buttonSound, transform.position, volume);
        }
        
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
