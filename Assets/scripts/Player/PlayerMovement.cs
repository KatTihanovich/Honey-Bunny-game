using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int AttackPressed = Animator.StringToHash("AttackPressed");
    private static readonly int IsFlying = Animator.StringToHash("IsFlying");
    private static readonly int JumpPressed = Animator.StringToHash("JumpPressed");
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");

    [Header("Audio Settings")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private float volume = 1.0f;
    private AudioManager audioManager;

    [Header("UI Elements")]
    public Button jumpButton;
    public Button kickButton;

    [Header("Joystick Settings")]
    private ETouch.Finger movementFinger;
    private RectTransform joystickRect;

    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 12f;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpBufferTime;
    private float jumpBufferCounter;
    [SerializeField] private float coyoteTime;
    private float coyoteCounter;
    [SerializeField] private float jumpHeight = 17f;

    [Header("Gravity Parameters")]
    [SerializeField] private float gravity = 33f;
    private bool isGrounded;
    private bool isJumping;
    private bool isFalling;

    [Header("What is Ground Parameters")]
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D rb;
    private RaycastHit2D groundHit;
    public bool isOnPlatform;

    [Header("Starting point to cast ray to the ground")]
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Attack Parameters")]
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private LayerMask entityLayer;
    [SerializeField] private float colliderDistanceX = 1f;
    [SerializeField] private float colliderDistanceY = 0.5f;
    [SerializeField] private float rangeX = 1.5f;
    [SerializeField] private float rangeY = 1f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCd = 10; // CD
    private float attackCounter = Mathf.Infinity;

    // Movement
    private Vector2 moveVelocity;
    private float verticalVelocity;

    // Animation
    private Animator anim;

    private void Start()
    {
        SetCharacterState(PlayerState.Idle);
        // Application.targetFrameRate = 60;

        jumpButton.onClick.AddListener(InitiateJump);
 
        ETouch.EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        IsGrounded();
        Jump();
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

        if (Input.GetKey(KeyCode.Space))
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

        if (Input.GetKeyDown(KeyCode.F) && attackCounter >= attackCd)
        {
            anim.SetTrigger(AttackPressed);
            attackCounter = 0;
            StartAttack();
        }
    }
    public void TryJump()
    {
        if (isGrounded && !isJumping && !isFalling || coyoteCounter > 0)
        {
            InitiateJump();
        }
    }
    public void TryAttack()
    {
        if (attackCounter >= attackCd)
        {
            anim.SetTrigger(AttackPressed);
            attackCounter = 0;
            StartAttack();
        }
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
            anim.SetBool(IsFlying, true);
            anim.SetTrigger(JumpPressed);
            verticalVelocity = jumpHeight;
            jumpBufferCounter = 0;
            coyoteCounter = 0;

            if (jumpSound != null)
            {
                AudioSource.PlayClipAtPoint(jumpSound, transform.position, volume);
            }
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
                anim.SetBool(IsFalling, true);
                anim.SetBool(IsFlying, false);
            }
        }
        else if (isGrounded && isFalling)
        {
            isJumping = false;
            isFalling = false;
            anim.SetBool(IsFalling, false);
            anim.SetBool(IsFlying, false);
            verticalVelocity = 0f;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalVelocity);
    }

    private enum PlayerState
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Attacking 
    }

    private static void SetCharacterState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Walking:
                break;
            case PlayerState.Jumping:
                break;
            case PlayerState.Falling:
                break;
        }
    }

    private void HandleFingerDown(ETouch.Finger touchedFinger)
    {
        if (movementFinger == null && touchedFinger.screenPosition.x <= 400 & touchedFinger.screenPosition.y <= 400)
        {
            movementFinger = touchedFinger;
           
        }
    }
    
    private void HandleLoseFinger(ETouch.Finger lostFinger)
    {
        if (lostFinger == movementFinger)
        {
            movementFinger = null;
           
        }
    }

    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleLoseFinger;
        ETouch.EnhancedTouchSupport.Disable();
    }

    private void StartAttack()
    {
        attackCounter = 0;
    }
}
