using UnityEngine;
using UnityEngine.UI;

public class ButtonImageToggler : MonoBehaviour
{
    public Button button;
    public Sprite image1;
    public Sprite image2;
    private Image buttonImage;
    private bool isImage1 = true;

    void Start()
    {
        buttonImage = button.GetComponent<Image>();
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
}
