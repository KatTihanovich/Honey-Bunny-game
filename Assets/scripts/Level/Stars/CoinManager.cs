using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [SerializeField] private Text coinText; 
    private int totalCoins; 

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
        UpdateCoinUI(); 
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        coinText.text = $"Coins: {totalCoins}";
    }
}
