using UnityEngine;
using System.Collections.Generic;

public class TotalCoinTracker : MonoBehaviour
{
    private const string _levelCoinsKey = "LevelCoins";

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
        // Метод для вывода общего количества всех собранных монет
        Debug.Log("Кол-во всех собранных монет: " + GetTotalCoins());
    }

    /// <summary>
    /// Получает общее количество монет, собранных на всех уровнях.
    /// </summary>
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

    /// <summary>
    /// Получает количество монет, собранных на конкретном уровне.
    /// </summary>
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

    /// <summary>
    /// Удаляет прогресс только для указанного уровня.
    /// </summary>
    public static void ResetProgressForLevel(string levelName)
    {
        if (!PlayerPrefs.HasKey(_levelCoinsKey)) return;

        string json = PlayerPrefs.GetString(_levelCoinsKey);
        var data = JsonUtility.FromJson<LevelCoinDataWrapper>(json);

        data.levelCoins.RemoveAll(entry => entry.levelName == levelName);

        PlayerPrefs.SetString(_levelCoinsKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();

        Debug.Log($"Прогресс уровня '{levelName}' был удалён.");
    }

    /// <summary>
    /// Удаляет весь прогресс по монетам (все уровни).
    /// </summary>
    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteKey(_levelCoinsKey);
        PlayerPrefs.Save();

        Debug.Log("Весь прогресс по монетам был сброшен.");
    }
}
