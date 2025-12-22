using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LevelProgressUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text[] levelProgressTexts; // Массив Text для каждого уровня
    public Button refreshButton;
    
    [Header("Level IDs")]
    public long[] levelIds; // ID уровней из вашей БД

    void Start()
    {
        refreshButton.onClick.AddListener(LoadAllLevelProgress);
        LoadAllLevelProgress();
    }

    void LoadAllLevelProgress()
    {
        refreshButton.interactable = false;
        
        // Очищаем все тексты
        foreach (TMP_Text text in levelProgressTexts)
        {
            text.text = "Загрузка...";
        }
        
        // Загружаем прогресс для каждого уровня
        for (int i = 0; i < levelIds.Length; i++)
        {
            int index = i; // Локальная копия для замыкания
            StartCoroutine(LoadLevelProgress(levelIds[i], index));
        }
    }

    IEnumerator LoadLevelProgress(long levelId, int textIndex)
    {
        yield return GameAPIManager.Instance.GetLatestProgress(levelId, (success, progress) =>
        {
            if (success && progress != null)
            {
                // Форматируем данные прогресса
                string progressText = $"Уровень {levelId}\n";
                progressText += $"⭐ Звёзд: {progress.stars}/3\n";
                progressText += $"👾 Врагов: {progress.killedEnemiesNumber}\n";
                progressText += $"🧩 Пазлов: {progress.solvedPuzzlesNumber}\n";
                progressText += $"⏱️ Время: {progress.timeSpent}\n";
                progressText += $"📅 {progress.createdAt}";
                
                levelProgressTexts[textIndex].text = progressText;
            }
            else
            {
                levelProgressTexts[textIndex].text = $"Уровень {levelId}\nЕщё не пройден";
            }
            
            // Проверяем, загрузились ли все уровни
            CheckAllLoaded();
        });
    }

    void CheckAllLoaded()
    {
        bool allLoaded = true;
        foreach (TMP_Text text in levelProgressTexts)
        {
            if (text.text == "Загрузка...")
            {
                allLoaded = false;
                break;
            }
        }
        
        if (allLoaded)
        {
            refreshButton.interactable = true;
        }
    }
}
