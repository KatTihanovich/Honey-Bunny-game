using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
// using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int AttackPressed = Animator.StringToHash("AttackPressed");
    private static readonly int UltimatePressed = Animator.StringToHash("UltimatePressed");
    private static readonly int IsFlying = Animator.StringToHash("IsFlying");
    private static readonly int JumpPressed = Animator.StringToHash("JumpPressed");
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");

    [Header("Audio Settings")] [SerializeField]
    private AudioClip jumpSound;

    [SerializeField] private AudioClip kickSound;
    [SerializeField] private float volume = 1.0f;
    [SerializeField] private AudioMixerGroup audioMixerGroup;

    [Header("UI Elements")] public Button jumpButton;
    public Button kickButton;

    // [Header("Joystick Settings")] 
    // private ETouch.Finger movementFinger;
    // private RectTransform joystickRect;

    [Header("Player Movement")] [SerializeField]
    private float moveSpeed = 12f;

    [Header("Jumping Parameters")] [SerializeField]
    private float jumpBufferTime;

    private float jumpBufferCounter;
    [SerializeField] private float coyoteTime;
    private float coyoteCounter;
    [SerializeField] private float jumpHeight = 17f;

    [Header("Gravity Parameters")] [SerializeField]
    private float gravity = 33f;

    private bool isGrounded;
    private bool isJumping;
    private bool isFalling;

    [Header("What is Ground Parameters")] [SerializeField]
    private LayerMask groundLayer;

    private Rigidbody2D rb;
    private RaycastHit2D groundHit;
    public bool isOnPlatform;

    [Header("Starting point to cast ray to the ground")] [SerializeField]
    private BoxCollider2D boxCollider;

    [Header("Attack Parameters")] [SerializeField]
    private Collider2D playerCollider;

    [SerializeField] private LayerMask entityLayer;
    [SerializeField] private float colliderDistanceX = 1f;
    [SerializeField] private float colliderDistanceY = 0.5f;
    [SerializeField] private float rangeX = 1.5f;
    [SerializeField] private float rangeY = 1f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCd = 10; // CD
    public UltimateCooldown ultimateCooldown;
    private float attackCounter = Mathf.Infinity;

    // hero have a stick
    public bool canAttack;
    
    // will be moved later
    [Header("UI elements")] [SerializeField]
    public Image hitCooldownSprite;

    public Button hitButton;

    // Movement
    private Vector2 moveVelocity;
    private float verticalVelocity;
    private static readonly int Run = Animator.StringToHash("Run");

    // Animation
    private Animator anim;

    private bool isUltimateAttack;

    public bool isHavingStick;
    
    private void Start()
    {
        SetCharacterState(PlayerState.Idle);

        if (Application.isMobilePlatform)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
        EventTrigger trigger = jumpButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        entry.callback.AddListener((data) => { InitiateJump(); });

        trigger.triggers.Add(entry);
        
        // jumpButton.onClick.AddListener(InitiateJump);

        // ETouch.EnhancedTouchSupport.Enable();
        // ETouch.Touch.onFingerDown += HandleFingerDown;
        // ETouch.Touch.onFingerUp += HandleLoseFinger;

        attackCounter = 0f;
        hitButton.interactable = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Move();
        IsGrounded();
        Jump();
    }

    private void Update()
    {
        if (isHavingStick)
        {
            attackCounter += Time.deltaTime;
        }
        
        if (isGrounded && !isJumping && !isFalling)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
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

        if (attackCounter >= attackCd)
        {
            hitButton.interactable = true;
        }

        if (Keyboard.current.fKey.wasPressedThisFrame && attackCounter >= attackCd)
        {
            TryAttack();
        }

        var cooldownFactor = attackCounter / attackCd;

        if (cooldownFactor < 1.1)
        {
            hitCooldownSprite.fillAmount = cooldownFactor;
        }

        if (Keyboard.current.qKey.wasPressedThisFrame && ultimateCooldown.isAvailable)
            {
                StartUltimate();
                ultimateCooldown.UsePower();
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
            hitButton.interactable = false;
        }
    }

    private void IsGrounded()
    {
        //Changed by mentor in Estoty
        Vector2 boxCastOrigin = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(playerCollider.bounds.size.x, 0.1f);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, 0.1f, groundLayer.value);
        isGrounded = groundHit.collider;

        if (groundHit.collider != null && groundHit.collider.TryGetComponent(out DestructiblePlatformController platform))
        {
            platform.StartDestroying();
        }
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

            PlaySound(jumpSound);
        }
    }

    private void Jump()
    {
        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;

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

    private void Move()
    {
        float horizontalInput = 0;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            horizontalInput = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed
)
            horizontalInput = 1;

        if (horizontalInput is > 0 or < 0)
        {
            var targetVelocity = new Vector2(horizontalInput, 0f) * moveSpeed;
            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, 5f * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);
            
            anim.SetBool(Run, true);
            switch (horizontalInput)
            {
                case > 0 when transform.localScale.x < 0:
                case < 0 when transform.localScale.x > 0:
                    Flip();
                    break;
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool(Run, false);
        }
    }

    private void Flip()
    {
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void StartAttack()
    {
        attackCounter = 0;
        PlaySound(kickSound);
    }

    public void StartUltimate()
    {
        isUltimateAttack = true;
        anim.SetTrigger(UltimatePressed);
        PlaySound(kickSound);
    }

    public void ApplyUltimateDamage()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            boxCollider.bounds.center + transform.up * (transform.localScale.y * colliderDistanceY) +
            transform.right * (transform.localScale.x * colliderDistanceX),
            new Vector2(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY),
            0, entityLayer);

        foreach (var hit in hits)
        {
            if (hit && hit.CompareTag("Enemy"))
            {
                Health entityHealth = hit.GetComponent<Health>();
                if (entityHealth)
                {
                    entityHealth.TakeDamage(damage + damage);
                }
            }
        }

        isUltimateAttack = false;
    }

    public void ApplyAreaDamage()
    {
        // this function uses AnimatedEvent, so I used bool flag to prevent calling this function on ultimate
        if (isUltimateAttack) return;
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            boxCollider.bounds.center + transform.up * (transform.localScale.y * colliderDistanceY) +
            transform.right * (transform.localScale.x * colliderDistanceX),
            new Vector2(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY),
            0, entityLayer);

        foreach (var hit in hits)
        {
            if (hit && hit.CompareTag("Enemy"))
            {
                Health entityHealth = hit.GetComponent<Health>();
                if (entityHealth)
                {
                    entityHealth.TakeDamage(damage);
                }
            }
        }
    }


    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            GameObject tempAudio = new GameObject("TempAudio");
            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.Play();
            Destroy(tempAudio, clip.length);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnStickCollected()
    {
        print("OnStickCollected!!!!!");
        isHavingStick = true;
    }
}