using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonScript : MonoBehaviour
{
    public GameObject panel;  
    public Button settingsToggleButton; 
    public Button button_main1;        
    public Button button_main2;       
    public Button button_main3;  
    private bool isPanelVisible = false; 
    void Start()
    {
        panel.SetActive(false);

        button_main1.interactable = true;
        button_main2.interactable = true;
        button_main3.interactable = true;
    }


    public void TogglePanel()
    {
        isPanelVisible = !isPanelVisible;

        panel.SetActive(isPanelVisible);

        button_main1.interactable = !isPanelVisible;
        button_main2.interactable = !isPanelVisible;
        button_main3.interactable = !isPanelVisible;
    }
}
