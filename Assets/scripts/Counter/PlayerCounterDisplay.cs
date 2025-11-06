using UnityEngine;
using TMPro; // обязательно, если используешь TextMeshPro

/// <summary>
/// Отображает текущее количество запусков игры на UI-тексте.
/// Обновляется каждый кадр.
/// </summary>
public class PlayCounterDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;

    private void Update()
    {
        if (counterText != null)
            counterText.text = $"Игроков сыграло: {PlayCounter.GetPlayCount()}";
    }
}
