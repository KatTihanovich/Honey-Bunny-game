using UnityEngine;

/// <summary>
/// Глобальный счётчик количества запусков и рестартов игры.
/// Работает на всех сценах, данные сохраняются локально через PlayerPrefs.
/// </summary>
public static class PlayCounter
{
    private const string StartGameKey = "StartGameCount";
    private const string RestartKey = "RestartCount";

    // ======================
    // ▶ СТАРТ НОВОЙ ИГРЫ
    // ======================
    /// <summary>
    /// Увеличить значение счётчика "начатых игр".
    /// </summary>
    public static void IncrementStartGameCount()
    {
        int currentCount = PlayerPrefs.GetInt(StartGameKey, 0);
        currentCount++;
        PlayerPrefs.SetInt(StartGameKey, currentCount);
        PlayerPrefs.Save();

        Debug.Log($"▶ Начато игр: {currentCount}");
    }

    /// <summary>
    /// Получить текущее количество начатых игр.
    /// </summary>
    public static int GetStartGameCount()
    {
        return PlayerPrefs.GetInt(StartGameKey, 0);
    }

    // ======================
    // 🔁 РЕСТАРТ УРОВНЯ
    // ======================
    /// <summary>
    /// Увеличить значение счётчика рестартов.
    /// </summary>
    public static void IncrementRestartCount()
    {
        int currentCount = PlayerPrefs.GetInt(RestartKey, 0);
        currentCount++;
        PlayerPrefs.SetInt(RestartKey, currentCount);
        PlayerPrefs.Save();

        Debug.Log($"🔁 Рестартов уровня: {currentCount}");
    }

    /// <summary>
    /// Получить текущее количество рестартов.
    /// </summary>
    public static int GetRestartCount()
    {
        return PlayerPrefs.GetInt(RestartKey, 0);
    }

    // ======================
    // ⚙️ ОБЩИЕ ОПЕРАЦИИ
    // ======================
    /// <summary>
    /// Сбросить оба счётчика (например, для тестов).
    /// </summary>
    public static void ResetAllCounts()
    {
        PlayerPrefs.DeleteKey(StartGameKey);
        PlayerPrefs.DeleteKey(RestartKey);
        PlayerPrefs.Save();

        Debug.Log("🔄 Счётчики запусков и рестартов сброшены.");
    }
}
