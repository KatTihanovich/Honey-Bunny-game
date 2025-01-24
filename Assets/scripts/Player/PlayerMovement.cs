using UnityEngine;
using Spine.Unity;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI; 
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip jumpSound; 
    [SerializeField] private float volume = 1.0f; 
    private AudioManager audioManager;

    [Header("UI Elements")]
    public Button jumpButton;
    public Button kickButton;

    [Header("Joystick Settings")]
    [SerializeField] private Vector2 JoystickSize = new Vector2(200, 200);
    [SerializeField] private Vector2 JoystickPosition = new Vector2(300, 250);
    public JoyStick Joystick;
    private Finger MovementFinger;
    public Vector2 MovementAmount;
    private RectTransform joystickRect;

    [Header("Player Movement")]
    public Rigidbody2D playerRigidbody;
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
    // Collision
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
    [SerializeField] private float attackCD = 10; // CD
    private float attackCounter = Mathf.Infinity;
    private Health entityHealth;
    // Movement
    private Vector2 moveVelocity;
    private bool isFacingRight = true;
    private float verticalVelocity;
    //Animation
    private Animator anim;

    private void Start()
    {
        SetCharacterState(PlayerState.Idle);
        Application.targetFrameRate = 60;

        jumpButton.onClick.AddListener(InitiateJump);
        joystickRect = Joystick.joyStickObj.GetComponent<RectTransform>();

        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }
    private void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        //audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void FixedUpdate()
    {
        Move(InputManager.Movement);
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

        if (Input.GetMouseButtonDown(0) && attackCounter >= attackCD)
        {
            anim.SetTrigger("AttackPressed");
            attackCounter = 0;
            StartAttack();
        }
    }
    ///Movevement Input
    private void Move(Vector2 moveInput)
    {
        Turn(moveInput);
        if (moveInput.x != 0)
        {
            anim.SetBool("Run", true);
            Vector2 targetVelocity = new Vector2(moveInput.x, 0f) * moveSpeed;
            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, 5f * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);

        }
        else
        {
            anim.SetBool("Run", false);
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


    //Gravity related
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
            anim.SetBool("IsFlying", true);
            anim.SetTrigger("JumpPressed");
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
                anim.SetBool("IsFalling", true);
                anim.SetBool("IsFlying", false);
            }
        }
        else if (isGrounded && isFalling)
        {
            isJumping = false;
            isFalling = false;
            anim.SetBool("IsFalling", false);
            anim.SetBool("IsFlying", false);
            verticalVelocity = 0f;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalVelocity);
    }

    //Animation
    public enum PlayerState
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Attacking
    }


    private void SetCharacterState(PlayerState state)
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
        //JoyStick
            private void SetJoystickPosition(Vector2 position)
    {
        joystickRect.anchorMin = new Vector2(0, 0);
        joystickRect.anchorMax = new Vector2(0, 0);
        joystickRect.anchoredPosition = position;
    }


    private void HandleFingerDown(Finger touchedFinger)
    {
        if (MovementFinger == null && touchedFinger.screenPosition.x <= 400 & touchedFinger.screenPosition.y <= 400)
        {
            MovementFinger = touchedFinger;
            MovementAmount = Vector2.zero;
        }
    }


    private void HandleFingerMove(Finger movedFinger)
    {
        if (movedFinger == MovementFinger)
        {
            Vector2 touchPosition = movedFinger.currentTouch.screenPosition;
            Vector2 localTouchPosition = joystickRect.InverseTransformPoint(touchPosition);
            float maxMovement = JoystickSize.x / 2f;
            localTouchPosition = Vector2.ClampMagnitude(localTouchPosition, maxMovement);
            Vector2 movementDirection = localTouchPosition.normalized;
            MovementAmount = new Vector2(movementDirection.x, 0f);
            Joystick.Knob.anchoredPosition = localTouchPosition;
        }
    }
    private void HandleLoseFinger(Finger lostFinger)
    {
        if (lostFinger == MovementFinger)
        {
            MovementFinger = null;
            Joystick.Knob.anchoredPosition = Vector2.zero;
            MovementAmount = Vector2.zero;
        }
    }
    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleLoseFinger;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();

    }
}
    private void StartAttack()
    {
        attackCounter = 0;
    }
    // Damage
    public void ApplyAreaDamage()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            boxCollider.bounds.center + transform.up * transform.localScale.y * colliderDistanceY + transform.right * transform.localScale.x * colliderDistanceX,
            new Vector2(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY),
            0, entityLayer);

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
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
