using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private PlayerController _player;
    private HealthNew _health;

    private bool _attackTriggered;
    private bool _damageTriggered;
    private bool _superAttackAnimPlayed = false;


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _player = GetComponent<PlayerController>();
        _health = GetComponent<HealthNew>();


        if (_health != null)
        {
            _health.OnDamaged += HandleDamaged;
            _health.OnDeath += HandleDeath;
        }
    }

    private void Update()
    {
    
        _animator.SetBool("isGrounded", _player.IsGrounded());
        _animator.SetBool("Run", _player.IsRunning());

        if (_player.JumpTriggered() && _player.IsGrounded() && !_player.IsJumping())
        {
            Debug.Log("Анимация прыжка: JumpPressed");
            _animator.SetTrigger("JumpPressed");
        }

        if (_player.JumpTriggered()) 
        {
            _animator.SetBool("IsFlying", _player.Rb.linearVelocity.y > 0.1f && !_player.IsGrounded());
        }
   
        _animator.SetBool("IsFalling", _player.Rb.linearVelocity.y < -0.1f && !_player.IsGrounded());

        if (_player.IsAttacking() && !_attackTriggered)
        {
            _animator.SetTrigger("AttackPressed");
            _attackTriggered = true;
        }
        else if (!_player.IsAttacking())
        {
            _attackTriggered = false;
        }

        if (_player.IsSuperAttacking() && !_superAttackAnimPlayed)
        {
            Debug.Log("Вызов анимации");
            _animator.SetTrigger("UltimatePressed");
            _superAttackAnimPlayed = true;
        }
        else if (!_player.IsSuperAttacking())
        {
            _superAttackAnimPlayed = false;
        }



        _animator.SetFloat("velocityX", Mathf.Abs(_player.Rb.linearVelocity.x));
        _animator.SetFloat("velocityY", _player.Rb.linearVelocity.y);
        _animator.SetFloat("RandomA", _player.GetRandomA());
    }

    private void OnDestroy()
    {
        if (_health != null)
        {
            _health.OnDamaged -= HandleDamaged;
            _health.OnDeath -= HandleDeath;
        }
    }

    private void HandleDamaged(float damage)
    {
        _animator.SetTrigger("GotHit");
    }

    private void HandleDeath()
    {
        _animator.SetBool("Dead", true);
    }
}