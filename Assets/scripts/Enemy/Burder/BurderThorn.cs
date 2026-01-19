using System.Collections;
using UnityEngine;
using Game.Audio;

public class BurderThorn : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _damageInterval = 1f;

    [Header("Life Settings")]
    [SerializeField] private float _lifeTime = 15f;
    public float deathSoundDelay = 1f; 

    private HealthNew _health;
    private Animator _animator;
    private Coroutine _damageCoroutine;
    private bool _isDead = false;
    private ISoundManager _soundManager;

    private void Start()
    {
        _soundManager = SoundManagerNew.Instance;
        _health = GetComponent<HealthNew>();
        _animator = GetComponent<Animator>();

        if (_health != null)
        {
            _health.OnDamageTaken += Damage;
            _health.OnDeath += Death;
        }

       
        Invoke(nameof(SelfDestruct), _lifeTime);
    }

    private void Damage()
    {
        if (_isDead) return;
        _animator.SetTrigger("Damage");
        _soundManager.PlaySound("Damage");
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void Death()
    {
        if (_isDead) return;
        _isDead = true;

        _animator.SetTrigger("Death");
        StartCoroutine(DeathRoutine()); 

        if (_health != null)
        {
            _health.OnDamageTaken -= Damage;
            _health.OnDeath -= Death;
        }
        
        // StopAllCoroutines();
        // Destroy(gameObject, 2f); // ������� ������������ ��������
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathSoundDelay);
        _soundManager.PlaySound("BurderThornDeath");
        StopAllCoroutines();
        Destroy(gameObject, 2f); 
    }

    private void SelfDestruct()
    {
        if (!_isDead)
        {
            _animator.SetTrigger("IdleDeath");
            StopAllCoroutines();
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
