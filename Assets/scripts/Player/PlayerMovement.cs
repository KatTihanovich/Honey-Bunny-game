using Spine.Unity;
using UnityEngine;

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

    private Health health;

    public bool isOnPlatform;
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, walking, falling, jumping;
    private string currentAnimation;


    private void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
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
        else if (moveInput.x == 0)
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
        {
            SetCharacterState("Idle");
            Application.targetFrameRate = 60;
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
       // Debug.Log($"CoyoteCounter Decreased: {coyoteCounter}");
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
            SetCharacterState("Jumping");
        }
        else if (isFalling)
        {
            SetCharacterState("Falling");
        }
        else
        {
            SetCharacterState(InputManager.Movement.x != 0 ? "Walking" : "Idle");
        }
    }

    private void FixedUpdate()
    {
        Move(InputManager.Movement);
        IsGrounded();
        Jump();
    }

    public void SetAnimation(AnimationReferenceAsset animation, bool loop, float timescale)
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

}