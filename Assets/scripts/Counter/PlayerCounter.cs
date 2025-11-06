using UnityEngine;

/// <summary>
/// Простой глобальный счётчик количества запусков игры.
/// Работает на всех сценах, сохраняет данные локально через PlayerPrefs.
/// </summary>
public static class PlayCounter
{
    private const string PlayCountKey = "PlayCount";

    /// <summary>
    /// Увеличить значение счётчика на 1.
    /// </summary>
    public static void IncrementPlayCount()
    {
        int currentCount = PlayerPrefs.GetInt(PlayCountKey, 0);
        currentCount++;
        PlayerPrefs.SetInt(PlayCountKey, currentCount);
        PlayerPrefs.Save();

        Debug.Log($"▶ Количество запусков игры: {currentCount}");
    }

    /// <summary>
    /// Получить текущее количество запусков.
    /// </summary>
    public static int GetPlayCount()
    {
        return PlayerPrefs.GetInt(PlayCountKey, 0);
    }

    /// <summary>
    /// Сбросить счётчик (например, для тестов).
    /// </summary>
    public static void ResetPlayCount()
    {
        PlayerPrefs.DeleteKey(PlayCountKey);
        PlayerPrefs.Save();
        Debug.Log("🔄 Счётчик запусков сброшен.");
    }
}
