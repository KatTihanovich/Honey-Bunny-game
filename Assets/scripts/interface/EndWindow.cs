using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndWindow : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject endWindow; 
    public Button restartButton;

    void Start()
    {
        endWindow.SetActive(false);
    }

    public void ShowEndWindow()
    {
        endWindow.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
