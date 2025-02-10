using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuUI;
    public List<GameObject> menusToDisable = new List<GameObject>(); 
    public Image image;
    public Sprite defaultSprite;

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ResumeWithButton()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        if (image != null)
        {
            image.sprite = defaultSprite;
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void PauseWithDisabling()
    {
        pauseMenuUI.SetActive(true);
        
        foreach (GameObject menu in menusToDisable)
        {
            if (menu != null)
            {
                menu.SetActive(false);
            }
        }

        Time.timeScale = 0f;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
