using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private Dictionary<string, Vector3> sceneCheckpoints = new Dictionary<string, Vector3>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadCheckpoints(); // Load saved data when game starts
    }

    public static bool HasCheckpoint(string sceneName)
    {
        return Instance != null && Instance.sceneCheckpoints.ContainsKey(sceneName);
    }

    public static Vector3 GetCheckpoint(string sceneName)
    {
        return Instance.sceneCheckpoints.TryGetValue(sceneName, out var pos) ? pos : Vector3.zero;
    }

    public static void SetCheckpoint(string sceneName, Vector3 position)
    {
        if (Instance == null) return;

        Instance.sceneCheckpoints[sceneName] = position;
        Instance.SaveCheckpoint(sceneName, position);
    }

    private void SaveCheckpoint(string sceneName, Vector3 position)
    {
        PlayerPrefs.SetFloat(sceneName + "_x", position.x);
        PlayerPrefs.SetFloat(sceneName + "_y", position.y);
        PlayerPrefs.SetFloat(sceneName + "_z", position.z);
        PlayerPrefs.Save();
    }

    private void LoadCheckpoints()
    {

        // Loop through known scenes, or use saved scene names in future version
        string[] scenes = { "TestSceneSave", "NewCaveLevel" }; // Replace with your actual scenes
        foreach (string sceneName in scenes)
        {
            if (PlayerPrefs.HasKey(sceneName + "_x"))
            {
                float x = PlayerPrefs.GetFloat(sceneName + "_x");
                float y = PlayerPrefs.GetFloat(sceneName + "_y");
                float z = PlayerPrefs.GetFloat(sceneName + "_z");
                sceneCheckpoints[sceneName] = new Vector3(x, y, z);
            }
        }
    }
}