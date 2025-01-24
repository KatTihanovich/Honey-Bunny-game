using UnityEngine;
using UnityEngine.UI;

public class SettingsCanvasScript : MonoBehaviour
{
    public GameObject panel;  
    public Button settings_button;        
    public Button info_button;       
    public Button arrow_button;  

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

        settings_button.interactable = false;
        info_button.interactable = false;
        arrow_button.interactable = false;
    }

}
