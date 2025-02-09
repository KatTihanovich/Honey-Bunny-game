using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Include TextMeshPro namespace

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }
    [SerializeField] private Slider coinSlider;
    [SerializeField] private TextMeshProUGUI coinText;  // Change to TextMeshProUGUI
    [SerializeField] private int maxCoins = 100;

    public int totalCoins = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (coinSlider != null)
        {
            coinSlider.maxValue = maxCoins;
            coinSlider.value = totalCoins;
        }
        UpdateCoinText(); // Ensure the text starts correctly
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateSlider();
        UpdateCoinText(); // Update the coin text when coins are added
    }

    private void UpdateSlider()
    {
        if (coinSlider != null)
        {
            coinSlider.value = totalCoins;
        }
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = $"{totalCoins}/{maxCoins}"; // Display in format "currentCoins/maxCoins"
        }
    }
}