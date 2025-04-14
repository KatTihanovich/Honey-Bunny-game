using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float startingHealth;
    public float CurrentHealth { get; private set; }
    private bool isDead = false;

    public GameObject deathCanvas;
    public GameObject toSelect;

    public event System.Action<float> OnHealthChanged;

    [SerializeField] private AudioMixerGroup audioMixerGroup;
    public AudioClip damageSound;
    [SerializeField] private float volume = 1.0f;


    private void Awake()
    {
        CurrentHealth = startingHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // If already dead, no further damage can be taken

        // Handheld.Vibrate();

        // Reduce health and invoke the health changed event
        Play(damageSound);
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, startingHealth);
        if (CurrentHealth == 0 && deathCanvas != null)
        {
            EventSystem.current.SetSelectedGameObject(toSelect);
            deathCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public void AddHealth(float value)
    {
        if (isDead) return; // No health can be added if the character is dead

        // Increase health and invoke the health changed event
        CurrentHealth = Mathf.Clamp(CurrentHealth + value, 0, startingHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }
    public void Respawn()
    {
        CurrentHealth = startingHealth;
        isDead = false;
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    private void Play(AudioClip clip)
    {
        if (clip != null && audioMixerGroup != null)
        {
            GameObject tempAudio = new GameObject("TempAudioClip");
            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();

            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();

            Destroy(tempAudio, clip.length);
        }
    }
}