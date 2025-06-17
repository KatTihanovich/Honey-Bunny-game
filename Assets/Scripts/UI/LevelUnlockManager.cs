using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LevelButtonEntry
{
    public Button levelButton;
    public GameObject lockOverlay;
}

public class LevelUnlockManager : MonoBehaviour
{
    public LevelButtonEntry[] levelButtons; // Level 1 = index 0, Level 2 = index 1, etc.

    private void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("LevelUnlocked", 1); // Default: only level 1 unlocked

        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool isUnlocked = i < unlockedLevel;

            levelButtons[i].levelButton.interactable = isUnlocked;
            if (levelButtons[i].lockOverlay != null)
                levelButtons[i].lockOverlay.SetActive(!isUnlocked);
        }
    }
}
