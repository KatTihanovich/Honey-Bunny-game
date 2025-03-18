using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonImageToggler : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    public Button button;
    public Sprite image1;
    public Sprite image1_selected;
    public Sprite image2;
    public Sprite image2_selected;

    private Image buttonImage;
    private bool isImage1 = true;
    private bool isSelected = false;

    public float scaleFactor = 1.7f; 
    private Vector3 originalScale;

    void Start()
    {
        buttonImage = button.GetComponent<Image>();

        if (buttonImage != null)
        {
            originalScale = buttonImage.rectTransform.localScale;
            buttonImage.sprite = image1; 
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        UpdateImage();
        ScaleUp();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        UpdateImage();
        ResetScale();
    }

    public void OnSubmit(BaseEventData eventData) // Triggered by keyboard Enter/Space
    {
        ToggleImage();
        UpdateImage();
    }

    public void ToggleFromExternal()
    {
        ToggleImage();
        UpdateImage();
    }

    private void UpdateImage()
    {
        if (isSelected)
        {
            buttonImage.sprite = isImage1 ? image1_selected : image2_selected;
        }
        else
        {
            buttonImage.sprite = isImage1 ? image1 : image2;
        }
    }

    private void ToggleImage()
    {
        isImage1 = !isImage1;
    }

    private void ScaleUp()
    {
        if (buttonImage != null)
        {
            buttonImage.rectTransform.localScale = originalScale * scaleFactor;
        }
    }

    private void ResetScale()
    {
        if (buttonImage != null)
        {
            buttonImage.rectTransform.localScale = originalScale;
        }
    }
}
