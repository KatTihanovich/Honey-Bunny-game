using UnityEngine;
using Spine.Unity;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI; 
using UnityEngine.Playables;

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

    [Header("Attack Parameters")]
    [SerializeField] private BoxCollider2D boxCollider; // Êîëëàéäåð äëÿ îïðåäåëåíèÿ öåíòðà ïðîâåðêè
    [SerializeField] private LayerMask entityLayer; // Ñëîé äëÿ ôèëüòðàöèè ñóùíîñòåé
    [SerializeField] private float colliderDistanceX = 1f; // Ñìåùåíèå ïî X
    [SerializeField] private float colliderDistanceY = 0.5f; // Ñìåùåíèå ïî Y
    [SerializeField] private float rangeX = 1.5f; // Øèðèíà îáëàñòè
    [SerializeField] private float rangeY = 1f; // Âûñîòà îáëàñòè
    [SerializeField] private int damage = 10; // Óðîí
    [SerializeField] private float attackCD = 10; // Óðîí
    private float attackCounter = Mathf.Infinity;
    private Health entityHealth;


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
    private bool isAttacking = false; 

    //health
    private Health health;

    private PlayerState currentState;

    [Header("Animation")]
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, walking, falling, jumping, attacking;
    private string currentAnimation;

    //platform
    public bool isOnPlatform;

     private void Awake()
    {
        isFacingRight = true;
        playerRigidbody = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        currentState = PlayerState.Idle;
        //audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        jumpButton.onClick.AddListener(InitiateJump);
        joystickRect = Joystick.joyStickObj.GetComponent<RectTransform>();

        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger;
        ETouch.Touch.onFingerMove += HandleFingerMove;

        SetCharacterState(PlayerState.Idle);
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
        attackCounter += Time.fixedDeltaTime;
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

        if (InputManager.AttackWasPressed) //&& attackCounter >= attackCD)
        {
            attackCounter = 0;
            StartAttack();
        }

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
            SetCharacterState(PlayerState.Jumping);
        }
        else if (isFalling)
        {
            SetCharacterState(PlayerState.Falling);
        }
        else
        {
            SetCharacterState(MovementAmount.x != 0 ? PlayerState.Walking : PlayerState.Idle);
        }
    }

    public void SetAnimation(AnimationReferenceAsset animation, bool loop, float timescale)
    {
        skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timescale;
    }


    private void SetCharacterState(PlayerState state)
    {
        // Íå ìåíÿòü ñîñòîÿíèå, åñëè óæå â àòàêå
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
                isAttacking = true; // Óñòàíàâëèâàåì ôëàã àòàêè
                SetAnimation(attacking, false, 1.7f);
                skeletonAnimation.state.GetCurrent(0).Complete += OnAttackAnimationComplete;
                break;
        }
    }

    private void OnAttackAnimationComplete(Spine.TrackEntry trackEntry)
    {
        // Ñíèìàåì ôëàã àòàêè
        isAttacking = false;

        // Âîçâðàùàåì ñîñòîÿíèå â Idle èëè Walking
        SetCharacterState(MovementAmount.x != 0 ? PlayerState.Walking : PlayerState.Idle);
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

    public void ApplyAreaDamage()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            boxCollider.bounds.center + transform.up * transform.localScale.y * colliderDistanceY + transform.right * transform.localScale.x * colliderDistanceX,
            new Vector2(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY),
            0, entityLayer); // Èñïîëüçóåì OverlapBoxAll âìåñòî BoxCastAll

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
        //audioManager.PlaySFX(audioManager.playerAttack);
        SetCharacterState(PlayerState.Attacking);

        // Ïîñëå çàâåðøåíèÿ àíèìàöèè àòàêè, ñáðàñûâàåì ôëàã
        skeletonAnimation.state.Complete += OnAttackAnimationComplete;
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.color = Color.red;
    //     Vector3 boxSize = new Vector3(
    //         boxCollider.bounds.size.x * rangeX,
    //         boxCollider.bounds.size.y * rangeY,
    //         boxCollider.bounds.size.z);
    //     Vector3 boxCenter = boxCollider.bounds.center +
    //         transform.up * transform.localScale.y * colliderDistanceY +
    //         transform.right * transform.localScale.x * colliderDistanceX;

    //     Gizmos.DrawWireCube(boxCenter, boxSize);
    // }
    public enum PlayerState
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Attacking
    }
}