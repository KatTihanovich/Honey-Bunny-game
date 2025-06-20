using UnityEngine;
using UnityEngine.SceneManagement;

public class ResumeManager : MonoBehaviour
{
    public static ResumeManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string targetScene;

    public void ResumeGame(string sceneName)
    {
        targetScene = sceneName;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Level.LevelLoader loader = FindAnyObjectByType<Level.LevelLoader>();
        if (loader != null)
        {
            loader.LoadSceneByName(sceneName);
        }
        else
        {
            Debug.LogError("LevelLoader not found! Can't load scene with loading screen.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!CheckpointManager.HasCheckpoint(targetScene))
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Vector3 checkpointPosition = CheckpointManager.GetCheckpoint(targetScene);
            player.transform.position = checkpointPosition;
            Debug.Log("Player resumed at: " + checkpointPosition);
            HealthNew playerHealth = player.GetComponent<HealthNew>();
            if (playerHealth != null)
            {
                playerHealth.RestoreFull();
                Debug.Log("Player health restored.");
            }
            else
            {
                Debug.LogWarning("HealthNew component not found on player.");
            }
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}