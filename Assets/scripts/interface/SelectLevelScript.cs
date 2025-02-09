using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SelectLevelScript : MonoBehaviour
{
    public GameObject panel;  
    public GameObject ui_panel; 
    public Button select_button; 

    void Start()
    {
        panel.SetActive(false);
        select_button.interactable = true;
    }

    public void TogglePanel()
    {
        panel.SetActive(true);
        select_button.interactable = false;
        if (ui_panel != null)
        {
            ui_panel.SetActive(false);
        }
    }
    public void PlayForest()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(1);
    }

    public void PlayCave()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(3);
    }
    
    public void PlayBossLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(4);
    }
}
