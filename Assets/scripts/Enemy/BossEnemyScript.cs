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

        [Header("Attack Parameters")]
        public Animator animator;
        public float delay = 5f;
        public float attackCooldownInterval = 2f;
        public GameObject attackArea;
        public float damage = 1f;

        [Header("Boss portal")]
        public GameObject portal;
        private Animator portalAnimator;

        [Header("Tails objects")]
        public List<GameObject> tails;

        [Header("Hide and Recover Settings")]
        public float hideDuration = 5f;

        private Vector3 initialScale;
        private float attackCooldownTimer;
        private Coroutine attackCoroutine;
        private BoxCollider2D boxCollider;
        private int _currentDamage = 0;
        private bool isAlive = true;
        private bool isHiding = false;

        private HealthNew health;
        private GameObject player;
        private HealthNew playerHealth;
        private ISoundManager _soundManager;

        private void Start()
        {
            _soundManager = SoundManagerNew.Instance;

            if (animator == null)
                animator = GetComponent<Animator>();

            if (portal != null)
                portalAnimator = portal.GetComponent<Animator>();

            player = FindFirstObjectByType<PlayerController>().gameObject;
            playerHealth = player.GetComponent<HealthNew>();
            health = GetComponent<HealthNew>();
            boxCollider = GetComponent<BoxCollider2D>();
            initialScale = transform.localScale;

            if (health != null)
            {
                health.OnDeath += Die;
                health.OnDamageTaken += GetDamage;
            }

            attackCoroutine = StartCoroutine(AttackChainCoroutine());
        }

        private void Update()
        {
            attackCooldownTimer += Time.deltaTime;

            if (player != null && isAlive)
            {
                float deltaX = player.transform.position.x - transform.position.x;

                if (Mathf.Abs(deltaX) > 0.1f)
                {
                    Vector3 newScale = transform.localScale;

                    newScale.x = deltaX > 0 ? Mathf.Abs(initialScale.x) : -Mathf.Abs(initialScale.x);
                    transform.localScale = newScale;
                }
            }
        }

        public void GetDamage()
        {
            if (!isAlive || isHiding) return;

            Debug.Log("Босс получил урон");
            animator.SetBool("GotHit", true);
            StartCoroutine(ResetHitAnimation());

            _currentDamage++;

            if (_currentDamage >= 2)
            {
                _currentDamage = 0;

                if (attackCoroutine != null)
                    StopCoroutine(attackCoroutine);

                StartCoroutine(HideAndRecover());
            }
        }

        private IEnumerator ResetHitAnimation()
        {
            yield return new WaitForSeconds(0.3f);
            animator.SetBool("GotHit", false);
        }

        private void Die()
        {
            isAlive = false;

            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);

            StartCoroutine(PortalDissapear());

            PlayerPrefs.SetInt("BossDefeated", 1);
            PlayerPrefs.Save();

            animator.SetBool("Dead", true);
            _soundManager.PlaySound("BossDie");
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

        private IEnumerator HideAndRecover()
        {
            isHiding = true;

            yield return StartCoroutine(PerformDisappearance());

            Debug.Log("Босс прячется после получения урона");
            yield return new WaitForSeconds(hideDuration);

            yield return StartCoroutine(PerformAppearance());

            isHiding = false;

            if (isAlive)
                attackCoroutine = StartCoroutine(AttackChainCoroutine());
        }

        private IEnumerator AttackChainCoroutine()
        {
            while (isAlive)
            {
                for (int i = 0; i < 3; i++)
                {
                    yield return StartCoroutine(PerformAppearance());
                    yield return StartCoroutine(PerformAttack());
                    yield return StartCoroutine(PerformDisappearance());

                    if (player != null)
                    {
                        transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
                    }
                }

                Debug.Log("10 секунд на атаку по площади");

                foreach (var tail in tails)
                {
                    tail.GetComponent<TailBossEnemyScript>().RespawnOrAppear();
                    tail.transform.GetComponentInChildren<HealthNew>().RestoreFull();
                }

                yield return new WaitForSeconds(10f);
            }
        }

        private IEnumerator PerformAppearance()
        {
            _soundManager.PlaySound("Portal");

            animator.SetTrigger(AppearTrigger);
            yield return new WaitForSeconds(0.5f);

            if (attackArea)
            {
                attackArea.SetActive(true);
                boxCollider.enabled = true;
            }
        }

        private IEnumerator PerformAttack()
        {
            yield return new WaitForSeconds(delay);
        }

        private IEnumerator PerformDisappearance()
        {
            if (attackArea)
            {
                attackArea.SetActive(false);
                boxCollider.enabled = false;
            }

            _soundManager.PlaySound("Portal");
            animator.SetTrigger(HideTrigger);
            yield return new WaitForSeconds(1f);
        }

        public void OnPlayerEntered()
        {
            if (isAlive && attackCooldownTimer >= attackCooldownInterval)
            {
                animator.SetTrigger(AttackTrigger);
                attackCooldownTimer = 0f;
            }
        }

        public void HIT_BITE()
        {
            Debug.Log("КУСЬ!");
            if (playerHealth)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Player damaged by enemy!");
            }
            else
            {
                Debug.LogError("Player health is null!");
            }
        }
    }
}
