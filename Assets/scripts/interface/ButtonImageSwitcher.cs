using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonImageSwitcher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Button button;
    private Image targetImage; 
    public Sprite defaultSprite; 
    public Sprite clickedSprite;

    void Start()
    {
        targetImage = button.GetComponent<Image>();
        targetImage.sprite = defaultSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetImage.sprite = clickedSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetImage.sprite = defaultSprite;
    }
}