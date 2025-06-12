using Spine.Unity;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private PlayerController _player;
    private HealthNew _health;
    private Stress _stress;
    private SkeletonAnimation _jumpEffect;

    private bool _attackTriggered;
    private bool _damageTriggered;
    private bool _superAttackAnimPlayed = false;
    private bool _isAnimationDamageExit = true;
    private bool _isMeditation;

    public bool IsMeditation
    {
        get { return _isMeditation; }
        set { _isMeditation = value; }
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _player = GetComponent<PlayerController>();
        _health = GetComponent<HealthNew>();
        _stress = GetComponent<Stress>();
        _jumpEffect = GetComponentInChildren<SkeletonAnimation>();

        Debug.LogWarning(_jumpEffect);

        if (_health != null)
        {
            _health.OnDamaged += HandleDamaged;
            _health.OnDeath += HandleDeath;
        }

        if (_stress != null)
        {
            _stress.OnStressed += HandleStressChanged;
            _stress.OnStressReduced += HandleStressChanged;

            HandleStressChanged(0f);
        }
    }

    public bool IsAnimationDamageExit
    {
        get { return _isAnimationDamageExit; }
    }

    public bool ToggleDamageAnimationStatus()
    {
        _isAnimationDamageExit = !_isAnimationDamageExit;
        return _isAnimationDamageExit;
    }

    public void SetDoubleJump(bool isDoubleJump)
    {
        _animator.SetBool("DoubleJump", isDoubleJump);
        Debug.Log($"Set DoubleJump to {isDoubleJump}, IsFlying={_player.IsFlying()}");
    }

    private void Update()
    {
        _animator.SetBool("Grounded", _player.IsGrounded());
        _animator.SetBool("Run", _player.IsRunning());
        _animator.SetBool("IsFlying", _player.IsFlying());
        _animator.SetBool("IsFalling", _player.IsFalling());
        _animator.SetBool("Save", _player.IsMeditation);
        _animator.SetBool("Push", _player.IsPushed());

 
        if (_player.JumpTriggered())
        {
     
            _animator.SetTrigger("JumpPressed");
          
        


        }

        if (_player.IsJumpPress() && !_isMeditation) 
        {
            Debug.LogWarning("������ �����");
            _jumpEffect.AnimationState.SetAnimation(0, "JUMP EFFECT", false);
        }


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
         
            _animator.SetTrigger("UltimatePressed");
            _superAttackAnimPlayed = true;
        }
        else if (!_player.IsSuperAttacking())
        {
            _superAttackAnimPlayed = false;
        }
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

    private void HandleStressChanged(float _)
    {
        float stressPercent = _stress.CurrentStress / _stress.MaxStress;

        if (stressPercent < 0.3f)
            _animator.SetFloat("StressLevel", 0f);
        else if (stressPercent < 0.5f)
            _animator.SetFloat("StressLevel", 0.3f);
        else
            _animator.SetFloat("StressLevel", 0.5f);
    }
}