using Game.Audio;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class TailBossEnemyScript : MonoBehaviour
    {
        private static readonly int AppearTrigger = Animator.StringToHash("Appearing");
        private static readonly int DissapearTrigger = Animator.StringToHash("Diappearing");

        [Header("Boss")]
        [SerializeField] private HealthNew _healtBossHead;
        public GameObject tailBoss;
        private Animator animator;
        private BoxCollider2D tailBossCollider2D;


        [Header("Boss Facing")]
        public Transform bossToFlip;

        public float startY = -12.61f;
        public float targetY = -5.84f;
        public float duration = 0.2f;

        public float attackCooldownInterval = 2f;
        private float attackCooldownTimer;
        private bool playerInside;

        public float damage = 1f;

        private GameObject player;
        private HealthNew playerHealth;

        private MeshRenderer meshRenderer;

        private HealthNew _health;

        private ISoundManager _soundManager;

        public void Start()
        {
            _soundManager = SoundManagerNew.Instance;
            animator = tailBoss.GetComponent<Animator>();

            tailBossCollider2D = tailBoss.GetComponent<BoxCollider2D>();

            player = FindFirstObjectByType<PlayerController>()?.gameObject;

            playerHealth = player.GetComponent<HealthNew>();

            meshRenderer = tailBoss.GetComponent<MeshRenderer>();

       
            StartCoroutine(MoveY(tailBoss, startY, targetY, duration));

            _health = GetComponent<HealthNew>();
            if (_health != null)
            {
                _health.OnDeath += HandleDeath;
                _health.OnDamageTaken += GetDamage;
            }
        }

        private void HandleDeath()
        {
            animator.SetTrigger("Dead");
            HideOrKill();
        }

        public void SetDie()
        {
            animator.SetTrigger("Diappearing");
            tailBossCollider2D.enabled = false;
        }

        private void GetDamage()
        {
            animator.SetTrigger("Damage");
            _soundManager.PlaySound("Damage");
            _healtBossHead.TakeDamage(5);
        }

        private void Update()
        {
            attackCooldownTimer += Time.deltaTime;

            if (playerInside && attackCooldownTimer >= attackCooldownInterval)
            {
                attackCooldownTimer = 0f;
                FacePlayer();
                animator.SetTrigger("Attack");
            }
        }

        private void HandleHealthChanged(float currentHealth)
        {
            Debug.Log("TAIL HP: " + currentHealth);

            if (currentHealth <= 0)
            {
                HideOrKill();
            }
        }

        public void RespawnOrAppear()
        {
            meshRenderer.enabled = true;
            transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
            StartCoroutine(MoveY(tailBoss, startY, targetY, duration));
            animator.SetTrigger(AppearTrigger);
            tailBossCollider2D.enabled = true;
        }

        public void HideOrKill()
        {
            StopAllCoroutines();
            StartCoroutine(PortalDissapear());
            //StartCoroutine(MoveY(tailBoss, targetY, startY, duration));
        }

        private static IEnumerator MoveY(GameObject target, float fromY, float toY, float time)
        {
            if (target == null) yield break;

            float elapsedTime = 0f;
            Vector3 startPosition =
                new Vector3(target.transform.localPosition.x, fromY, target.transform.localPosition.z);
            Vector3 targetPosition =
                new Vector3(target.transform.localPosition.x, toY, target.transform.localPosition.z);

            while (elapsedTime < time)
            {
                target.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            target.transform.localPosition = targetPosition;
        }

        private IEnumerator PortalDissapear()
        {
            animator.SetTrigger(DissapearTrigger);
            tailBossCollider2D.enabled = false;
            transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
            yield return new WaitForSeconds(1f);
            meshRenderer.enabled = false;
            GetComponent<HealthNew>().enabled = false;
        }

        private void FacePlayer()
        {
            if (player == null || bossToFlip == null) return;

            Vector3 rotation = bossToFlip.eulerAngles;

            if (player.transform.position.x < transform.position.x)
            {
                rotation.y = 0f;
            }
            else
            {
                rotation.y = 180f;
            }

            bossToFlip.eulerAngles = rotation;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInside = true;
                FacePlayer(); 
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInside = false;
            }
        }

        public void Attack()
        {
            Debug.Log("ХРЯСЬ!");
            _soundManager.PlaySound("TolikTallAttack");
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Player damaged by tail!");
            }
            else
            {
                Debug.LogError("Player health is null!");
            }
        }
    }
}
