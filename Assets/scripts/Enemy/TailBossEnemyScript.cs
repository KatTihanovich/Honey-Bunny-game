using Game.Audio;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class TailBossEnemyScript : MonoBehaviour
    {
        private static readonly int AppearTrigger = Animator.StringToHash("Appear");
        private static readonly int DissapearTrigger = Animator.StringToHash("Dissapear");

        [Header("Boss")] public GameObject tailBoss;
        private Animator animator;
        private BoxCollider2D tailBossCollider2D;
        
        [Header("Boss portal")] public GameObject tailPortal;
        private Animator portalAnimator;
  

        private Health health;

        public float startY = -12.61f;
        public float targetY = -5.84f;
        public float duration = 0.2f;

        public float attackCooldownInterval = 2f;
        private float attackCooldownTimer;
        private bool playerInside;
        public float damage = 1f;

        private GameObject player;
        private HealthNew playerHealth;
        
        public GameObject blackHolder;
        private MeshRenderer meshRenderer;

        private HealthNew _health;

        private ISoundManager _soundManager;

        public void Start()
        {
            _soundManager = SoundManagerNew.Instance; 
            animator = tailBoss.GetComponent<Animator>();
            portalAnimator = tailPortal.GetComponent<Animator>();

            health = tailBoss.GetComponent<Health>();
            tailBossCollider2D = tailBoss.GetComponent<BoxCollider2D>();
            if (health != null)
            {
                health.OnHealthChanged += HandleHealthChanged;
            }

            player = FindFirstObjectByType<PlayerController>().gameObject;
            Debug.LogError("Find " + player);
            playerHealth = FindFirstObjectByType<HealthNew>();
           

            health = GetComponent<Health>();

            meshRenderer = tailBoss.GetComponent<MeshRenderer>();
            
            // Le показать нах
            StartCoroutine(MoveY(tailBoss, startY, targetY, duration));


            _health = transform.GetChild(0).GetComponent<HealthNew>();
            if (_health != null)
            {
                _health.OnDeath += HandleDeath;
                _health.OnDamageTaken += GetDamage;
            }
        }

        //Смерть 
        private void HandleDeath() 
        {
            animator.SetTrigger("Dead");
            HideOrKill();
        }

        //Получение урона
        private void GetDamage() 
        {
            animator.SetTrigger("GotHit");
            _soundManager.PlaySound("Damage");
        }


        private void Update()
        {
            attackCooldownTimer += Time.deltaTime;
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
            blackHolder.SetActive(true);
            meshRenderer.enabled = true;
            StartCoroutine(MoveY(tailBoss, startY, targetY, duration));
            portalAnimator.SetTrigger(AppearTrigger);
            tailBossCollider2D.enabled = true;
        }

        public void HideOrKill()
        {
            StopAllCoroutines();
            StartCoroutine(PortalDissapear());
            StartCoroutine(MoveY(tailBoss, targetY, startY, duration));
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
            portalAnimator.SetTrigger(DissapearTrigger);
            tailBossCollider2D.enabled = false;
            yield return new WaitForSeconds(1f);
            meshRenderer.enabled = false;
            blackHolder.SetActive(false);
        }

        // NOTE: Used by attack BoxCollider2D trigger!
        public void OnPlayerEntered()
        {
            if (attackCooldownTimer >= attackCooldownInterval)
            {
                attackCooldownTimer = 0f;
                print("ХРЯСЬ!");
                if (playerHealth)
                {
                    // Play(attackSound);
                    playerHealth.TakeDamage(damage);
                    Debug.Log("Player damaged by tail!");
                }
                else
                {
                    Debug.LogError("Player heath is null!");

                }
            }
        }
    }
}