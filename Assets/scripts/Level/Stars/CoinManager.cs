using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }
    [SerializeField] private Slider coinSlider; 
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
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateSlider();
    }
    private void UpdateSlider()
    {
        if (coinSlider != null)
        {
            coinSlider.value = totalCoins; 
        }
    }


}
