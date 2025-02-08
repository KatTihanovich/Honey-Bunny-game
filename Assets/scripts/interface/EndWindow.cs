using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; 

public class EndWindow : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject endWindow; 
    public Button restartButton;
    [SerializeField] private TextMeshProUGUI starsText; 
    [SerializeField] private TextMeshProUGUI mobsText; 
    public static int enemiesDefeated = 0;

    void Start()
    {
        if (starsText != null)
        {
            starsText.text = CoinManager.Instance.totalCoins.ToString();
        }
        if (mobsText != null)
        {
            mobsText.text = enemiesDefeated.ToString();
        }
    }

    public static void IncreaseEnemyCount()
    {
        enemiesDefeated++;
        Debug.Log("Enemies Defeated: " + enemiesDefeated);
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
