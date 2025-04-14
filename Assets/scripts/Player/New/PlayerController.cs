using UnityEngine;
using Spine.Unity;
using UnityEngine.InputSystem;
using Game.Combat;
using Game.Audio;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius = 0.5f;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private int _attackDamage = 20;
    [SerializeField] private float _attackDuration = 0.5f;

    [Header("Super Attack Settings")]
    [SerializeField] private float _superAttackRadius = 1f;
    [SerializeField] private int _superAttackDamage = 100;
    [SerializeField] private bool _isSuperAttackReady = true; // Можно включить через прогресс

    public void SetSuperAttackReady(bool ready) => _isSuperAttackReady = ready;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _moveSmoothing = 5f;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _fallMultiplier = 2.5f;
    [SerializeField] private float _lowJumpMultiplier = 2f;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private float _jumpBufferTime = 0.2f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private Transform _groundCheck;

    [Header("Sound Settings")]
    [SerializeField] private float _runSoundInterval = 0.3f;

    private bool _isGrounded;
    private bool _isAttacking;
    private bool _isSuperAttacking;
    private bool _isDead;
    private bool _isJumping;
    private bool _isTakingDamage;
    private bool _isRunning;
    private bool _jumpTriggered;

    private Rigidbody2D _rb;
    private CapsuleCollider2D _coll;
    private PlayerAnimation _playerAnimation;
    private HealthNew _health;
    private IAttack _meleeAttack;
    private ISoundManager _soundManager;

    private float _horizontalInput;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;
    private Vector2 _moveVelocity;
    private Vector3 _originalScale;
    private float _runSoundTimer;

    public bool IsGrounded() => _isGrounded;
    public bool IsAttacking() => _isAttacking;
    public bool IsSuperAttacking() => _isSuperAttacking;
    public bool IsDead() => _isDead;
    public bool IsJumping() => _isJumping;
    public bool IsTakingDamage() => _isTakingDamage;
    public bool IsRunning() => _isRunning;
    public bool JumpTriggered() => _jumpTriggered;
    public void JumpTriggered(bool value) => _jumpTriggered = value;
    public bool IsFalling() => _rb.linearVelocity.y < -0.1f;
    public bool IsFlying() => _rb.linearVelocity.y > 0.1f;
    public float GetRandomA() => Random.Range(0f, 1f);
    public Rigidbody2D Rb => _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _coll = GetComponent<CapsuleCollider2D>();
        _playerAnimation = GetComponent<PlayerAnimation>();
        _meleeAttack = new PlayerMeleeAttack(_attackDuration);
        _soundManager = SoundManagerNew.Instance;

        _rb.freezeRotation = true;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _originalScale = transform.localScale;

        _health = GetComponent<HealthNew>();
        if (_health != null)
        {
            _health.OnDeath += HandleDeath;
        }
    }

    private void Update()
    {
        if (_isDead) return;

        HandleInput();
        CheckGrounded();
        HandleJumpInput();
        HandleRunSound();
    }

    private void FixedUpdate()
    {
        if (_isDead) return;

        HandleMovement();
        ApplyJumpPhysics();
    }

    private void HandleDeath()
    {
        Die();
    }

    private void HandleInput()
    {
        _horizontalInput = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            _horizontalInput = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            _horizontalInput = 1f;

        if (Keyboard.current.spaceKey.wasPressedThisFrame & _isGrounded)
        {
            _jumpTriggered = true; 
            _jumpBufferCounter = _jumpBufferTime;
        }

        if (_isAttacking || _isSuperAttacking) return;
        if (Keyboard.current.eKey.wasPressedThisFrame && _isGrounded && !_isAttacking)
        {
            Debug.Log("Атака");
            _isAttacking = true;
            Attack();
            Invoke(nameof(ResetAttack), _meleeAttack.AttackDuration);
        }

        if (Keyboard.current.qKey.wasPressedThisFrame && _isGrounded && _isSuperAttackReady && !_isSuperAttacking)
        {
            Debug.Log("СУПЕР АТАКА!");

            _isSuperAttacking = true;
            SuperAttack();
            _isSuperAttackReady = false; 
            Invoke(nameof(ResetSuperAttack), _meleeAttack.AttackDuration);

        }
    }


 
   


    private void HandleMovement()
    {
        if (_isAttacking) return;

        Vector2 targetVelocity = new Vector2(_horizontalInput * _moveSpeed, _rb.linearVelocity.y);
        _moveVelocity = Vector2.Lerp(_rb.linearVelocity, targetVelocity, _moveSmoothing * Time.fixedDeltaTime);
        _rb.linearVelocity = _moveVelocity;

        _isRunning = Mathf.Abs(_horizontalInput) > 0.1f;

        if (_horizontalInput != 0f)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(_horizontalInput) * Mathf.Abs(_originalScale.x),
                _originalScale.y,
                _originalScale.z
            );
        }
    }

    private void CheckGrounded()
    {
        bool wasGrounded = _isGrounded;
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);

        if (_isGrounded)
        {
            _coyoteTimeCounter = _coyoteTime;
            if (!wasGrounded)
            {
                _isJumping = false;
                _jumpTriggered = false;
            }
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void HandleJumpInput()
    {
        if (_jumpBufferCounter > 0f && (_isGrounded || _coyoteTimeCounter > 0f) && _jumpTriggered)
        {
            Jump();
            _jumpBufferCounter = 0f;
            _coyoteTimeCounter = 0f;
        }
        _jumpBufferCounter -= Time.deltaTime;
    }

    private void Jump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
        _isJumping = true;
        _soundManager.PlaySound("Jump");
    }

    private void ApplyJumpPhysics()
    {
        if (_rb.linearVelocity.y < 0f)
        {
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (_rb.linearVelocity.y > 0f && !Keyboard.current.spaceKey.isPressed)
        {
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (_lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    private void HandleRunSound()
    {
        if (_isRunning && _isGrounded)
        {
            _runSoundTimer -= Time.deltaTime;
            if (_runSoundTimer <= 0f)
            {
                _soundManager.PlaySound("Run");
                _runSoundTimer = _runSoundInterval;
            }
        }
        else
        {
            _runSoundTimer = 0f;
        }
    }

    private void Attack()
    {
        _meleeAttack.PerformAttack(_attackPoint, _attackRadius, _enemyLayer, _attackDamage, transform.position);
        _soundManager.PlaySound("Attack");
    }

    private void SuperAttack()
    {
        _meleeAttack.PerformSuperAttack(_attackPoint, _attackRadius, _enemyLayer, _superAttackDamage, transform.position);
        _soundManager.PlaySound("SuperAttack");
    }

    public void GetDamage() 
    {
    
    }



    private void ResetAttack()
    {
        _meleeAttack.Reset();
        _isAttacking = false;
    }

    private void ResetSuperAttack()
    {
        _meleeAttack.Reset();
        _isSuperAttacking = false;
    }
    public void Die()
    {
        _isDead = true;
        GetComponent<PlayerController>().enabled = false;
        GetComponent<HealthNew>().enabled = false;
    }
}