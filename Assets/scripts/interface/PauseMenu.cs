using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Image image; 
    public Sprite defaultSprite; 

    void Update()
    {
        
    }

    public void Resume(){
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ResumeWithButton(){
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        image.sprite = defaultSprite;
    }

    public void Pause(){
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void LoadMenu(){
        SceneManager.LoadSceneAsync(0);
    }
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
