using UnityEngine;
using UnityEngine.EventSystems;

public class HoverRevealImage2 : MonoBehaviour, IPointerExitHandler
{
    public GameObject overlayImage;
    public GameObject underImage;


    public void OnPointerExit(PointerEventData eventData)
    {
        if (overlayImage != null)
        {
            overlayImage.SetActive(false);
            underImage.SetActive(true);
        }
    }
}
