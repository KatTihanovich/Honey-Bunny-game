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
    public List<GameObject> menusToDisable = new List<GameObject>(); 
    public Image image;
    public Sprite defaultSprite;
    public GameObject toSelect; 
    [Header("Player Input")]
    public PlayerInput playerInput; 

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        playerInput.SwitchCurrentActionMap("Player");
    }

    public void ResumeWithButton()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        playerInput.SwitchCurrentActionMap("Player");
        if (image != null)
        {
            image.sprite = defaultSprite;
        }
    }

    public void Pause()
    {
        EventSystem.current.SetSelectedGameObject(toSelect);

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void PauseWithDisabling()
    {
        EventSystem.current.SetSelectedGameObject(toSelect);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        playerInput.SwitchCurrentActionMap("UI");
        
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
