using UnityEngine;

public class ClimbingRope : RopeBase
{
    [Header("Player Interaction")]
    public Transform player;
    public float collisionRadius = 0.15f;
    [Range(0f, 1f)] public float collisionDamping = 0.2f;
    public float climbSpeed = 5f;
    [Range(0.05f, 0.5f)] public float smoothSpeed = 0.15f;
    public KeyCode grabKey = KeyCode.E;

    [Header("Rope Collision")]
    public float ropeThickness = 0.1f;
    public int collisionCheckInterval = 2;

    private bool playerGrabbed;
    private float playerSegmentPosition;

    private Rigidbody2D playerRb;
    private Collider2D playerCollider;

    [Header("Grab Zone")]
    public Vector2 grabZoneSize = new(2f, 4f);
    public Vector2 grabZoneOffset = Vector2.zero;
    public Color grabZoneColor = new(0f, 1f, 0f, 0.25f);

    protected override void Awake()
    {
        base.Awake();

        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            playerCollider = player.GetComponent<Collider2D>();
        }
    }

    protected override void Update()
    {
        HandleGrabInput();
        base.Update();
    }

    protected override void OnAfterConstraints()
    {
        if (!playerGrabbed)
            ResolveRopeCollisions();
        else
            HandlePlayerOnRope();
    }


    void HandleGrabInput()
    {
        if (player == null)
            return;

        if (!IsPlayerInGrabZone())
            return;

        if (Input.GetKeyDown(grabKey))
        {
            float minDist = float.MaxValue;

            for (int i = 0; i < ropeSegments.Count; i++)
            {
                float dist = Vector2.Distance(player.position, ropeSegments[i].posNow);
                if (dist < minDist)
                {
                    minDist = dist;
                    playerSegmentPosition = i;
                }
            }

            playerGrabbed = true;
        }
        else if (Input.GetKeyUp(grabKey))
        {
            playerGrabbed = false;
        }
    }


    void ResolveRopeCollisions()
    {
        if (playerCollider == null)
            return;

        for (int i = 1; i < ropeSegments.Count; i += collisionCheckInterval)
        {
            RopeSegment seg = ropeSegments[i];

            Vector2 closest = playerCollider.ClosestPoint(seg.posNow);
            float dist = Vector2.Distance(seg.posNow, closest);

            if (dist < ropeThickness)
            {
                Vector2 normal = (seg.posNow - closest).normalized;

                if (normal == Vector2.zero)
                    normal = (seg.posNow - (Vector2)player.position).normalized;

                seg.posNow = closest + normal * ropeThickness;

                Vector2 velocity = seg.posNow - seg.posOld;
                Vector2 reflected = Vector2.Reflect(velocity, normal) * collisionDamping;

                seg.posOld = seg.posNow - reflected;
                ropeSegments[i] = seg;
            }
        }
    }


    void HandlePlayerOnRope()
    {
        float vertical = Input.GetAxisRaw("Vertical");

        playerSegmentPosition -= vertical * climbSpeed * Time.fixedDeltaTime;
        playerSegmentPosition = Mathf.Clamp(
            playerSegmentPosition, 0f, ropeSegments.Count - 1f
        );

        Vector2 targetPos = GetInterpolatedPosition(playerSegmentPosition);
        Vector2 smoothed = Vector2.Lerp(player.position, targetPos, smoothSpeed);

        if (playerRb != null)
        {
            playerRb.MovePosition(smoothed);
            playerRb.linearVelocity = Vector2.zero;
        }
        else
        {
            player.position = smoothed;
        }
    }

    Vector2 GetInterpolatedPosition(float position)
    {
        int a = Mathf.FloorToInt(position);
        int b = Mathf.CeilToInt(position);

        a = Mathf.Clamp(a, 0, ropeSegments.Count - 1);
        b = Mathf.Clamp(b, 0, ropeSegments.Count - 1);

        if (a == b)
            return ropeSegments[a].posNow;

        float t = position - a;
        return Vector2.Lerp(ropeSegments[a].posNow, ropeSegments[b].posNow, t);
    }

    bool IsPlayerInGrabZone()
    {
        if (player == null)
            return false;

        Vector2 center = (Vector2)transform.position + grabZoneOffset;
        Vector2 half = grabZoneSize * 0.5f;

        Vector2 p = player.position;

        return p.x >= center.x - half.x &&
            p.x <= center.x + half.x &&
            p.y >= center.y - half.y &&
            p.y <= center.y + half.y;
    }
    

    void OnDrawGizmosSelected()
    {
        Vector2 center = (Vector2)transform.position + grabZoneOffset;

        Gizmos.color = grabZoneColor;
        Gizmos.DrawCube(center, grabZoneSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, grabZoneSize);
    }
}
