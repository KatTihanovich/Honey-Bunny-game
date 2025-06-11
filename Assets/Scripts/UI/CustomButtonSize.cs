using UnityEngine;
using UnityEngine.UI;

public class CustomButtonSize : MonoBehaviour
{
    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }
}
