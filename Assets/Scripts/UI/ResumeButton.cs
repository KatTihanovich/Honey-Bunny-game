using UnityEngine;
using UnityEngine.UI;


public class ResumeButton : MonoBehaviour
{
    public string sceneName;
    public Button resumeButton; 

    private void Start()
    {
        resumeButton.onClick.AddListener(() => ResumeLevel());

        if (!CheckpointManager.HasCheckpoint(sceneName))
        {
            resumeButton.gameObject.SetActive(false);
        }
    }

    private void ResumeLevel()
    {
        ResumeManager.Instance.ResumeGame(sceneName);
    }
}