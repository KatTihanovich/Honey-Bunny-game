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
            Time.timeScale = 1f;
            StartCoroutine(LoadSceneAsync(scene_id));
        }

        public void NextLevelWithoutUI(int scene_id)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene_id); 
        }

        IEnumerator LoadSceneAsync(int scene_id)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene_id);
            operation.allowSceneActivation = false; 

            LoadingScreen.SetActive(true);

            float displayedProgress = 0f;
            float targetProgress = 0f;

            while (!operation.isDone)
            {
                targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

                if (displayedProgress < targetProgress)
                {
                    displayedProgress += Time.deltaTime * 0.35f;
                    displayedProgress = Mathf.Min(displayedProgress, targetProgress);
                }

                LoadingBarFill.fillAmount = displayedProgress;

                if (operation.progress >= 0.9f && displayedProgress >= 1f)
                {
                    yield return new WaitForSeconds(0.3f);

                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }

    }
}