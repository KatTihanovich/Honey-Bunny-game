using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SoundPauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject toSelect;

    private bool isPaused = false;

    public void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePauseMenu();
        }

        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            SoundControl();
        }
    }

    private void TogglePauseMenu()
    {
        isPaused = !isPaused;

        pauseMenuUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        EventSystem.current.SetSelectedGameObject(toSelect);
    }

    public void SoundControl()
    {
        AudioListener.pause = !AudioListener.pause;
    }

}
