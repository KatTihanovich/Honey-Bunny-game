using UnityEngine;
using System.Collections.Generic;

public class TotalCoinTracker : MonoBehaviour
{
    private const string _levelCoinsKey = "LevelCoins";
    private const string _versionKey = "GameVersion"; // ключ для хранения версии

    [System.Serializable]
    private class LevelCoinDataWrapper
    {
        public List<LevelCoinEntry> levelCoins;
    }

    [System.Serializable]
    private class LevelCoinEntry
    {
        public string levelName;
        public int coinCount;
    }

    private void Start()
    {
        CheckVersionAndResetIfNeeded();
        Debug.Log("Кол-во всех собранных монет: " + GetTotalCoins());
    }

    /// <summary>
    /// Проверяет версию игры и сбрасывает прогресс, если версия изменилась.
    /// </summary>
    private void CheckVersionAndResetIfNeeded()
    {
        string currentVersion = Application.version;
        string savedVersion = PlayerPrefs.GetString(_versionKey, "");

        if (savedVersion != currentVersion)
        {
            Debug.Log($"Версия изменилась: {savedVersion} → {currentVersion}. Сброс прогресса.");
            ResetAllProgress();
            PlayerPrefs.SetString(_versionKey, currentVersion);
            PlayerPrefs.Save();
        }
    }

    public static int GetTotalCoins()
    {
        int total = 0;

        if (PlayerPrefs.HasKey(_levelCoinsKey))
        {
            string json = PlayerPrefs.GetString(_levelCoinsKey);
            var data = JsonUtility.FromJson<LevelCoinDataWrapper>(json);

            foreach (var entry in data.levelCoins)
            {
                total += entry.coinCount;
            }
        }

        return total;
    }

    public static int GetCoinsForLevel(string levelName)
    {
        if (PlayerPrefs.HasKey(_levelCoinsKey))
        {
            string json = PlayerPrefs.GetString(_levelCoinsKey);
            var data = JsonUtility.FromJson<LevelCoinDataWrapper>(json);

            foreach (var entry in data.levelCoins)
            {
                if (entry.levelName == levelName)
                    return entry.coinCount;
            }
        }

        return 0;
    }

    public static void ResetProgressForLevel(string levelName)
    {
        if (!PlayerPrefs.HasKey(_levelCoinsKey)) return;

        string json = PlayerPrefs.GetString(_levelCoinsKey);
        var data = JsonUtility.FromJson<LevelCoinDataWrapper>(json);

        data.levelCoins.RemoveAll(entry => entry.levelName == levelName);

        PlayerPrefs.SetString(_levelCoinsKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();

        FindAnyObjectByType<CoinManager>().ResetAllCollectedCoins();

        Debug.Log($"Прогресс уровня '{levelName}' был удалён.");
    }

    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteKey(_levelCoinsKey);
        PlayerPrefs.Save();

        Debug.Log("Весь прогресс по монетам был сброшен.");
    }
}
