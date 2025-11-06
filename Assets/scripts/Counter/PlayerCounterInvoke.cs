using UnityEngine;

/// <summary>
/// Класс-обёртка для вызова счётчика через кнопки в Unity (OnClick()).
/// </summary>
public class PlayCounterInvoker : MonoBehaviour
{
    public void AddStartGame()
    {
        PlayCounter.IncrementStartGameCount();
    }

    public void AddRestart()
    {
        PlayCounter.IncrementRestartCount();
    }

    public void ResetAll()
    {
        PlayCounter.ResetAllCounts();
    }
}