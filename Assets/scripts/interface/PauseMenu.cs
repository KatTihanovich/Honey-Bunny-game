using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuUI;
    public GameObject settingsUI;
    public List<GameObject> menusToDisable = new List<GameObject>(); 
    public Image image;
    public Sprite defaultSprite;
    public GameObject toSelectOnPause; 
    public GameObject toSelectOnSeetings; 

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ResumeWithButton()
    {
        Resume();
        if (image != null)
        {
            image.sprite = defaultSprite;
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(toSelectOnPause);
        Time.timeScale = 0f;
    }

    public void PauseWithDisabling()
    {
        Pause();
        foreach (GameObject menu in menusToDisable)
        {
            if (menu != null)
            {
                menu.SetActive(false);
            }
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        EndWindow.enemiesDefeated = 0;
        CoinManager.Instance.totalCoins = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ShowSettings()
    {
        EventSystem.current.SetSelectedGameObject(toSelectOnSeetings);
        settingsUI.SetActive(true);
    }
    public void HideSettings()
    {
        EventSystem.current.SetSelectedGameObject(toSelectOnPause);
        settingsUI.SetActive(false);
    }
}
