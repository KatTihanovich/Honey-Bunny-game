using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [SerializeField] private Text coinText; 
    //[SerializeField] private GameObject starBar;
    //[SerializeField] private float TargetStars = 3;
   
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
        //starBar = starBar.GetComponent<Slider>();
        UpdateCoinUI(); 
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateCoinUI();

        //float progressPerCoin = 1f / TargetStars;
        //starBar.IncrementProgress(progressPerCoin);
    }

    private void UpdateCoinUI()
    {
        coinText.text = $"Coins: {totalCoins}";
    }
}
