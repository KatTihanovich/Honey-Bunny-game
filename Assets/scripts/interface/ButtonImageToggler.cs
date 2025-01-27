using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonImageToggler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Button button;
    public Sprite image1;
    public Sprite image1_clicked;
    public Sprite image2;
    public Sprite image2_clicked;

    private Image buttonImage;
    private bool isImage1 = true;

    public float scaleFactor = 1.7f; // How much to scale the button when clicked
    private Vector3 originalScale;

    void Start()
    {
        buttonImage = button.GetComponent<Image>();

        if (buttonImage != null)
        {
            originalScale = buttonImage.rectTransform.localScale;
        }
    }

    public void ToggleImage()
    {
        if (isImage1)
        {
            buttonImage.sprite = image2;
        }
        else
        {
            buttonImage.sprite = image1;
        }
        isImage1 = !isImage1;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isImage1)
        {
            buttonImage.sprite = image1_clicked;
        }
        else
        {
            buttonImage.sprite = image2_clicked;
        }

        if (buttonImage != null)
        {
            // Scale the button up when pressed
            buttonImage.rectTransform.localScale = originalScale * scaleFactor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonImage != null)
        {
            // Reset the scale back to the original size
            ResetScale();
        }

        if (isImage1)
        {
            buttonImage.sprite = image1;
        }
        else
        {
            buttonImage.sprite = image2;
        }

        // Toggle the image state
        ToggleImage();
    }

    private void ResetScale()
    {
        if (buttonImage != null)
        {
            buttonImage.rectTransform.localScale = originalScale;
        }
    }
}
