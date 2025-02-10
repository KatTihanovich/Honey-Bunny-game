using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI coinTextWithWarning;  // UI with warning
    [SerializeField] private TextMeshProUGUI coinTextWithoutWarning;  // UI without warning

    [Header("Coin Settings")]
    [SerializeField] private int maxCoins = 4;  // Max for main coins
    [SerializeField] private int maxSpecialCoins = 4; // Max for special coins (Score)
    
    public int totalCoins = 0;   // First counter
    private int specialCoins = 0; // Second counter (e.g., collectibles, bonuses)

    [Header("Color Settings")]
    [SerializeField] private bool enableRedWarning = true;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lowCoinColor = Color.red;

    private const string StarsKey = "TotalStars";  // Key for PlayerPrefs
    private int totalStars;

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
        UpdateCoinTexts();
    }

    /// <summary>
    /// Adds coins to the **main counter** (with warning).
    /// </summary>
    public void AddCoins(int amount)
    {
        totalCoins += amount;
        totalCoins = Mathf.Clamp(totalCoins, 0, maxCoins); // Prevent overflow
        UpdateCoinTexts();
    }

    /// <summary>
    /// Adds coins to the **special counter** (without warning).
    /// </summary>
    public void AddSpecialCoins(int amount)
    {
        specialCoins += amount;
        specialCoins = Mathf.Clamp(specialCoins, 0, maxSpecialCoins); // Prevent overflow
        UpdateCoinTexts();
        PlayerPrefs.SetInt(StarsKey, totalStars);
        PlayerPrefs.Save();
    }

    private void UpdateCoinTexts()
    {
        if (coinTextWithWarning != null)
        {
            coinTextWithWarning.text = $"{totalCoins}/{maxCoins}";
            CheckCoinColor();
        }

        if (coinTextWithoutWarning != null)
        {
            coinTextWithoutWarning.text = $"{specialCoins}/{maxSpecialCoins}";
        }
    }
    private void LoadStars()
    {
        totalStars = PlayerPrefs.GetInt(StarsKey, 0);  // Default is 0
    }

    private void CheckCoinColor()
    {
        if (coinTextWithWarning != null)
        {
            float percentage = (float)totalCoins / maxCoins;
            coinTextWithWarning.color = (enableRedWarning && percentage < 0.8f) ? lowCoinColor : normalColor;
        }
    }
}
