using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonImageSwitcher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Button button;
    private Image targetImage; 
    public Sprite defaultSprite; 
    public Sprite clickedSprite;
    public float scaleFactor = 1.7f;
    private Vector3 originalScale;

    void Start()
    {
        targetImage = button.GetComponent<Image>();
        targetImage.sprite = defaultSprite;

        if (targetImage != null)
        {
            originalScale = targetImage.rectTransform.localScale;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetImage.sprite = clickedSprite;

        if (targetImage != null)
        {
            targetImage.sprite = clickedSprite;
            targetImage.rectTransform.localScale = originalScale * scaleFactor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetScale();
        targetImage.sprite = defaultSprite;
    }

    private void ResetScale()
    {
        if (targetImage != null)
        {
            targetImage.rectTransform.localScale = originalScale;
        }
    }
}