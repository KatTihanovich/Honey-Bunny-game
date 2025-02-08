using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(1);
    }

    public void PlayTutorial()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(2);
    }
    
    // TODO: Delete after
    public void PlayBossLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(4);
    }
}
