using UnityEngine;
using Spine.Unity;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI; 

public class PlayerMovement : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip jumpSound; 
    [SerializeField] private float volume = 1.0f; 

    [Header("UI Elements")]
    public Button jumpButton;

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

    [Header("Collision")]
    [SerializeField] private Collider2D collider;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpBufferTime;
    private float jumpBufferCounter;
    [SerializeField] private float coyoteTime;
    private float coyoteCounter;
    [SerializeField] private float jumpHeight = 17f;

    [Header("Gravity Parameters")]
    [SerializeField] private float gravity = 33f;


    // movement
    private Vector2 moveVelocity;
    private bool isFacingRight = true;

    // collision check
    private RaycastHit2D groundHit;
    private bool isGrounded;

    //jump
    private bool isJumping;
    private bool isFalling;
    private float verticalVelocity;


    //health
    private Health health;

    [Header("Animation")]
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, walking, falling, jumping;
    private string currentAnimation;

    //platform
    public bool isOnPlatform;

     private void Awake()
    {
        isFacingRight = true;
        playerRigidbody = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
    }

    private void Start()
    {
        jumpButton.onClick.AddListener(InitiateJump);
        joystickRect = Joystick.joyStickObj.GetComponent<RectTransform>();

        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger;
        ETouch.Touch.onFingerMove += HandleFingerMove;

        SetCharacterState("Idle");
        Application.targetFrameRate = 60;
    }
    private void SetJoystickPosition(Vector2 position)
    {
        joystickRect.anchorMin = new Vector2(0, 0);
        joystickRect.anchorMax = new Vector2(0, 0);
        joystickRect.anchoredPosition = position;
    }

    private void FixedUpdate()
    {
    MovePlayer(MovementAmount);
    IsGrounded();
    Jump();
    }

    private void Update()
    {
        if (isGrounded && !isJumping && !isFalling)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.fixedDeltaTime;
        }

        if (InputManager.JumpWasPressed)
        {
            jumpBufferCounter = jumpBufferTime;
            InputManager.JumpWasPressed = false;
        }
        else
        {
            jumpBufferCounter -= Time.fixedDeltaTime;
        }

        if (jumpBufferCounter > 0f)
        {
            InitiateJump();
        }

        UpdateAnimationState();
        SetJoystickPosition(JoystickPosition);
    }


    private void MovePlayer(Vector2 moveInput)
    {
        Turn();
        if (moveInput.x != 0)
        {
            Vector2 targetVelocity = new Vector2(moveInput.x * moveSpeed, playerRigidbody.linearVelocity.y);
            playerRigidbody.linearVelocity = Vector2.Lerp(playerRigidbody.linearVelocity, targetVelocity, 5f * Time.fixedDeltaTime);
        }
        else
        {
            Vector2 targetVelocity = new Vector2(0, playerRigidbody.linearVelocity.y);
            playerRigidbody.linearVelocity = Vector2.Lerp(playerRigidbody.linearVelocity, targetVelocity, 20f * Time.fixedDeltaTime);
        }
    }

    private void Turn()
    {
        if (isFacingRight && MovementAmount.x < 0f)
        {
            isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
        else if (!isFacingRight && MovementAmount.x > 0f)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(collider.bounds.center.x, collider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(collider.bounds.size.x, 0.1f);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, 0.1f, groundLayer.value);
        isGrounded = groundHit.collider;

        if (!isGrounded && !isJumping)
        {
            isFalling = true;
        }
    }

    public void InitiateJump()
    {
        if (isGrounded && !isJumping && !isFalling || coyoteCounter > 0)
        {
            isJumping = true;
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

            }
        }
        else if (isGrounded && isFalling)
        {
            isJumping = false;
            isFalling = false;
            verticalVelocity = 0f;
        }
        playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, verticalVelocity);
    }


    private void UpdateAnimationState()
    {
        if (isJumping)
        {
            SetCharacterState("Jumping");
        }
        else if (isFalling)
        {
            SetCharacterState("Falling");
        }
        else
        {
            SetCharacterState(MovementAmount.x != 0 ? "Walking" : "Idle");
        }
    }


    private void SetAnimation(AnimationReferenceAsset animation, bool loop, float timescale)
    {
        if (animation.name.Equals(currentAnimation))
        {
            return;
        }

        skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timescale;
        currentAnimation = animation.name;
    }

    private void SetCharacterState(string state)
    {
        if (state.Equals("Idle"))
        {
            SetAnimation(idle, true, 1f);
        }
        else if (state.Equals("Walking"))
        {
            SetAnimation(walking, true, 1f);
        }
        else if (state.Equals("Falling"))
        {
            SetAnimation(falling, true, 1f);
        }
        else if (state.Equals("Jumping"))
        {
            SetAnimation(jumping, true, 1f);
        }
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
