using UnityEngine;
using Spine.Unity;
using UnityEngine.InputSystem;
using Game.Combat;
using Game.Audio;
using UnityEngine.UI;

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
    [SerializeField] private float _moveSpeed = 9f;
    [SerializeField] private float _moveSmoothing = 5f;
    [SerializeField] private float _airControlMultiplier = 0.5f;
    private float _baseMoveSpeed;
    private bool _isSlowed = false;

    [Header("Jump Settings")]
    [SerializeField] private bool _enableDoubleJump = false;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _fallMultiplier = 2.5f;
    [SerializeField] private float _lowJumpMultiplier = 2f;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private float _jumpBufferTime = 0.2f;
    private bool _canDoubleJump = false;
    private bool _hasDoubleJumped = false;
    private bool _hasJumped = false;

    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private Transform _groundCheck;

    [Header("Sound Settings")]
    [SerializeField] private float _runSoundInterval = 0.3f;
    private float _baseRunSoundInterval;

    [Header("Push Settings")]
    [SerializeField] private float _pushPower = 2f;

    private bool _isGrounded;
    private bool _isAttacking;
    private bool _isSuperAttacking;
    private bool _isDead;
    private bool _isJumping;
    private bool _isTakingDamage;
    private bool _isRunning;
    private bool _jumpTriggered;
    private bool _isExitAnimationDagame;
    private bool _isIsFlying;
    private bool _isMeditation;
    private bool _isPush;
    private bool _isJumpPress;

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
    public bool IsPushed() => _isPush;
    public bool IsJumpPress() => _isJumpPress;

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
    public bool IsFlying()
    {
        bool groundedNow = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);
        return !groundedNow && _rb.linearVelocity.y > 0.5f;
    }

    public void SetDeadState(bool isDead)
    {
        _isDead = isDead;
    }
    public float GetRandomA() => Random.Range(0f, 1f);
    public bool IsMeditation
    {
        get { return _isMeditation; }
        set { _isMeditation = value; }
    }
    public Rigidbody2D Rb => _rb;

    public void DoubleJump(Toggle status)
    {
        _enableDoubleJump = status.isOn;
    }

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
        _baseMoveSpeed = _moveSpeed;
        _baseRunSoundInterval = _runSoundInterval;

        _health = GetComponent<HealthNew>();
        if (_health != null)
        {
            _health.OnDeath += HandleDeath;
            _health.OnDamageTaken += GetDamage;
        }
    }

    private void Update()
    {
        if (_isDead) return;

        HandleInput();
        CheckGrounded();
        HandleJumpInput();
        HandleRunSound();
        _isIsFlying = IsFlying();
    }

    private void FixedUpdate()
    {
        if (_isDead) return;

        if (_isTakingDamage && !_playerAnimation.IsAnimationDamageExit)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        CheckGrounded();
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

        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            _jumpTriggered = true;
            _jumpBufferCounter = _jumpBufferTime;
     
        }
        else
        {
            _isJumpPress = false; 
        }

        if (_isAttacking || _isSuperAttacking) return;
        if (Keyboard.current.fKey.wasPressedThisFrame && _isGrounded && !_isAttacking)
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!_isGrounded)
            return;

        if (collision.gameObject.TryGetComponent<PushableObject>(out var pushable))
        {
            bool sideContact = false;
            foreach (var c in collision.contacts)
                if (Mathf.Abs(c.normal.y) <= 0.5f) { sideContact = true; break; }

            if (sideContact && Mathf.Abs(_horizontalInput) > 0.1f)
            {
                pushable.StartPushing(_horizontalInput);
                _isPush = true;
                return;
            }
        }

        _isPush = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PushableObject>(out var pushable))
        {
            pushable.StopPushing();
            _isPush = false;
        }
    }

//домножить скорость движения на deltaTime
    private void HandleMovement()
    {
        if (_isAttacking || _isTakingDamage || !_playerAnimation.IsAnimationDamageExit) return;

        float control = _isGrounded ? 1f : _airControlMultiplier;
        float targetXVelocity = _horizontalInput * _moveSpeed * control;

        Vector2 targetVelocity = new Vector2(targetXVelocity, _rb.linearVelocity.y);
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
                _hasJumped = false;
                _hasDoubleJumped = false;
                _playerAnimation.SetDoubleJump(false); 
                Debug.Log("Приземление: DoubleJump сброшен");
            }
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void SetForceJump() 
    {
        if (_enableDoubleJump)
        {
            _jumpForce = 8;

        }
        else 
        {
            _jumpForce = 10;
        }
    }

    private void HandleJumpInput()
    {
        SetForceJump();

        if (_isAttacking || _isTakingDamage || !_playerAnimation.IsAnimationDamageExit)
            return;

        _jumpBufferCounter -= Time.deltaTime;

        if (_jumpTriggered)
        {
            if (_isGrounded && _jumpBufferCounter > 0f)
            {
                Jump();
                _hasJumped = true;
                _hasDoubleJumped = false;
                _playerAnimation.SetDoubleJump(false);
                _jumpTriggered = false; 
                Debug.Log("Первый прыжок выполнен");
            }
            else if (_enableDoubleJump && _hasJumped && !_hasDoubleJumped)
            {
                Jump();
                _hasDoubleJumped = true;
                _playerAnimation.SetDoubleJump(true); // Устанавливаем для двойного прыжка
                _jumpTriggered = false; // Сбрасываем после двойного прыжка
                Debug.Log("Двойной прыжок выполнен");
            }
        }
    }

    private void Jump()
    {
        _isJumpPress = true;
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
        _isJumping = true;
        if (!_isMeditation)
            _soundManager.PlaySound("Jump");
        _jumpTriggered = true; // Устанавливаем триггер для анимации
        Debug.Log($"Прыжок: IsDoubleJump={_hasDoubleJumped}, IsGrounded={_isGrounded}");

        // Устанавливаем параметры для анимации
        if (_hasDoubleJumped && !_isGrounded)
        {
            _playerAnimation.SetDoubleJump(true); // Передаем информацию о двойном прыжке
        }
        else
        {
            _playerAnimation.SetDoubleJump(false); // Сбрасываем для первого прыжка
        }
    }

    private void ApplyJumpPhysics()
    {
        if (_rb.linearVelocity.y < 0f)
        {
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (_rb.linearVelocity.y > 0f && !_enableDoubleJump)
        {
            if (!Keyboard.current.spaceKey.isPressed &&
                !Keyboard.current.wKey.isPressed &&
                !Keyboard.current.upArrowKey.isPressed)
            {
                _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (_lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
            }
        }
    }

    private void HandleRunSound()
    {
        if (_isRunning && _isGrounded && !IsMeditation)
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
        _isTakingDamage = true;
        _rb.linearVelocity = Vector2.zero;
        _soundManager.PlaySound("Damage");
        Invoke(nameof(ResetDamageState), 0.5f);
    }

    private void ResetDamageState()
    {
        _isTakingDamage = false;
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
        _soundManager.PlaySound("Death");
        GetComponent<PlayerController>().enabled = false;
        GetComponent<HealthNew>().enabled = false;
        GetComponent<PlayerRespawn>().CheckRespawn();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
    }

    public void SlowModeEnable()
    {
        if (!_isSlowed)
        {
            _moveSpeed = Mathf.Max(_baseMoveSpeed * 0.75f, 1f);
            Debug.Log("Текущая скорость: " + _moveSpeed);
            _runSoundInterval = _baseRunSoundInterval + 0.4f;
            _isSlowed = true;
        }
    }

    public void SlowModeDesable()
    {
        if (_isSlowed)
        {
            _moveSpeed = _baseMoveSpeed;
            _runSoundInterval = _baseRunSoundInterval;
            _isSlowed = false;
        }
    }
}