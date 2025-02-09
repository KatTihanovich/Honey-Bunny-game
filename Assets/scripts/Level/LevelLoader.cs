using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level
{
    public class LevelLoader : MonoBehaviour
    {
        public int nextLevelIndex;

        public void NextLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadSceneAsync(nextLevelIndex);
        }
    }
}