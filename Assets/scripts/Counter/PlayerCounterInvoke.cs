using UnityEngine;

/// <summary>
/// Класс-обёртка для вызова счётчика через кнопки в Unity (OnClick()).
/// </summary>
public class PlayCounterInvoker : MonoBehaviour
{
    /// <summary>
    /// Вызвать увеличение счётчика.
    /// </summary>
    public void AddPlay()
    {
        PlayCounter.IncrementPlayCount();
    }

    /// <summary>
    /// Вывести текущее значение счётчика в консоль (по желанию).
    /// </summary>
    public void ShowPlayCount()
    {
        Debug.Log($"📊 Всего запусков игры: {PlayCounter.GetPlayCount()}");
    }

    /// <summary>
    /// Сбросить счётчик запусков (по желанию).
    /// </summary>
    public void ResetCounter()
    {
        PlayCounter.ResetPlayCount();
    }
}
