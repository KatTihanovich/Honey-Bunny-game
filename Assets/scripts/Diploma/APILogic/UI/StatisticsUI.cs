using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatisticsUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text messageText;
    public Button backButton;

    void Start()
    {
        backButton.onClick.AddListener(OnBackClicked);
        LoadStatistics();
    }

    void LoadStatistics()
    {
        messageText.text = "Loading...";
        
        StartCoroutine(GameAPIManager.Instance.GetUserStatistics(OnStatisticsLoaded));
    }

    void OnStatisticsLoaded(bool success, UserStatistics stats)
    {
        if (success)
        {
            if (stats == null)
        {
            messageText.text = "You don't have any statistics yet.";
            return;
        }

            string statsText = "Overall Statistics:\n\n";
            statsText += $"Levels Completed: {stats.totalLevelsCompleted}\n";
            statsText += $"Time in game: {stats.totalTimePlayed}\n";
            statsText += $"Killed enemies: {stats.totalKilledEnemies}\n";
            statsText += $"Solved puzzles: {stats.totalSolvedPuzzles}\n";
            statsText += $"Total stars: {stats.totalStars}\n";
            
            messageText.text = statsText;
        }
        else
        {
            messageText.text = "Error loading statistics";
        }
    }

    void OnBackClicked()
    {
        gameObject.SetActive(false);
    }
}
