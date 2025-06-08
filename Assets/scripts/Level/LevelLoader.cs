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

        IEnumerator LoadSceneAsync(int scene_id)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene_id);
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