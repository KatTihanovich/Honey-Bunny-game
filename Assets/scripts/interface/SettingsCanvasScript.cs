using UnityEngine;
using UnityEngine.UI;
public class SettingsCanvasScript : MonoBehaviour
{
    public GameObject panel;  
    public Button settings_button;        
    public Button info_button;       
    public Button arrow_button;  
    private bool isPanelVisible = false; 
    void Start()
    {
        panel.SetActive(false);

        settings_button.interactable = true;
        info_button.interactable = true;
        arrow_button.interactable = true;
    }

    public void TogglePanel()
    {
        isPanelVisible = !isPanelVisible;

        panel.SetActive(isPanelVisible);

        settings_button.interactable = !isPanelVisible;
        info_button.interactable = !isPanelVisible;
        arrow_button.interactable = !isPanelVisible;
    }
}
