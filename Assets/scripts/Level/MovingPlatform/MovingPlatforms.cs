using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    public enum MoveMode { Horizontal, Vertical }
    public enum StartDirection { Forward, Backward }

    [Header("Movement Settings")]
    [SerializeField] private MoveMode moveMode = MoveMode.Horizontal;
    [SerializeField] private StartDirection startDirection = StartDirection.Forward;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private float waitTime = 0f;

    private Rigidbody2D rb;
    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 currentTarget;
    private Vector2 prevPos;
    private Vector2 platformVelocity;

    private HashSet<Rigidbody2D> carriedBodies = new HashSet<Rigidbody2D>();

    private bool isWaiting = false;
    private float waitTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        Vector2 direction = moveMode == MoveMode.Horizontal ? Vector2.right : Vector2.up;

  
        pointA = rb.position;
        pointB = pointA + direction * moveDistance;

  
        if (startDirection == StartDirection.Forward)
        {
            rb.position = pointA;
            currentTarget = pointB;
        }
        else
        {
            rb.position = pointB;
            currentTarget = pointA;
        }

        prevPos = rb.position;
    }

    private void FixedUpdate()
    {
        if (isWaiting)
        {
            waitTimer += Time.fixedDeltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                currentTarget = (currentTarget == pointA) ? pointB : pointA;
            }
            return;
        }

        Vector2 newPos = Vector2.MoveTowards(rb.position, currentTarget, moveSpeed * Time.fixedDeltaTime);
        platformVelocity = (newPos - prevPos) / Time.fixedDeltaTime;

        rb.MovePosition(newPos);

        foreach (var body in carriedBodies)
        {
            body.MovePosition(body.position + platformVelocity * Time.fixedDeltaTime);
        }

        if (Vector2.Distance(newPos, currentTarget) < 0.01f)
        {
            if (waitTime > 0f)
            {
                isWaiting = true;
            }
            else
            {
                currentTarget = (currentTarget == pointA) ? pointB : pointA;
            }
        }

        prevPos = newPos;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("PlayerFoot"))
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    Rigidbody2D playerRb = collision.rigidbody;
                    if (playerRb != null)
                        carriedBodies.Add(playerRb);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("PlayerFoot"))
        {
            Rigidbody2D playerRb = collision.rigidbody;
            if (playerRb != null)
                carriedBodies.Remove(playerRb);
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 basePos = Application.isPlaying ? pointA : (Vector2)transform.position;
        Vector2 direction = moveMode == MoveMode.Horizontal ? Vector2.right : Vector2.up;
        Vector2 endPos = basePos + direction * moveDistance;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(basePos, endPos);
        Gizmos.DrawWireCube((basePos + endPos) / 2f,
            moveMode == MoveMode.Horizontal ? new Vector2(moveDistance, 0.1f) : new Vector2(0.1f, moveDistance));
    }
}
