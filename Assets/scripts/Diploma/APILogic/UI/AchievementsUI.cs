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

    void OnEnable()
    {
        LoadAchievements();
    }

    void LoadAchievements()
    {
        messageText.text = "Loading achievements...";
        
        StartCoroutine(GameAPIManager.Instance.GetUserAchievements(OnAchievementsLoaded));
    }

    void OnAchievementsLoaded(bool success, Achievement[] achievements)
{
    if (success)
    {
        if (achievements == null || achievements.Length == 0)
        {
            messageText.text = "You don't have any achievements yet.";
            return;
        }

        string achievementsList = "Your achievements:\n\n";
        foreach (Achievement achievement in achievements)
        {
            achievementsList += $"\n {achievement.achievementName}\n";
            achievementsList += $"\n  ●  {achievement.achievementDescription}\n\n";
        }
        
        messageText.text = achievementsList;
    }
    else
    {
        messageText.text = "Error loading achievements. Please try to login again.";
    }
}


    void OnBackClicked()
    {
        gameObject.SetActive(false);
    }
}
