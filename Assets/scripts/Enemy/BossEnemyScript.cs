using Game.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


namespace Enemy
{
    public class BossEnemyScript : MonoBehaviour
    {
        private static readonly int HideTrigger = Animator.StringToHash("Hide");
        private static readonly int AppearTrigger = Animator.StringToHash("Appear");
        private static readonly int DissapearTrigger = Animator.StringToHash("Dissapear");
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");

        [Header("Attack Parameters")] public Animator animator;
        public float delay = 5f; // Задержка в секундах перед запуском анимации
        private Vector3 initialScale;
        public float attackCooldownInterval = 2f;
        private float attackCooldownTimer;
        private BoxCollider2D boxCollider;
        public GameObject attackArea;
        public float damage = 1f;

        private Coroutine attackCoroutine;
        private int attackCount;

        private HealthNew health;

        private GameObject player;
        private HealthNew playerHealth;

        private int _currentDamage=0;

        [Header("Boss portal")] public GameObject portal;
        private Animator portalAnimator;

        private bool isAlive = true;

        private ISoundManager _soundManager;

        [Header("Tails objects")]
        public List<GameObject> tails;


        private void Start()
        {
            _soundManager = SoundManagerNew.Instance;

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if (portal != null)
            {
                portalAnimator = portal.GetComponent<Animator>();
            }

            player = GameObject.Find("Bunny");
            playerHealth = FindFirstObjectByType<HealthNew>();
            if (player != null)
            {

            }
            else
            {
                Debug.LogError("Player is null!");
            }

            health = GetComponent<HealthNew>();

            boxCollider = GetComponent<BoxCollider2D>();

            if (health != null)
            {
              
                health.OnDeath += Die;
                health.OnDamageTaken += GetDamage;
            }

            initialScale = transform.localScale;
            attackCoroutine = StartCoroutine(AttackChainCoroutine());
        }

        public void GetDamage()
        {
            Debug.Log("Босс получил урон");
            animator.SetBool("GotHit", true);
            _currentDamage++;
        }

      
        private void Die()
        {
            isAlive = false;
            StopCoroutine(attackCoroutine);
            StartCoroutine(PortalDissapear());
            PlayerPrefs.SetInt("BossDefeated", 1);
            PlayerPrefs.Save();
            animator.SetBool("Dead",true);
            _soundManager.PlaySound("BossDie");
        }
        private void Update()
        {
            attackCooldownTimer += Time.deltaTime;

            if (player != null && isAlive)
            {
                float deltaX = player.transform.position.x - transform.position.x;

            
                if (Mathf.Abs(deltaX) > 0.1f)
                {
                    float direction = Mathf.Sign(deltaX);
                    transform.localScale = new Vector3(initialScale.x * -direction, initialScale.y, initialScale.z);
                }
            }
        }





        private IEnumerator PortalDissapear()
        {
            portalAnimator.SetTrigger(DissapearTrigger);
            foreach (var tail in tails)
            {
                _soundManager.PlaySound("Portal");
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
                    _soundManager.PlaySound("Portal");
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

                        _soundManager.PlaySound("Portal");
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
                    tail.transform.GetComponentInChildren<HealthNew>().RestoreFull();
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
       
    }

}