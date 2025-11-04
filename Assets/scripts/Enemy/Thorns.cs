using UnityEngine;
using Game.Audio;

public class SpikeTrap : MonoBehaviour
{
    public Animator animator;
    public int damage = 1;
    public float attackInterval = 2f;

    private float timer = 0f;
    private bool playerInside = false;
    private GameObject playerInTrigger;
    private ISoundManager _soundManager;
    private void Start()
    {
        _soundManager = SoundManagerNew.Instance;
    }

    private void Update()
    {
        if (playerInside)
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
        // Запускаем анимацию
        if (animator != null)
            animator.SetTrigger("Hit");

        // Наносим урон, если игрок на месте
        if (playerInTrigger != null)
        {
            HealthNew player = playerInTrigger.GetComponent<HealthNew>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("Игрок получил урон от ловушки");
            }
        }
        _soundManager.PlaySound("Thorns");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = true;
            playerInTrigger = collision.gameObject;
            timer = 0f; // Моментальный удар при входе
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
