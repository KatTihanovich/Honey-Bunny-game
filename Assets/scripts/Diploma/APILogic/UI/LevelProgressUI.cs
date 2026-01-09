using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LevelProgressUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text[] levelProgressTexts; 
    public Button refreshButton;
    
    [Header("Level IDs")]
    public long[] levelIds;

    void Start()
    {
        refreshButton.onClick.AddListener(LoadAllLevelProgress);
        LoadAllLevelProgress();
    }

    void LoadAllLevelProgress()
    {
        refreshButton.interactable = false;
        
        foreach (TMP_Text text in levelProgressTexts)
        {
            text.text = "Loading...";
        }
        
        for (int i = 0; i < levelIds.Length; i++)
        {
            int index = i;
            StartCoroutine(LoadLevelProgress(levelIds[i], index));
        }
    }

    IEnumerator LoadLevelProgress(long levelId, int textIndex)
    {
        yield return GameAPIManager.Instance.GetLatestProgress(levelId, (success, progress) =>
        {
            if (success && progress != null)
            {
                string progressText = $"⭐ Stars: {progress.stars}/3\n";
                progressText += $"👾 Enemies: {progress.killedEnemiesNumber}\n";
                progressText += $"🧩 Puzzles: {progress.solvedPuzzlesNumber}\n";
                progressText += $"⏱️ Time: {progress.timeSpent}\n";
                progressText += $"📅 {progress.createdAt}";
                
                levelProgressTexts[textIndex].text = progressText;
            }
            else
            {
                levelProgressTexts[textIndex].text = $"Level is not completed yet";
            }
            
            CheckAllLoaded();
        });
    }

    void CheckAllLoaded()
    {
        bool allLoaded = true;
        foreach (TMP_Text text in levelProgressTexts)
        {
            if (text.text == "Loading...")
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
