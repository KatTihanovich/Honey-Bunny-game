using UnityEngine;

public class PreSettingsScript : MonoBehaviour
{
    void Awake()
    {
        if (!PlayerPrefs.HasKey("HasInitialized"))
        {
            PlayerPrefs.SetInt("LevelUnlocked", 1);
            PlayerPrefs.SetInt("HasInitialized", 1);
            PlayerPrefs.Save();
        }
    }
    
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("LevelUnlocked");
        PlayerPrefs.DeleteKey("HasInitialized");
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs have been reset.");
    }
}
