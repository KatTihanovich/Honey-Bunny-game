using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementsUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text messageText;
    public Button backButton;

    void Start()
    {
        backButton.onClick.AddListener(OnBackClicked);
        LoadAchievements();
    }

    void LoadAchievements()
    {
        messageText.text = "Загрузка достижений...";
        
        StartCoroutine(GameAPIManager.Instance.GetUserAchievements(OnAchievementsLoaded));
    }

    void OnAchievementsLoaded(bool success, Achievement[] achievements)
{
    if (success)
    {
        if (achievements == null || achievements.Length == 0)
        {
            messageText.text = "У вас пока нет достижений";
            return;
        }

        string achievementsList = "Ваши достижения:\n\n";
        foreach (Achievement achievement in achievements)
        {
            achievementsList += $"🏆 {achievement.achievementName}\n";
            achievementsList += $"   {achievement.achievementDescription}\n";
            achievementsList += $"   Получено: {achievement.createdAt}\n\n";
        }
        
        messageText.text = achievementsList;
    }
    else
    {
        messageText.text = "Ошибка загрузки достижений";
    }
}


    void OnBackClicked()
    {
        gameObject.SetActive(false);
    }
}
