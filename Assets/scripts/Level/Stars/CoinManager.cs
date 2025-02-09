using UnityEngine;
using TMPro;  // For TextMeshPro

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI coinText; 

    [Header("Coin Settings")]
    [SerializeField] private int maxCoins = 100;
    public int totalCoins = 0;

    [Header("Color Settings")]
    [SerializeField] private bool enableRedWarning = true; // Toggle in Inspector
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lowCoinColor = Color.red;

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
        UpdateCoinText();
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = $"{totalCoins}/{maxCoins}";
            CheckCoinColor();
        }
    }

    private void CheckCoinColor()
    {
        if (coinText != null)
        {
            float percentage = (float)totalCoins / maxCoins;
            
            if (enableRedWarning)
            {
                coinText.color = (percentage < 0.8f) ? lowCoinColor : normalColor;
            }
            else
            {
                coinText.color = normalColor; // Always white if warning is disabled
            }
        }
    }

    /// <summary>
    /// Allows toggling the red text warning from code if needed.
    /// </summary>
    public void SetRedWarning(bool enable)
    {
        enableRedWarning = enable;
        CheckCoinColor(); // Refresh text color immediately
    }
}
