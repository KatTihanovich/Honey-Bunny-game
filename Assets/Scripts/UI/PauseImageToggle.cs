using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseImageToggle : MonoBehaviour
{
    public Button button;   

    public Sprite image1_clicked;
    public Sprite image2;

    private Image buttonImage;

    void Start()
    {
        buttonImage = button.GetComponent<Image>();
    }

    public void ToggleImage()
    {
        buttonImage.sprite = image2;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonImage.sprite = image1_clicked;
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        ToggleImage();
    }
}
