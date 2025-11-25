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

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
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

    public void Restart()
    {
        Time.timeScale = 1f;
        EndWindow.enemiesDefeated = 0;
        if (CoinManager.Instance != null)
            CoinManager.Instance.totalCoins = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit(){
        Application.Quit();
    }
}
