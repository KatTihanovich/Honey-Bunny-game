using Spine.Unity;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerMovement : MonoBehaviour
{
    [Header("Horizontal Parameters")]
    [SerializeField] private float moveSpeed = 12f;
    [Header("Jumping Parameters")]
    [SerializeField] private float jumpBufferTime;
    private float jumpBufferCounter;
    [SerializeField] private float coyoteTime;
    private float coyoteCounter;
    [SerializeField] private float jumpHeight = 17f;
    [Header("Gravity Parameters")]
    [SerializeField] private float gravity = 33f;

    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    [SerializeField] private Collider2D playerCollider;

    [Header("Attack Parameters")]
    [SerializeField] private BoxCollider2D boxCollider; // Коллайдер для определения центра проверки
    [SerializeField] private LayerMask entityLayer; // Слой для фильтрации сущностей
    [SerializeField] private float colliderDistanceX = 1f; // Смещение по X
    [SerializeField] private float colliderDistanceY = 0.5f; // Смещение по Y
    [SerializeField] private float rangeX = 1.5f; // Ширина области
    [SerializeField] private float rangeY = 1f; // Высота области
    [SerializeField] private int damage = 10; // Урон
    [SerializeField] private float attackCD = 10; // Урон
    private float attackCounter = Mathf.Infinity;
    private Health entityHealth;

    // Movement
    private Vector2 moveVelocity;
    private bool isFacingRight = true;

    // Collision
    private RaycastHit2D groundHit;
    private bool isGrounded;

    // Jumping
    private bool isJumping;
    private bool isFalling;
    private float verticalVelocity;
    private bool isAttacking = false; // Флаг, чтобы предотвратить переключение состояний

    public bool isOnPlatform;
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, walking, falling, jumping, attacking;

    private PlayerState currentState;

    private void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        currentState = PlayerState.Idle;
    }

    private void Move(Vector2 moveInput)
    {
        Turn(moveInput);
        if (moveInput.x != 0)
        {
            Vector2 targetVelocity = new Vector2(moveInput.x, 0f) * moveSpeed;
            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, 5f * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);
        }
        else
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, 20f * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);
        }
    }

    private void Turn(Vector2 moveInput)
    {
        if (isFacingRight && moveInput.x < 0f)
        {
            isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
        else if (!isFacingRight && moveInput.x > 0f)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void Start()
    {
        SetCharacterState(PlayerState.Idle);
        Application.targetFrameRate = 60;
    }

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(playerCollider.bounds.size.x, 0.1f);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, 0.1f, groundLayer.value);
        isGrounded = groundHit.collider;
    }

    private void InitiateJump()
    {
        if (isGrounded && !isJumping && !isFalling || coyoteCounter > 0)
        {
            isJumping = true;
            verticalVelocity = jumpHeight;
            jumpBufferCounter = 0;
            coyoteCounter = 0;
        }
    }

    private void Jump()
    {
        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.fixedDeltaTime;

            if (verticalVelocity < 0f && !isFalling)
            {
                isJumping = false;
                isFalling = true;
            }
        }
        else if (isGrounded && isFalling)
        {
            isJumping = false;
            isFalling = false;
            verticalVelocity = 0f;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalVelocity);
    }

    private void Update()
    {

        attackCounter += Time.fixedDeltaTime;
        if (isGrounded && !isJumping && !isFalling)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.fixedDeltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.fixedDeltaTime;
        }

        if (jumpBufferCounter > 0f)
        {
            InitiateJump();
        }

        if (isJumping)
        {
            SetCharacterState(PlayerState.Jumping);
        }
        else if (isFalling)
        {
            SetCharacterState(PlayerState.Falling);
        }
        else
        {
            SetCharacterState(InputManager.Movement.x != 0 ? PlayerState.Walking : PlayerState.Idle);
        }

        if (Input.GetMouseButtonDown(0) && attackCounter >= attackCD)
        {
            attackCounter = 0;
            StartAttack();
        }
    }

    private void FixedUpdate()
    {
        Move(InputManager.Movement);
        IsGrounded();
        Jump();
    }

    private void SetCharacterState(PlayerState state)
    {
        // Не менять состояние, если уже в атаке
        if (isAttacking && state != PlayerState.Attacking) return;

        if (currentState == state && state != PlayerState.Attacking) return;

        currentState = state;
        switch (state)
        {
            case PlayerState.Idle:
                SetAnimation(idle, true, 1f);
                break;
            case PlayerState.Walking:
                SetAnimation(walking, true, 1f);
                break;
            case PlayerState.Jumping:
                SetAnimation(jumping, true, 1f);
                break;
            case PlayerState.Falling:
                SetAnimation(falling, true, 1f);
                break;
            case PlayerState.Attacking:
                isAttacking = true; // Устанавливаем флаг атаки
                SetAnimation(attacking, false, 1.7f);
                skeletonAnimation.state.GetCurrent(0).Complete += OnAttackAnimationComplete;
                break;
        }
    }

    private void OnAttackAnimationComplete(Spine.TrackEntry trackEntry)
    {
        // Снимаем флаг атаки
        isAttacking = false;

        // Возвращаем состояние в Idle или Walking
        SetCharacterState(InputManager.Movement.x != 0 ? PlayerState.Walking : PlayerState.Idle);
    }


    public void SetAnimation(AnimationReferenceAsset animation, bool loop, float timescale)
    {
        skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timescale;
    }

    public void ApplyAreaDamage()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            boxCollider.bounds.center + transform.up * transform.localScale.y * colliderDistanceY + transform.right * transform.localScale.x * colliderDistanceX,
            new Vector2(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY),
            0, entityLayer); // Используем OverlapBoxAll вместо BoxCastAll

        foreach (var hit in hits)
        {
            if (hit != null && hit.CompareTag("Enemy"))
            {
                Health entityHealth = hit.GetComponent<Health>();
                if (entityHealth != null)
                {
                    entityHealth.TakeDamage(damage);
                }
                else
                {
                }
            }
        }
    }


    private void StartAttack()
    {
        isAttacking = true;
        SetCharacterState(PlayerState.Attacking);

        // После завершения анимации атаки, сбрасываем флаг
        skeletonAnimation.state.Complete += OnAttackAnimationComplete;
    }




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.color = Color.red;
        Vector3 boxSize = new Vector3(
            boxCollider.bounds.size.x * rangeX,
            boxCollider.bounds.size.y * rangeY,
            boxCollider.bounds.size.z);
        Vector3 boxCenter = boxCollider.bounds.center +
            transform.up * transform.localScale.y * colliderDistanceY +
            transform.right * transform.localScale.x * colliderDistanceX;

        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
    public enum PlayerState
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Attacking
    }
}
