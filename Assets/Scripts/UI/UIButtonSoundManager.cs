using UnityEngine;
using UnityEngine.UI;

public class UIButtonSoundManager : MonoBehaviour
{
    [Header("Audio Source for all buttons")]
    public AudioSource audioSource;

    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    public void PlayClickSound()
    {
        if (audioSource != null)
        {
            audioSource.Play(); 
        }
    }
}