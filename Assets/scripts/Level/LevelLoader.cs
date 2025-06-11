using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Level
{
    public class LevelLoader : MonoBehaviour
    {
        public GameObject LoadingScreen;
        public Image LoadingBarFill;

        public void NextLevel(int scene_id)
        {
            if (LoadingScreen != null && LoadingBarFill != null)
            {
                Time.timeScale = 1f;
                StartCoroutine(LoadSceneAsync(scene_id));
            }
            else
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(scene_id);
            }
            
        }

        IEnumerator LoadSceneAsync(int scene_id)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene_id);
            LoadingScreen.SetActive(true);

            float displayedProgress = 0f;

            while (!operation.isDone)
            {
                float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

                displayedProgress = Mathf.Lerp(displayedProgress, targetProgress, Time.deltaTime * 5f); // Adjust 5f to control speed
                LoadingBarFill.fillAmount = displayedProgress;

                yield return null;
            }

            LoadingBarFill.fillAmount = 1f;
        }

    }
}