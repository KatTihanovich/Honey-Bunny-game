using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LevelProgressTracker : MonoBehaviour
{
    [Header("Level Info")]
    public long levelId = 1;
    
    [Header("References")]
    public EndWindow endWindow;
    
    private float startTime;
    private string levelName; // Теперь не публичное поле

    void Start()
    {
        startTime = Time.time;
        
        // Автоматически получаем имя текущей сцены
        levelName = SceneManager.GetActiveScene().name;
        
        Debug.Log($"Level Name: {levelName}");
        
        if (endWindow == null)
        {
            endWindow = FindObjectOfType<EndWindow>();
        }
    }

    public void OnLevelComplete()
    {
        float timeSpent = Time.time - startTime;
        string timeString = TimeSpanToString(timeSpent);
        
        int killedEnemies = endWindow.GetEnemyCount();
        int solvedPuzzles = endWindow.GetPuzzleCount();
        
        // Используем то же имя что и TotalCoinTracker
        int stars = TotalCoinTracker.GetCoinsForLevel(levelName);
        
        Debug.Log($"Уровень '{levelName}' завершён! Звёзд: {stars}, Врагов: {killedEnemies}, Пазлов: {solvedPuzzles}, Время: {timeString}");
        
        bool isLoggedIn = PlayerPrefs.HasKey("JWT_Token") && 
                         !string.IsNullOrEmpty(PlayerPrefs.GetString("JWT_Token"));
        
        if (isLoggedIn)
        {
            SaveProgress(stars, timeString, killedEnemies, solvedPuzzles);
        }
        else
        {
            Debug.Log("Пользователь не авторизован. Прогресс не будет сохранён в API.");
        }
    }

    void SaveProgress(int stars, string timeSpent, int killedEnemies, int solvedPuzzles)
    {
        StartCoroutine(GameAPIManager.Instance.SaveProgress(
            levelId,
            killedEnemies,
            solvedPuzzles,
            timeSpent,
            stars,
            OnProgressSaved
        ));
    }

    void OnProgressSaved(bool success, string response)
    {
        if (success)
        {
            Debug.Log("Прогресс успешно сохранён в API!");
        }
        else
        {
            Debug.LogError("Ошибка сохранения прогресса: " + response);
        }
    }

    string TimeSpanToString(float seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", 
            timeSpan.Hours, 
            timeSpan.Minutes, 
            timeSpan.Seconds);
    }
}
