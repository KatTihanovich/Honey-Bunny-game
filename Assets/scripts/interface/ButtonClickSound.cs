using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private float volume = 1.0f;

    public void PlayClickSound()
    {
        if (buttonSound != null)
        {
            AudioSource.PlayClipAtPoint(buttonSound, transform.position, volume);
        }
    }
}
