using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Level
{
    public class LevelLoader : MonoBehaviour
    {
        public int nextLevelIndex;
        public GameObject LoadingScreen;
        public Image LoadingBarFill;

        public void NextLevel()
        {
            Time.timeScale = 1f;
            StartCoroutine(LoadSceneAsync());
        }

        IEnumerator LoadSceneAsync()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(nextLevelIndex);
            LoadingScreen.SetActive(true);

            while (!operation.isDone)
            {
                float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
                LoadingBarFill.fillAmount = progressValue;
                yield return null;
            }
        }
    }
}