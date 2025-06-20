using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatformForButton : MonoBehaviour
{
    public enum MoveMode { Horizontal, Vertical }

    [Header("Movement Settings")]
    [SerializeField] private MoveMode moveMode = MoveMode.Horizontal;
    [SerializeField] private float moveSpeedToB = 2f;
    [SerializeField] private float moveSpeedToA = 2f;
    [SerializeField] private float moveDistance = 310f;

    private Rigidbody2D rb;
    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 currentTarget;
    private Vector2 prevPos;
    private Vector2 platformVelocity;

    private HashSet<Rigidbody2D> carriedBodies = new HashSet<Rigidbody2D>();

    private bool isMoving = false;
    private bool movingToB = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        Vector2 direction = moveMode == MoveMode.Horizontal ? Vector2.right : Vector2.up;
        pointA = rb.position;
        pointB = pointA + direction * moveDistance;
        prevPos = rb.position;
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;

        float speed = movingToB ? moveSpeedToB : moveSpeedToA;
        Vector2 target = movingToB ? pointB : pointA;

        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        platformVelocity = (newPos - prevPos) / Time.fixedDeltaTime;

        rb.MovePosition(newPos);

        foreach (var body in carriedBodies)
        {
            body.MovePosition(body.position + platformVelocity * Time.fixedDeltaTime);
        }

        if (Vector2.Distance(newPos, target) < 0.01f)
        {
            isMoving = false; // останавливаемся при достижении цели
        }

        prevPos = newPos;
    }

    public void SetDirection(bool toB)
    {
        if (movingToB != toB || !isMoving)
        {
            movingToB = toB;
            isMoving = true;
        }
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

        Gizmos.color = Color.green;
        Gizmos.DrawLine(basePos, endPos);
        Gizmos.DrawWireCube((basePos + endPos) / 2f,
            moveMode == MoveMode.Horizontal ? new Vector2(moveDistance, 0.1f) : new Vector2(0.1f, moveDistance));
    }
}
