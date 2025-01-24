using UnityEngine;

public class JoyStickMove : MonoBehaviour
{
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

        float moveInputX = movementJoystick.Direction.x;

        if (Mathf.Abs(moveInputX) > 0.1f)
        {
            Debug.Log($"TRUE Joystick Direction: {moveInputX}");
            Vector2 targetVelocity = new Vector2(moveInputX, 0f) * moveSpeed;
            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, 5f * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);
            anim.SetBool("Run", true);
            if (moveInputX > 0 && transform.localScale.x < 0)
            {
                Flip();
            }
            else if (moveInputX < 0 && transform.localScale.x > 0)
            {
                Flip();
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool("Run", false);
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
