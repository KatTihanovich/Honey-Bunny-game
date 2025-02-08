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
        private Health playerHealth;

        public void Start()
        {
            animator = tailBoss.GetComponent<Animator>();
            portalAnimator = tailPortal.GetComponent<Animator>();

            health = tailBoss.GetComponent<Health>();

            if (health != null)
            {
                health.OnHealthChanged += HandleHealthChanged;
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

            // Le показать нах
            StartCoroutine(MoveY(tailBoss, startY, targetY, duration));
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
                // Le спрятать нах
                HideOrKill();
            }
        }

        public void RespawnOrAppear()
        {
            StartCoroutine(MoveY(tailBoss, startY, targetY, duration));
            portalAnimator.SetTrigger(AppearTrigger);
        }

        public void HideOrKill()
        {
            portalAnimator.SetTrigger(DissapearTrigger);
            StartCoroutine(MoveY(tailBoss, targetY, startY, duration));
            StartCoroutine(PortalDissapear());
        }

        private static IEnumerator MoveY(GameObject target, float fromY, float toY, float time)
        {
            if (target == null) yield break;

            yield return new WaitForSeconds(1f);

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
            yield return new WaitForSeconds(1f);
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