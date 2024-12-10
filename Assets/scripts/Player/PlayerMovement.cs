using Spine.Unity;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float jumpHeight = 17f;

    [SerializeField] private float gravity = 33f;

    // [SerializeField] private float jumpBufferTime;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    [SerializeField] private Collider2D collider;

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

    public bool isOnPlatform;
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, walking, falling, jumping;
    public string currentState;
    public string currentAnimation;

    private void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
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
            currentState = "Idle";
            SetCharacterState(currentState);
        }
    }

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(collider.bounds.center.x, collider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(collider.bounds.size.x, 0.1f);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, 0.1f, groundLayer.value);
        isGrounded = groundHit.collider;
    }

    private void InitiateJump()
    {
        if (isGrounded && !isJumping && !isFalling)
        {
            isJumping = true;
            // var grav = -(2f * jumpHeight) / Mathf.Pow(0.35f, 2f);
            // verticalVelocity = Mathf.Abs(grav) * 0.35f;
            verticalVelocity = jumpHeight;
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

        // if (isJumping && rb.linearVelocity.y <= 0)
        // {
        //     isJumping = false;
        // }
        // if (!isGrounded)
        // {
        //     rb.linearVelocity += new Vector2(0, -gravity * Time.fixedDeltaTime);
        // }
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalVelocity);
    }

    private void Update()
    {
        if (InputManager.JumpWasPressed)
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


        // if (isOnPlatform)
        // {
        //     rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * walkSpeed + platformRb.linearVelocity.x, rb.linearVelocity.y);
        // }
        // else
        // {
        //    rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * walkSpeed, rb.linearVelocity.y);
        // }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if (collision.gameObject.tag == "Ground") ;
        // grounded = true;
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
            SetAnimation(falling, true, 0.1f);
        }
        else if (state.Equals("Jumping"))
        {
            SetAnimation(jumping, false, 0.1f);
        }
    }
}