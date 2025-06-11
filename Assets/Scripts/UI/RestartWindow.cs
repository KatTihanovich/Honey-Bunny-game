using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartWindow : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject restartWindow; 
    public Button restartButton;

    void Start()
    {
        restartWindow.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
    }

    public void ShowRestartWindow()
    {
        restartWindow.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

        public void QuitGame()
    {
        Time.timeScale = 1f; 
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; 
        #else
            Application.Quit();
        #endif
    }
}
