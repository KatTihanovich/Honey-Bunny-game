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
    private void OnDestroy()
{
    if (health != null)
    {
        health.OnHealthChanged -= HandleHealthChanged;
    }
}

    // private void HandleHealthChanged(float currentHealth)
    // {
    //     if (currentHealth > 0)
    //     {
    //         Debug.Log("Health changed: " + currentHealth);
    //         anim.SetTrigger(GotHit);
    //     }
    //     else
    //     {
    //         //if (isDeadAnimationPlayed) return;
    //         isDeadAnimationPlayed = true;
    //         anim.SetTrigger(Dead);
    //         HandleDeath();
    //     }
    // }

    private void HandleHealthChanged(float currentHealth)
{
    if (isDeadAnimationPlayed) return; // ✅ Добавляем проверку: если персонаж уже мёртв, игнорируем

    if (currentHealth > 0)
    {
        Debug.Log("Health changed: " + currentHealth);
        anim.SetTrigger(GotHit);
    }
    else
    {
       // if (currentHealth <= 0) 
          Debug.Log("Character is dead.");
        anim.SetTrigger(Dead);
        isDeadAnimationPlayed = true;
        HandleDeath();
    }

}


    private void HandleDeath()
    {
        ultimateCooldown.AddPower();
        Play(deathSound);
        Debug.Log("Character is dead.");

        if (gameObject.CompareTag("Enemy")) 
        {
            EndWindow.IncreaseEnemyCount();
        }
        Invoke(nameof(DestroyObject), 1f);
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