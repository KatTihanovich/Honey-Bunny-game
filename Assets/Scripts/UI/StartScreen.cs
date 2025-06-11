using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Game.Audio;
using Level;

public class StartScreen : MonoBehaviour
{
    private ISoundManager _soundManager;
    private LevelLoader levelLoader;
    public int sceneToLoadId = 2;
    private void Awake()
    {
        _soundManager = SoundManagerNew.Instance;
        levelLoader = GetComponent<LevelLoader>();
    }

    void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            LoadScene();
        }
    }

    public void LoadScene()
    {
        _soundManager.PlaySound("UI");
        levelLoader.NextLevelWithoutUI(sceneToLoadId);
    }
}
