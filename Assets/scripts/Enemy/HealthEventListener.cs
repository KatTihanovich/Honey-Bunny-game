using UnityEngine;
using UnityEngine.Audio;

public class HealthEventListener : MonoBehaviour
{
    private static readonly int GotHit = Animator.StringToHash("GotHit");
    private static readonly int Dead = Animator.StringToHash("Dead");
    [SerializeField] private Health health;
    [SerializeField] private Animator anim; // Animator доступен через инспектор
    private readonly bool canMove = true;
    private UltimateCooldown ultimateCooldown;

    private bool isDeadAnimationPlayed;

    [SerializeField] private AudioMixerGroup audioMixerGroup;
    public AudioClip deathSound;
    [SerializeField] private float volume = 1.0f;

    private void Start()
    {
        if (health != null)
        {
            health.OnHealthChanged += HandleHealthChanged; // Подпишемся на событие изменения здоровья
        }

        ultimateCooldown = GameObject.Find("UltimateCooldown").GetComponent<UltimateCooldown>();
    }

    private void HandleHealthChanged(float currentHealth)
    {
        if (currentHealth > 0)
        {
            Debug.Log("Health changed: " + currentHealth);
            anim.SetTrigger(GotHit);
        }
        else
        {
            if (isDeadAnimationPlayed) return;
            isDeadAnimationPlayed = true;
            anim.SetTrigger(Dead);
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        ultimateCooldown.AddPower();
        Play(deathSound);
        Debug.Log("Character is dead.");
        Invoke(nameof(DestroyObject), 2f);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
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