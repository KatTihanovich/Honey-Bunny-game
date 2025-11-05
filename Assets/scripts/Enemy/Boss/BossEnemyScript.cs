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
        private static readonly int RoarTrigger = Animator.StringToHash("Roar");

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

        [Header("ROAR Settings")]
        [SerializeField] private GameObject[] listEnemy;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float roarDuration = 30f;
        [SerializeField] private bool useRoarPhase = true;
        private float roarHealthThreshold = 150f;
        private bool hasRoared = false;

        [Header("Spawn Settings")]
        [SerializeField] private Transform spawnPoint;

        private Renderer bossRenderer;
        private Vector3 initialScale;
        private float activeTimer;
        private float attackTimer;

        private int damageTakenThisPhase = 0;
        private int hitsDoneThisPhase = 0;

        private bool isAlive = true;
        private bool isVisible = false;
        private bool canTakeDamage = true;
        private bool isRoaring = false;

        private HealthNew health;
        private HealthNew playerHealth;
        private GameObject player;
        private ISoundManager soundManager;

        private Coroutine phaseRoutine;
        private BossAttackArea _bossAttackArea;

        private bool isFirstAppear = true;

        private void Awake()
        {
            animator = animator ? animator : GetComponent<Animator>();
            boxCollider = boxCollider ? boxCollider : GetComponent<BoxCollider2D>();
            health = GetComponent<HealthNew>();
            bossRenderer = GetComponentInChildren<Renderer>();
            initialScale = transform.localScale;
            _bossAttackArea = GetComponentInChildren<BossAttackArea>();
            tails = FindObjectsOfType<TailBossEnemyScript>();
        }

        private void Start()
        {
            soundManager = SoundManagerNew.Instance;
            player = FindFirstObjectByType<PlayerController>()?.gameObject;
            playerHealth = player?.GetComponent<HealthNew>();

            health.OnDamageTaken += TakeDamage;
            health.OnDeath += Die;

            phaseRoutine = StartCoroutine(PhaseCycle());

            if (useRoarPhase && health.CurrentHealth <= roarHealthThreshold)
            {
                hasRoared = true;
                Debug.Log("ROAR отключён на старте: здоровье уже ниже порога.");
            }
        }

        private void Update()
        {
            if (!isAlive || !isVisible || player == null || isRoaring) return;

            float deltaX = player.transform.position.x - transform.position.x;
            if (Mathf.Abs(deltaX) > 0.1f)
            {
                Vector3 newScale = initialScale;
                newScale.x = deltaX > 0 ? Mathf.Abs(initialScale.x) : -Mathf.Abs(initialScale.x);
                transform.localScale = newScale;
            }

            activeTimer += Time.deltaTime;
            attackTimer += Time.deltaTime;

            if (!isRoaring && attackTimer >= attackCooldown && _bossAttackArea.PlayerInside)
            {
                animator.SetTrigger(AttackTrigger);
                attackTimer = 0f;
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
            if (!isAlive || !canTakeDamage || !isVisible || isRoaring)
            {
                Debug.Log($"[TakeDamage] Условие не выполнено. isAlive={isAlive}, canTakeDamage={canTakeDamage}, isVisible={isVisible}, isRoaring={isRoaring}");
                return;
            }

            damageTakenThisPhase++;
            soundManager.PlaySound("Damage");
            animator.SetTrigger(GotHitTrigger);
            Debug.Log($"[TakeDamage] Урон принят. Текущее здоровье: {health.CurrentHealth}, Уронов за фазу: {damageTakenThisPhase}");

            // Запустить ROAR, если здоровье босса ниже порога и он ещё не рычал
            if (useRoarPhase && !hasRoared && health.CurrentHealth <= roarHealthThreshold)
            {
                Debug.Log($"[TakeDamage] Условия Roar выполнены. useRoarPhase={useRoarPhase}, hasRoared={hasRoared}, health={health.CurrentHealth} <= threshold={roarHealthThreshold}");

                hasRoared = true;

                if (phaseRoutine != null)
                {
                    StopCoroutine(phaseRoutine);
                    Debug.Log("[TakeDamage] Остановлен старый phaseRoutine перед Roar");
                }

                phaseRoutine = StartCoroutine(RoarPhase());
            }
            else
            {
                Debug.Log($"[TakeDamage] Условия Roar НЕ выполнены. useRoarPhase={useRoarPhase}, hasRoared={hasRoared}, health={health.CurrentHealth}");
            }
        }



        public void HIT_BITE()
        {
            if (!isAlive || !isVisible || isRoaring)
            {
                Debug.LogWarning("[HIT_BITE] Атака отменена: isAlive=" + isAlive + ", isVisible=" + isVisible + ", isRoaring=" + isRoaring);
                return;
            }

            if (player == null || playerHealth == null)
            {
                Debug.LogError("[HIT_BITE] Игрок или его здоровье не найдены!");
                return;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer > 2.5f)
            {
                Debug.LogWarning($"[HIT_BITE] Игрок вне радиуса атаки ({distanceToPlayer:F2}м > {2.5f}м)");
                return;
            }

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            playerHealth.TakeDamage(attackDamage);
            soundManager.PlaySound("TolikAttack");
            hitsDoneThisPhase++;

            Debug.Log(
                $"[HIT_BITE] УДАР ПРОИЗОШЁЛ!\n" +
                $"- Анимация: {stateInfo.shortNameHash} (time: {stateInfo.normalizedTime:F2})\n" +
                $"- Игрок: {player.name}, HP: {playerHealth.CurrentHealth}, Расстояние: {distanceToPlayer:F2}м\n" +
                $"- Удары в этой фазе: {hitsDoneThisPhase}"
            );
        }



        private IEnumerator PhaseCycle()
        {
            while (isAlive)
            {
                yield return StartCoroutine(AppearPhase());
                yield return new WaitUntil(() =>
                    damageTakenThisPhase >= 2 || hitsDoneThisPhase >= 2 || activeTimer >= activePhaseDuration
                );

                if (useRoarPhase && !hasRoared && health.CurrentHealth <= roarHealthThreshold)
                {
                    hasRoared = true;
                    yield return StartCoroutine(RoarPhase());
                }

                yield return StartCoroutine(DisappearPhase());
            }
        }

        private IEnumerator AppearPhase()
        {
            damageTakenThisPhase = 0;
            hitsDoneThisPhase = 0;
            activeTimer = 0f;
            attackTimer = 0f;

            if (isFirstAppear)
            {
                if (spawnPoint != null)
                {
                    transform.position = spawnPoint.position;
                }
                isFirstAppear = false;
            }
            else
            {
                if (player != null)
                {
                    transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
                }
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

        private IEnumerator RoarPhase()
        {
            Debug.Log($"[RoarPhase] Старт. isRoaring={isRoaring}, canTakeDamage={canTakeDamage}, isVisible={isVisible}");

            isRoaring = true;
            health.enabled = false;
            canTakeDamage = false;
            isVisible = true;

            animator.ResetTrigger(AttackTrigger);
            animator.Play("Idle", 0); 
            animator.SetTrigger(RoarTrigger);
            //soundManager?.PlaySound("TolikRoar");

            yield return StartCoroutine(SpawnEnemiesDuringRoar());

            isRoaring = false;
            health.enabled = true;

           
            animator.SetTrigger(HideTrigger);
            yield return new WaitForSeconds(1f);


            Debug.Log("[RoarPhase] Конец фазы Roar");

            if (isAlive)
            {
                phaseRoutine = StartCoroutine(PhaseCycle());
            }
        }

        public void PlayRoar()
        {
            soundManager.PlaySound("TolikRoar");
        }

        private IEnumerator SpawnEnemiesDuringRoar()
        {
            if (listEnemy.Length == 0 || spawnPoints.Length == 0)
            {
                Debug.LogWarning("Не заданы враги или точки спавна!");
                yield break;
            }

            float interval = 8f;
            float elapsed = 0f;
            int spawnedCount = 0;
            int maxEnemies = 6;

            while (elapsed < roarDuration && spawnedCount < maxEnemies)
            {
                GameObject enemyPrefab = listEnemy[Random.Range(0, listEnemy.Length)];
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                Debug.Log($"Спавн врага {enemyPrefab.name} в точке {spawnPoint.name}");

                spawnedCount++;
                yield return new WaitForSeconds(interval);
                elapsed += interval;
            }


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
            player.GetComponent<PlayerController>().enabled = false;
        }
    }
}
