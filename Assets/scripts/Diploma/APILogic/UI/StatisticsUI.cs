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
        messageText.text = "Загрузка статистики...";
        
        StartCoroutine(GameAPIManager.Instance.GetUserStatistics(OnStatisticsLoaded));
    }

    void OnStatisticsLoaded(bool success, UserStatistics stats)
    {
        if (success)
        {
            string statsText = "Общая статистика:\n\n";
            statsText += $"Пройдено уровней: {stats.totalLevelsCompleted}\n";
            statsText += $"Время в игре: {stats.totalTimePlayed}\n";
            statsText += $"Убито врагов: {stats.totalKilledEnemies}\n";
            statsText += $"Решено головоломок: {stats.totalSolvedPuzzles}\n";
            statsText += $"Всего звёзд: {stats.totalStars}\n";
            
            messageText.text = statsText;
        }
        else
        {
            messageText.text = "Ошибка загрузки статистики";
        }
    }

    void OnBackClicked()
    {
        gameObject.SetActive(false);
    }
}
