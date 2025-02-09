using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level
{
    public class LevelLoader : MonoBehaviour
    {
        public int nextLevelIndex;

        public void NextLevel()
        {
            SceneManager.LoadSceneAsync(nextLevelIndex);
        }
    }
}