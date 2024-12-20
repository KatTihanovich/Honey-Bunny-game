using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Vector2 JoystickSize = new Vector2(200, 200); // Size of the joystick
    public JoyStick Joystick; // Reference to joystick UI
    public Rigidbody2D playerRigidbody; // Player's Rigidbody2D
    private Finger MovementFinger; // Finger tracking the joystick
    public Vector2 MovementAmount; // Normalized movement direction
    public float playerSpeed = 5f; // Movement speed
    public Animator playerAnimator; // Animator for the player

    private float runThreshold = 0.5f; // Threshold for triggering running animation

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        
        // Set the joystick position to (250, 240) in screen space
        SetJoystickPosition(new Vector2(250, 240));
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }

    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleLoseFinger;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();
    }

    // Set the joystick's position to a fixed location on the screen (250, 240)
    private void SetJoystickPosition(Vector2 position)
    {
        RectTransform joystickRect = Joystick.joyStickObj.GetComponent<RectTransform>();

        // Set the anchor to bottom-left (0, 0)
        joystickRect.anchorMin = new Vector2(0, 0);
        joystickRect.anchorMax = new Vector2(0, 0);
        
        // Set the position to the desired screen coordinates (250, 240)
        joystickRect.anchoredPosition = position;
    }

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

            // Calculate the maximum movement radius of the joystick (half the width)
            float maxMovement = JoystickSize.x / 2f;

            // Restrict to horizontal movement (y will be set to 0)
            localTouchPosition.y = 0f;

            // Clamp the horizontal position to stay within the joystick's bounds
            if (Mathf.Abs(localTouchPosition.x) > maxMovement)
            {
                localTouchPosition.x = Mathf.Sign(localTouchPosition.x) * maxMovement;
            }

            // Update the joystick knob position based on the calculated local position
            Joystick.Knob.anchoredPosition = localTouchPosition;

            // Set the player's movement based on the horizontal position of the knob
            MovementAmount = new Vector2(localTouchPosition.x / maxMovement, 0f);
        }
    }

    private void HandleFingerDown(Finger touchedFinger)
    {
        if (MovementFinger == null && touchedFinger.screenPosition.x <= Screen.width)
        {
            MovementFinger = touchedFinger;
            MovementAmount = Vector2.zero;
        }
    }

    private void HandleLoseFinger(Finger lostFinger)
    {
        if (lostFinger == MovementFinger)
        {
            MovementFinger = null;
            Joystick.Knob.anchoredPosition = Vector2.zero; // Reset knob
            MovementAmount = Vector2.zero;                // Reset movement
        }
    }

    void FixedUpdate()
    {
        // Move the player based on joystick input
        Vector2 movement = MovementAmount * playerSpeed;
        playerRigidbody.linearVelocity = movement;

        // Update animator parameters for movement
        playerAnimator.SetFloat("moveX", MovementAmount.x);
        playerAnimator.SetFloat("moveY", MovementAmount.y);
        playerAnimator.SetBool("isMoving", MovementAmount != Vector2.zero); // Set idle/moving animation

        // Trigger the running animation if the player is moving fast enough
        if (MovementAmount.magnitude > runThreshold) 
        {
            playerAnimator.SetBool("isRunning", true); // Set running animation
        }
        else 
        {
            playerAnimator.SetBool("isRunning", false); // Set idle/walking animation
        }
    }
}
