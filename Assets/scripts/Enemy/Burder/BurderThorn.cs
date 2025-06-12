using System.Collections;
using UnityEngine;

public class BurderThorn : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _damageInterval = 1f;

    [Header("Life Settings")]
    [SerializeField] private float _lifeTime = 15f;

    private HealthNew _health;
    private Animator _animator;
    private Coroutine _damageCoroutine;
    private bool _isDead = false;

    private void Start()
    {
        _health = GetComponent<HealthNew>();
        _animator = GetComponent<Animator>();

        if (_health != null)
        {
            _health.OnDamageTaken += Damage;
            _health.OnDeath += Death;
        }

        // Автоматическая смерть через заданное время
        Invoke(nameof(SelfDestruct), _lifeTime);
    }

    private void Damage()
    {
        if (_isDead) return;
        _animator.SetTrigger("Damage");
    }

    private void Death()
    {
        if (_isDead) return;
        _isDead = true;

        _animator.SetTrigger("Death");

        if (_health != null)
        {
            _health.OnDamageTaken -= Damage;
            _health.OnDeath -= Death;
        }

        StopAllCoroutines();
        Destroy(gameObject, 2f); // Ожидаем проигрывания анимации
    }

    private void SelfDestruct()
    {
        if (!_isDead)
        {
            _animator.SetTrigger("IdleDeath");
            Death(); // Принудительно вызвать смерть
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDead) return;

        if (collision.CompareTag("Player"))
        {
            _damageCoroutine = StartCoroutine(DamageOverTime(collision));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _damageCoroutine != null)
        {
            StopCoroutine(_damageCoroutine);
            _damageCoroutine = null;
        }
    }

    private IEnumerator DamageOverTime(Collider2D player)
    {
        HealthNew playerHealth = player.GetComponent<HealthNew>();

        while (player != null && !_isDead)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damage);
            }

            yield return new WaitForSeconds(_damageInterval);
        }
    }
}
