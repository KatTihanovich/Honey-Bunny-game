using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public Image mainMenuImage;
    public Sprite defaultSprite;
    public Sprite bossDefeatedSprite; 

    void Start()
    {
        if (PlayerPrefs.GetInt("BossDefeated", 0) == 1)
        {
            mainMenuImage.sprite = bossDefeatedSprite;
        }
        else
        {
            mainMenuImage.sprite = defaultSprite;
        }
    }
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
}
