using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Meditation : MonoBehaviour
{
    [SerializeField] private float _healRatePerSecond = 2f;
    [SerializeField] private float _healInterval = 0.1f;

    private HealthNew _health;
    private Coroutine _healingCoroutine;
    private PlayerController _player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _player = collision.GetComponent<PlayerController>();
            _health = collision.GetComponent<HealthNew>();

           
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
     
        if (collision.CompareTag("Player") &&
            _health != null &&
            !_health.IsDead &&
            !_player.IsRunning() )
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                _player.IsMeditation = true;

                if (_healingCoroutine == null)
                    _healingCoroutine = StartCoroutine(HealOverTime());
            }
            else
            {
        
                _player.IsMeditation = false;

                if (_healingCoroutine != null)
                {
                    StopCoroutine(_healingCoroutine);
                    _healingCoroutine = null;
                }
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_player != null)
                _player.IsMeditation = false;

            if (_healingCoroutine != null)
                StopCoroutine(_healingCoroutine);

            _healingCoroutine = null;
            _health = null;
            _player = null;
        }
    }

    private IEnumerator HealOverTime()
    {
        while (_health != null && !_health.IsDead)
        {
            if (_health.CurrentHealth >= _health.MaxHealth)
            {
                _player.IsMeditation = false;
                _healingCoroutine = null;
                yield break;
            }

            _health.Heal(_healRatePerSecond * _healInterval);
            yield return new WaitForSeconds(_healInterval);
        }

        _healingCoroutine = null;
    }
}
