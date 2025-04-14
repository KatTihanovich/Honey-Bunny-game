using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    public enum MoveMode { Horizontal, Vertical }

    [Header("Movement Settings")]
    [SerializeField] private MoveMode moveMode = MoveMode.Horizontal;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 3f;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 prevPos;
    private Vector2 platformVelocity;

    // Игроки, стоящие на платформе
    private HashSet<Rigidbody2D> carriedBodies = new HashSet<Rigidbody2D>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        startPos = rb.position;
        prevPos = startPos;
    }

    private void FixedUpdate()
    {
        // Движение платформы
        Vector2 offset = moveMode switch
        {
            MoveMode.Horizontal => new Vector2(Mathf.PingPong(Time.time * moveSpeed, moveDistance) - moveDistance / 2f, 0f),
            MoveMode.Vertical => new Vector2(0f, Mathf.PingPong(Time.time * moveSpeed, moveDistance) - moveDistance / 2f),
            _ => Vector2.zero
        };

        Vector2 newPos = startPos + offset;
        platformVelocity = (newPos - prevPos) / Time.fixedDeltaTime;

        // Сначала двигаем платформу
        rb.MovePosition(newPos);

        // Затем всех "стоящих" на платформе
        foreach (var body in carriedBodies)
        {
            body.MovePosition(body.position + platformVelocity * Time.fixedDeltaTime);
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
                    {
                        carriedBodies.Add(playerRb);
                    }
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
            {
                carriedBodies.Remove(playerRb);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            startPos = transform.position;

        Gizmos.color = Color.cyan;

        Vector2 endOffset = moveMode == MoveMode.Horizontal
            ? new Vector2(moveDistance / 2f, 0f)
            : new Vector2(0f, moveDistance / 2f);

        Gizmos.DrawLine(startPos - endOffset, startPos + endOffset);
        Gizmos.DrawWireCube(startPos, moveMode == MoveMode.Horizontal
            ? new Vector2(moveDistance, 0.1f)
            : new Vector2(0.1f, moveDistance));
    }
}
