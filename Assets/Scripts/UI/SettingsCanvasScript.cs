using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsCanvasScript : MonoBehaviour
{
    public GameObject panel;  
    public Button settings_button;        
    public Button info_button;       
    public Button arrow_button;  
    public GameObject toBeSelectedButton; 

    void Start()
    {
        panel.SetActive(false);

        settings_button.interactable = true;
        info_button.interactable = true;
        arrow_button.interactable = true;
    }

    public void TogglePanel()
    {
        panel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(toBeSelectedButton.gameObject);

        settings_button.interactable = false;
        info_button.interactable = false;
        arrow_button.interactable = false;
    }

}
