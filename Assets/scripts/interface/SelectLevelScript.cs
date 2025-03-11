using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class SelectLevelScript : MonoBehaviour
{
    public GameObject panel;  
    public GameObject ui_panel; 
    public Button toBeSelectedButton; 

    void Start()
    {
        panel.SetActive(false);
    }

    public void TogglePanel()
    {
        panel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(toBeSelectedButton.gameObject);
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
