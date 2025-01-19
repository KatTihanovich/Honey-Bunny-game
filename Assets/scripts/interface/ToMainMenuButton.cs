using UnityEngine;
using UnityEngine.UI;

public class ToMainMenuButton : MonoBehaviour
{
    public GameObject button;
    public GameObject panel;

    public void OnButtonClick()
    {
        panel.SetActive(false);
        button.SetActive(true);
    }
}
