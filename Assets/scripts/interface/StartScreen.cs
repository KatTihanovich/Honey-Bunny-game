using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Game.Audio;
using Level;

public class StartScreen : MonoBehaviour
{
    private bool hasPressedButton = false;
    private ISoundManager _soundManager;
    private LevelLoader levelLoader;

    private void Awake()
    {
        _soundManager = SoundManagerNew.Instance;
        levelLoader = GetComponent<LevelLoader>();
    }

    void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            hasPressedButton = true;
            _soundManager.PlaySound("UI");
            levelLoader.NextLevel(6);
        }
    }
}
