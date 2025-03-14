using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SoundPauseManager : MonoBehaviour
{
    [Header("Player Input")]
    public PlayerInput playerInput;
    public GameObject pauseMenuUI;
    public GameObject toSelect;
    public ButtonImageToggler soundButtonImageToggler; 
    public ButtonImageToggler pauseButtonImageToggler; 

    private bool isPaused = false;

    public void Update()
    {
        // Toggle Pause Menu
        if (Keyboard.current.pKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleButtonImage(pauseButtonImageToggler);
            TogglePauseMenu();
        }

        // Toggle Sound
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            ToggleButtonImage(soundButtonImageToggler);
            SoundControl();
        }
    }

    private void TogglePauseMenu()
    {
        isPaused = !isPaused;

        pauseMenuUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            EventSystem.current.SetSelectedGameObject(toSelect);
            playerInput.SwitchCurrentActionMap("UI");
        }
        else
        {
            playerInput.SwitchCurrentActionMap("Player");
        }
    }

    public void SoundControl()
    {
        AudioListener.pause = !AudioListener.pause;
    }

    private void ToggleButtonImage(ButtonImageToggler buttonImageToggler)
    {
        if (buttonImageToggler != null)
        {
            buttonImageToggler.OnSubmit(null);
        }
    }
}
