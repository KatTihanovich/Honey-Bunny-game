using UnityEngine;
using TMPro;

public class CoinText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private int maxCoins = 5;

    private void UpdateCoinText(string levelName)
    {
        int totalCoins = TotalCoinTracker.GetCoinsForLevel(levelName);
        if (coinText != null)
        {
            coinText.text = $"{totalCoins}/{maxCoins}";
        }
    }
}
