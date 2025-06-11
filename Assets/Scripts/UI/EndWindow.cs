using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.EventSystems;

public class EndWindow : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject endWindow; 
    public Button restartButton;
    [SerializeField] private TextMeshProUGUI starsText; 
    [SerializeField] private TextMeshProUGUI mobsText; 
    [SerializeField] private TextMeshProUGUI puzzlesText; 
    public static int enemiesDefeated = 0;
    public static int puzzlesSolved = 0;
    public GameObject toSelect; 

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
        if (puzzlesText != null)
        {
            puzzlesText.text = puzzlesSolved.ToString();
        }
    }

    public static void IncreaseEnemyCount()
    {
        enemiesDefeated++;
        Debug.Log("Enemies Defeated: " + enemiesDefeated);
    }

    public void ShowEndWindow()
    {
        EventSystem.current.SetSelectedGameObject(toSelect);
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
