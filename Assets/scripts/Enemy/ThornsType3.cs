using UnityEngine;
using Game.Audio;

public class SpikeTraps : MonoBehaviour
{
    public Animator animator;
    public int damage = 1;
    public float attackInterval = 2f;
    public float wakeHealthThreshold = 70f;

    private float timer = 0f;
    private bool playerInside = false;
    private GameObject playerInTrigger;
    private ISoundManager _soundManager;

    private HealthNew playerHealth;   // глобальная ссылка на игрока
    private bool isAngry = false;     // false = Idle Sleep (Metro = true), true = Idle Angry (Metro = false)

    private void Start()
    {
        _soundManager = SoundManagerNew.Instance;

        // Найдём игрока сразу (как в MonsterAI)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<HealthNew>();
            // начальное состояние аниматора в зависимости от здоровья игрока
            if (animator != null && playerHealth != null)
                animator.SetBool("Mad", playerHealth.CurrentHealth < wakeHealthThreshold);
        }
        else
        {
            // если игрока нет — ставим в спящий (по безопасности)
            if (animator != null)
                animator.SetBool("Mad", false);
        }
    }

    private void Update()
    {
        // Если нет игрока — ничего не делаем
        if (playerHealth == null) return;

        // Обновляем состояние (аналогично MonsterAI)
        bool shouldBeAngry = playerHealth.CurrentHealth <= wakeHealthThreshold;
        if (shouldBeAngry != isAngry)
        {
            isAngry = shouldBeAngry;
            if (animator != null)
                animator.SetBool("Mad", isAngry); // Mad = true -> Sleep; Mad = false -> Angry
        }

        if (playerInside && isAngry)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                Attack();
                timer = attackInterval;
            }
        }
    }

    private void Attack()
    {
        if (animator != null)
            animator.SetTrigger("Hit");

        if (playerInTrigger != null)
        {
            HealthNew player = playerInTrigger.GetComponent<HealthNew>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("Игрок получил урон от ловушки");
            }
        }

        _soundManager?.PlaySound("Thorns");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = true;
            playerInTrigger = collision.gameObject;
            timer = 0f; // моментальный удар при входе (если злой)
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = false;
            playerInTrigger = null;
        }
    }
}
