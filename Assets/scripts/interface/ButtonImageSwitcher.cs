using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonImageSwitcher : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Button button;
    private Image targetImage;
    public Sprite defaultSprite;
    public Sprite selectedSprite;
    public float scaleFactor = 1.7f;
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

    public void OnSelect(BaseEventData eventData)
    {
        if (targetImage != null)
        {
            targetImage.sprite = selectedSprite;
            targetImage.rectTransform.localScale = originalScale * scaleFactor;
        }
    }

    public void OnDeselect(BaseEventData eventData)
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
