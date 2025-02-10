using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video; 
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace Enemy
{
    public class BossEnemyScript : MonoBehaviour
    {
        private static readonly int HideTrigger = Animator.StringToHash("Hide");
        private static readonly int AppearTrigger = Animator.StringToHash("Appear");
        private static readonly int DissapearTrigger = Animator.StringToHash("Dissapear");
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");

        [Header("Attack Parameters")] public Animator animator; // Ссылка на аниматор
        public float delay = 5f; // Задержка в секундах перед запуском анимации
        private Vector3 initialScale;
        public float attackCooldownInterval = 2f;
        private float attackCooldownTimer;
        private BoxCollider2D boxCollider;
        public GameObject attackArea;
        public float damage = 1f;

        private Coroutine attackCoroutine;
        private int attackCount;

        private Health health;

        private GameObject player;
        private Health playerHealth;
        

        [Header("Boss portal")] public GameObject portal;
        private Animator portalAnimator;

        private bool isAlive = true;

        [SerializeField] private AudioMixerGroup audioMixerGroup; 
        public AudioClip portalSound;
        [SerializeField] private float volume = 1.0f;
        
        [Header("Tails objects")] 
        public List<GameObject> tails;

        [Header("Cutscene")]
        public VideoPlayer cutsceneVideo;
        public GameObject UICanvas;
        public Image fadePanel; 
        public float fadeDuration = 1.5f;

        private void Start()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if (portal != null)
            {
                portalAnimator = portal.GetComponent<Animator>();
            }

            player = GameObject.Find("Bunny");
            if (player != null)
            {
                playerHealth = player.GetComponent<Health>();
            }
            else
            {
                Debug.LogError("Player is null!");
            }

            health = GetComponent<Health>();

            boxCollider = GetComponent<BoxCollider2D>();

            if (health != null)
            {
                health.OnHealthChanged += HandleHealthChanged; // Подпишемся на событие изменения здоровья
            }

            initialScale = transform.localScale;
            attackCoroutine = StartCoroutine(AttackChainCoroutine());
        }

        private void Update()
        {
            attackCooldownTimer += Time.deltaTime;

            if (player != null && isAlive)
            {
                float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
                transform.localScale = new Vector3(initialScale.x * -direction, initialScale.y, initialScale.z);
            }
        }

        private void HandleHealthChanged(float currentHealth)
        {
            Debug.Log("BOSS HP: " + currentHealth);

            if (currentHealth <= 0)
            {
                isAlive = false;
                StopCoroutine(attackCoroutine);
                StartCoroutine(PortalDissapear());
                // Включение кат-сцены после смерти босса
                                // Запускаем затемнение и кат-сцену
                if (cutsceneVideo != null && fadePanel != null)
                {
                    StartCoroutine(PlayCutscene());
                }
                else
                {
                    Debug.LogError("VideoPlayer или fadePanel не назначены!");
                }
            }
        }
      
    private IEnumerator PlayCutscene()
{
    // Smooth fade to black
    float elapsedTime = 0f;
    Color panelColor = fadePanel.color;
    
    while (elapsedTime < fadeDuration)
    {
        panelColor.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
        fadePanel.color = panelColor;
        elapsedTime += Time.deltaTime;
        yield return null;
    }
    
    panelColor.a = 1f;
    fadePanel.color = panelColor;

    // Enable and play video
    if (cutsceneVideo != null)
    {
        cutsceneVideo.gameObject.SetActive(true);
        cutsceneVideo.Prepare();
        yield return new WaitUntil(() => cutsceneVideo.isPrepared);
        
        UICanvas.SetActive(false);
        cutsceneVideo.Play();

        // Wait for video to finish, with a timeout
        float videoTimeout = 15f; // Set based on your video length
        elapsedTime = 0f;

        while (cutsceneVideo.isPlaying && elapsedTime < videoTimeout)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Stop video if still playing
        cutsceneVideo.Stop();
    }
    else
    {
        Debug.LogError("VideoPlayer is not assigned!");
    }

    // Load the main menu
    SceneManager.LoadScene("MainMenu"); // Replace with actual scene name or index
}

        
        private IEnumerator PortalDissapear()
        {
            portalAnimator.SetTrigger(DissapearTrigger);
            foreach (var tail in tails)
            {
                Play(portalSound);
                tail.GetComponent<TailBossEnemyScript>().HideOrKill();
            }
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator AttackChainCoroutine()
        {
            while (isAlive)
            {
                for (var i = 0; i < 3; i++)
                {
                    Play(portalSound);
                    portalAnimator.SetTrigger(AppearTrigger);
                    animator.SetTrigger(AppearTrigger);
                    yield return new WaitForSeconds(0.5f);
                    if (attackArea)
                    {
                        attackArea.SetActive(true);
                        boxCollider.enabled = true;
                    }

                    yield return new WaitForSeconds(delay);
                    if (isAlive)
                    {
                        if (attackArea)
                        {
                            attackArea.SetActive(false);
                            boxCollider.enabled = false;
                        }

                        Play(portalSound);
                        animator.SetTrigger(HideTrigger);
                        yield return new WaitForSeconds(1f);
                        portalAnimator.SetTrigger(DissapearTrigger);
                        yield return new WaitForSeconds(1f);
                        if (player != null)
                        {
                            transform.position = new Vector3(player.transform.position.x, transform.position.y,
                                transform.position.z);
                        }
                    }
                }

                Debug.Log("10 секунд на атаку по площади");
                foreach (var tail in tails)
                {
                    tail.GetComponent<TailBossEnemyScript>().RespawnOrAppear();
                }
                yield return new WaitForSeconds(10);
            }
        }

        // NOTE: Used by attack BoxCollider2D trigger!
        public void OnPlayerEntered()
        {
            if (isAlive && attackCooldownTimer >= attackCooldownInterval)
            {
                animator.SetTrigger(AttackTrigger);
                attackCooldownTimer = 0f;
            }
        }

        // NOTE: Used by Animator to provide attack!
        public void HIT_BITE()
        {
            print("КУСЬ!");
            if (playerHealth)
            {
                // Play(attackSound);
                playerHealth.TakeDamage(damage);
                Debug.Log("Player damaged by enemy!");
            }
            else
            {
                Debug.LogError("Player heath is null!");
            }
        }
        private void Play(AudioClip clip) {
            if (clip != null && audioMixerGroup != null) {
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

}