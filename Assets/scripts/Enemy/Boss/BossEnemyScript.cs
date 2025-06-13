using Game.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class BossEnemyScript : MonoBehaviour
    {
        private static readonly int GotHitTrigger = Animator.StringToHash("GotHit");
        private static readonly int HideTrigger = Animator.StringToHash("Hide");
        private static readonly int AppearTrigger = Animator.StringToHash("Appear");
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int DeadTrigger = Animator.StringToHash("Dead");

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private GameObject attackArea;
        [SerializeField] private TailBossEnemyScript[] tails;

        [Header("Settings")]
        [SerializeField] private float appearDuration = 0.9f;
        [SerializeField] private float disappearDuration = 0.9f;
        [SerializeField] private float activePhaseDuration = 5f;
        [SerializeField] private float hiddenDuration = 10f;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private float attackDamage = 1f;

        private Renderer bossRenderer;
        private Vector3 initialScale;
        private float activeTimer;
        private float attackTimer;

        private int damageTakenThisPhase = 0;
        private int hitsDoneThisPhase = 0;

        private bool isAlive = true;
        private bool isVisible = false;
        private bool canTakeDamage = true;

        private HealthNew health;
        private HealthNew playerHealth;
        private GameObject player;
        private ISoundManager soundManager;

        private Coroutine phaseRoutine;
        private BossAttackArea _bossAttackArea;

        private void Awake()
        {
            animator = animator ? animator : GetComponent<Animator>();
            boxCollider = boxCollider ? boxCollider : GetComponent<BoxCollider2D>();
            health = GetComponent<HealthNew>();
            bossRenderer = GetComponentInChildren<Renderer>();
            initialScale = transform.localScale;
            _bossAttackArea = GetComponentInChildren<BossAttackArea>();

            
                tails = FindObjectsOfType<TailBossEnemyScript>();
                Debug.Log($"Auto-assigned {tails.Length} tails");
           
        }

     
        private void Start()
        {
            soundManager = SoundManagerNew.Instance;
            player = FindFirstObjectByType<PlayerController>()?.gameObject;
            playerHealth = player?.GetComponent<HealthNew>();

            health.OnDamageTaken += TakeDamage;
            health.OnDeath += Die;

            phaseRoutine = StartCoroutine(PhaseCycle());
        }

        private void Update()
        {
            if (!isAlive || !isVisible || player == null) return;

            // Поворот к игроку
            float deltaX = player.transform.position.x - transform.position.x;
            if (Mathf.Abs(deltaX) > 0.1f)
            {
                Vector3 newScale = initialScale;
                newScale.x = deltaX > 0 ? Mathf.Abs(initialScale.x) : -Mathf.Abs(initialScale.x);
                transform.localScale = newScale;
            }

            // Активная фаза
            activeTimer += Time.deltaTime;
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackCooldown & _bossAttackArea.PlayerInside)
            {
                animator.SetTrigger(AttackTrigger);
                attackTimer = 0f;
            }

            if (damageTakenThisPhase >= 2 || hitsDoneThisPhase >= 2 || activeTimer >= activePhaseDuration)
            {
                if (phaseRoutine != null)
                {
                    StopCoroutine(phaseRoutine);
                }
                phaseRoutine = StartCoroutine(DisappearPhase());
            }
        }

        private void KillAllTail() 
        {
     
            foreach (TailBossEnemyScript tail in tails) 
            {
                if (tail != null)
                {
                    tail.SetDie();
                }
      
            }
        }

        private void TakeDamage()
        {
            if (!isAlive || !canTakeDamage || !isVisible) return;

            damageTakenThisPhase++;
            animator.SetTrigger(GotHitTrigger);
        }

        public void HIT_BITE()
        {
            if (!isAlive || !isVisible) return;

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                hitsDoneThisPhase++;
                Debug.Log("Игрок получил урон!");
            }
            else
            {
                Debug.LogError("PlayerHealth не найден!");
            }
        }

        private IEnumerator PhaseCycle()
        {
            while (isAlive)
            {
                yield return StartCoroutine(AppearPhase());
                yield return new WaitUntil(() =>
                    damageTakenThisPhase >= 2 || hitsDoneThisPhase >= 2 || activeTimer >= activePhaseDuration
                );
                yield return StartCoroutine(DisappearPhase());
            }
        }

        private IEnumerator AppearPhase()
        {
            damageTakenThisPhase = 0;
            hitsDoneThisPhase = 0;
            activeTimer = 0f;
            attackTimer = 0f;

            if (player != null)
            {
                transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
            }

            bossRenderer.enabled = true;
            boxCollider.enabled = true;
            attackArea.SetActive(true);

            animator.SetTrigger(AppearTrigger);
            soundManager?.PlaySound("Portal");
            yield return new WaitForSeconds(appearDuration);

            isVisible = true;
            canTakeDamage = true;
            Debug.Log("Босс появился");
        }

        private IEnumerator DisappearPhase()
        {
            isVisible = false;
            canTakeDamage = false;

            animator.SetTrigger(HideTrigger);
            soundManager?.PlaySound("Portal");
            Debug.Log("Босс исчезает");

            yield return new WaitForSeconds(disappearDuration);

            bossRenderer.enabled = false;
            boxCollider.enabled = false;
            attackArea.SetActive(false);

            yield return new WaitForSeconds(hiddenDuration);

            phaseRoutine = StartCoroutine(PhaseCycle());
        }

        private void Die()
        {
            isAlive = false;
            KillAllTail();
            canTakeDamage = false;
            isVisible = false;

            if (phaseRoutine != null)
            {
                StopCoroutine(phaseRoutine);
                phaseRoutine = null;
            }

            animator.SetTrigger(DeadTrigger);
            soundManager?.PlaySound("BossDie");
            PlayerPrefs.SetInt("BossDefeated", 1);
            PlayerPrefs.Save();
   
        }

        public void OnPlayerEntered() 
        {
        
        }
    }
}
