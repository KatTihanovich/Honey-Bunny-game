using UnityEngine;
using UnityEngine.UI;

public class PauseImageToggle : MonoBehaviour
{
    public Button button;   
    public SpriteRenderer frontImage;   

    private bool isFrontVisible = false;

    void Start()
    {
        frontImage.gameObject.SetActive(false);
    }

    public void ToggleImage()
    {
        isFrontVisible = !isFrontVisible;

        frontImage.gameObject.SetActive(isFrontVisible);
    }
}
