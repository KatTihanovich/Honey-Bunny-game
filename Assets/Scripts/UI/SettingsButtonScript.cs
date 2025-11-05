using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsButtonScript : MonoBehaviour
{
    public GameObject panel; 
    public Button arrowButton; 
    public Button[] mainMenuButtons; 

    private bool isPanelVisible = false;

    void Start()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        SetNavigationForArrowOnly();
    }

    public void TogglePanel()
    {
        isPanelVisible = !isPanelVisible;
        panel.SetActive(isPanelVisible);

        if (isPanelVisible)
        {
            SetNavigationForDropdown();
        }
        else
        {
            SetNavigationForArrowOnly();
        }
    }

    private void SetNavigationForArrowOnly()
    {
        foreach (Button btn in mainMenuButtons)
        {
            btn.interactable = true;
        }
    }

    private void SetNavigationForDropdown()
    {
        EventSystem.current.SetSelectedGameObject(arrowButton.gameObject);

        Navigation nav = new Navigation { mode = Navigation.Mode.Vertical };
        arrowButton.navigation = nav;

        foreach (Button btn in mainMenuButtons)
        {
            btn.interactable = false;
        }
    }
}
