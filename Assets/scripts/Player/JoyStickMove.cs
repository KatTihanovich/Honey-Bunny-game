using UnityEngine;
using UnityEngine.InputSystem;

public class JoyStickMove : MonoBehaviour
{
    private static readonly int Run = Animator.StringToHash("Run");
    public Joystick movementJoystick;
    public float moveSpeed = 12f;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        float horizontalInput = 0;

        if (Keyboard.current.aKey.isPressed)
            horizontalInput = -1;
        if (Keyboard.current.dKey.isPressed)
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
#else
        var moveInputX = movementJoystick.Direction.x;

        if (Mathf.Abs(moveInputX) > 0.1f)
        {
            var targetVelocity = new Vector2(moveInputX, 0f) * moveSpeed;
            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, 5f * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);
            anim.SetBool(Run, true);
            switch (moveInputX)
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
#endif
    }

    private void Flip()
    {
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}