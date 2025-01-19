using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Spine.Unity;
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
    [SerializeField] private Vector2 JoystickSize = new Vector2(200, 200); // Size of the joystick
    [SerializeField] private Vector2 JoystickPosition = new Vector2(300, 250);
    public JoyStick Joystick;
    private Finger MovementFinger; // Finger tracking the joystick
    public Vector2 MovementAmount; // Normalized movement direction

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

    // [SerializeField] private float jumpBufferTime;


    // movement
    private Vector2 moveVelocity;
    private bool isFacingRight = true;
    private float verticalVelocity;

    // collision check
    private RaycastHit2D groundHit;
    private bool isGrounded;

    //jump
    private bool isJumping;
    private bool isFalling;

    [Header("Collision")]
    [SerializeField] private Collider2D collider;
    [SerializeField] private LayerMask groundLayer;


    private Health health;

    [Header("Animation")]
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, walking, falling, jumping;
    private string currentAnimation;

    public bool isOnPlatform;
    private Rigidbody2D platformRigidbody;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        jumpButton.onClick.AddListener(InitiateJump);

        // Enable Enhanced Touch support
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger;
        ETouch.Touch.onFingerMove += HandleFingerMove;

        // Set initial animation state
        SetCharacterState("Idle");
        Application.targetFrameRate = 60;
    }
    private void SetJoystickPosition(Vector2 position)
{
    RectTransform joystickRect = Joystick.joyStickObj.GetComponent<RectTransform>();

    // Set the anchor and position to a fixed screen point
    joystickRect.anchorMin = new Vector2(0, 0);
    joystickRect.anchorMax = new Vector2(0, 0);
    joystickRect.anchoredPosition = position; // Fixed position in screen coordinates

    // Ensure the joystick base cannot be moved
    joystickRect.GetComponent<CanvasGroup>().blocksRaycasts = true;
}


    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleLoseFinger;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();
    
    }


    private void FixedUpdate()
{
    MovePlayer();
    IsGrounded();
    HandleJump();

    // Add platform velocity if the player is on a platform
    if (isOnPlatform && platformRigidbody != null)
    {
        playerRigidbody.linearVelocity += platformRigidbody.linearVelocity;
    }
}

    private void Update()
    {
        UpdateAnimationState();
        SetJoystickPosition(JoystickPosition);
    }

    // Handle joystick finger down
    private void HandleFingerDown(Finger touchedFinger)
    {
        if (MovementFinger == null && touchedFinger.screenPosition.x <= 400 & touchedFinger.screenPosition.y <= 400)
        {
            MovementFinger = touchedFinger;
            MovementAmount = Vector2.zero;
        }
    }

    // Handle joystick finger move
   private void HandleFingerMove(Finger movedFinger)
{
    if (movedFinger == MovementFinger)
    {
        // Get the touch position in screen space
        Vector2 touchPosition = movedFinger.currentTouch.screenPosition;

        // Get the RectTransform of the joystick (this is in screen space)
        RectTransform joystickRect = Joystick.joyStickObj.GetComponent<RectTransform>();

        // Convert touch position to local space relative to the joystick's RectTransform
        Vector2 localTouchPosition = joystickRect.InverseTransformPoint(touchPosition);

        // Calculate the maximum movement radius of the joystick knob (half the width)
        float maxMovement = JoystickSize.x / 2f;

        // Clamp the local position to the joystick's boundaries
        localTouchPosition = Vector2.ClampMagnitude(localTouchPosition, maxMovement);

        // Update the joystick knob position
        Joystick.Knob.anchoredPosition = localTouchPosition;

        // Set the player's movement amount based on the joystick's position
        MovementAmount = new Vector2(localTouchPosition.x / maxMovement, 0f); // Only horizontal movement
    }
}

    // Handle joystick finger up
    private void HandleLoseFinger(Finger lostFinger)
    {
        if (lostFinger == MovementFinger)
        {
            MovementFinger = null;
            Joystick.Knob.anchoredPosition = Vector2.zero;
            MovementAmount = Vector2.zero;
        }
    }

    private void MovePlayer()
    {
        // Smoothly apply horizontal movement
        Vector2 targetVelocity = MovementAmount * moveSpeed;
        moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, 5f * Time.fixedDeltaTime);
        playerRigidbody.linearVelocity = new Vector2(moveVelocity.x, playerRigidbody.linearVelocity.y);

        // Handle sprite flipping
        if (isFacingRight && MovementAmount.x < 0f)
        {
            Flip();
        }
        else if (!isFacingRight && MovementAmount.x > 0f)
        {
            Flip();
        }
    
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
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
        if (isGrounded && !isJumping && !isFalling)
        {
            isJumping = true;
            verticalVelocity = jumpHeight;

            
            if (jumpSound != null)
            {
                AudioSource.PlayClipAtPoint(jumpSound, transform.position, volume);
            }
        }
    }

    private void HandleJump()
{
    if (isJumping)
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
        isFalling = false;
        verticalVelocity = 0f;
    }

    playerRigidbody.gravityScale = isJumping ? 1f : 20f;  // Increase gravity when falling
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
        if (state == "Idle")
        {
            SetAnimation(idle, true, 1f);
        }
        else if (state == "Walking")
        {
            SetAnimation(walking, true, 1f);
        }
        else if (state == "Falling")
        {
            SetAnimation(falling, true, 1f);
        }
        else if (state == "Jumping")
        {
            SetAnimation(jumping, true, 1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
{
    // Check if the player is colliding with a moving platform
    if (collision.gameObject.CompareTag("MovingPlatform"))
    {
        isOnPlatform = true;
        platformRigidbody = collision.rigidbody;
    }
}

// Handle when the player leaves the platform
private void OnCollisionExit2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("MovingPlatform"))
    {
        isOnPlatform = false;
        platformRigidbody = null;
    }
}

}
