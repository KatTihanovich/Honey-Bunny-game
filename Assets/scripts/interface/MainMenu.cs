using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image backgroundImage;
    
    [Header("Backgrounds")]
    [SerializeField] private Sprite normalBackground;
    [SerializeField] private Sprite bossDefeatedBackground;

    [Header("Boss Defeat Status")]
    [SerializeField] private bool bossDefeated = false;  // Visible in Inspector

    private void Start()
    {
        // Load saved boss status
        bossDefeated = PlayerPrefs.GetInt("BossDefeated", 0) == 1;
        UpdateBackground();
    }

    public void SetBossDefeated(bool defeated)
    {
        bossDefeated = defeated;
        PlayerPrefs.SetInt("BossDefeated", defeated ? 1 : 0);
        PlayerPrefs.Save();
        UpdateBackground();
        Debug.Log($"BossDefeated set to: {defeated}");
    }

    private void UpdateBackground()
    {
        if (backgroundImage != null)
        {
            backgroundImage.sprite = bossDefeated ? bossDefeatedBackground : normalBackground;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // Change to your actual game scene
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MainMenu))]
public class MainMenuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MainMenu menu = (MainMenu)target;

        GUILayout.Space(10);

        if (GUILayout.Button("✅ Set Boss as Defeated"))
        {
            menu.SetBossDefeated(true);
        }

        if (GUILayout.Button("❌ Reset Boss Status"))
        {
            menu.SetBossDefeated(false);
        }
    }
}
#endif
