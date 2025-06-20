using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverRevealImage : MonoBehaviour, IPointerEnterHandler
{
    public GameObject overlayImage;
    public GameObject underImage;
    public string levelName;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private int maxCoins = 5;


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (overlayImage != null)
        {
            UpdateCoinText();
            overlayImage.SetActive(true);
            underImage.SetActive(false);
        }
    }

    private void UpdateCoinText()
    {
        int totalCoins = TotalCoinTracker.GetCoinsForLevel(levelName);
        if (coinText != null)
        {
            coinText.text = $"{totalCoins}/{maxCoins}";
        }
    }
}
