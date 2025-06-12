using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TogglePanel : MonoBehaviour
{
    public GameObject panel;
    public Button[] buttonsToDisable;
    public GameObject toBeSelectedButton;

    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);

        //EventSystem.current.SetSelectedGameObject(toBeSelectedButton);

        foreach (Button btn in buttonsToDisable)
        {
            btn.interactable = !btn.interactable;
        }
    }
    public void ToggleFalse()
    {
        panel.SetActive(false);

        //EventSystem.current.SetSelectedGameObject(toBeSelectedButton);

        foreach (Button btn in buttonsToDisable)
        {
            btn.interactable = true;
        }
    }
}
