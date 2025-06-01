using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonImageSwitcher : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Button button;
    private Image targetImage;
    public Sprite defaultSprite;
    public Sprite selectedSprite;
    public Sprite clickedSprite;
    public float scaleFactor = 1.7f;
    public float clickedScaleFactor = 1.7f;
    private Vector3 originalScale;

    void Start()
    {
        targetImage = button.GetComponent<Image>();

        if (targetImage != null)
        {
            targetImage.sprite = defaultSprite;
            originalScale = targetImage.rectTransform.localScale;
        }
    }

    // public void OnSelect(BaseEventData eventData)
    // {
    //     if (targetImage != null)
    //     {
    //         targetImage.sprite = selectedSprite;
    //         targetImage.rectTransform.localScale = originalScale * scaleFactor;
    //     }
    // }

    // public void OnDeselect(BaseEventData eventData)
    // {
    //     ResetScale();
    //     targetImage.sprite = defaultSprite;
    // }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer entered button!");
        if (targetImage != null)
        {
            targetImage.rectTransform.localScale = originalScale * scaleFactor;
            targetImage.sprite = selectedSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer exited button!");
        if (targetImage != null)
        {
            ResetScale();
            targetImage.sprite = defaultSprite;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (targetImage != null && clickedSprite != null)
        {
            targetImage.rectTransform.localScale = originalScale * clickedScaleFactor;
            targetImage.sprite = clickedSprite;
        }
    }

    private void ResetScale()
    {
        if (targetImage != null)
        {
            targetImage.rectTransform.localScale = originalScale;
        }
    }
}
