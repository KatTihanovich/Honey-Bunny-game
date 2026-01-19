using UnityEngine;

public class RopePendulum : RopeBase
{
    [Header("Platform")]
    public Transform platform;
    public int platformAttachSegment = 25;

    [Header("Player Interaction")]
    public Transform player;
    public float grabDistance = 3f;
    public KeyCode grabKey = KeyCode.E;
    public KeyCode releaseKey = KeyCode.R;
    public Vector2 handOffset = new(0, 1.5f);
    public float releaseImpulse = 2.5f;

    private bool isGrabbed;

    protected override void Update()
    {
        if (Application.isPlaying)
            HandlePlayerInput();

        base.Update(); 
    }

    void HandlePlayerInput()
    {
        if (player == null || ropeSegments == null || ropeSegments.Count == 0)
            return;

        Vector2 ropeEnd = ropeSegments[^1].posNow;
        float dist = Vector2.Distance(player.position, ropeEnd);

        if (Input.GetKeyDown(grabKey) && dist <= grabDistance && !isGrabbed)
        {
            isGrabbed = true;
        }

        if (Input.GetKeyDown(releaseKey) && isGrabbed)
        {
            isGrabbed = false;

            Vector2 center = ropeSegments[ropeSegments.Count / 2].posNow;
            Vector2 dir = (ropeEnd - center).normalized;

            for (int i = ropeSegments.Count - 6; i < ropeSegments.Count; i++)
            {
                RopeSegment seg = ropeSegments[i];
                seg.posOld = seg.posNow - dir * releaseImpulse;
                ropeSegments[i] = seg;
            }
        }
    }

    protected override void ApplyConstraints()
    {
        base.ApplyConstraints();

        if (isGrabbed && player != null)
        {
            RopeSegment end = ropeSegments[^1];
            end.posNow = (Vector2)player.position + handOffset;
            ropeSegments[^1] = end;
        }
    }

    protected override void OnAfterConstraints()
    {
        if (platform != null &&
            platformAttachSegment >= 0 &&
            platformAttachSegment < ropeSegments.Count)
        {
            platform.position = ropeSegments[platformAttachSegment].posNow;
        }
    }

    void OnDrawGizmos()
    {
        if (ropeSegments == null || ropeSegments.Count == 0)
            return;

        Gizmos.color = isGrabbed ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(ropeSegments[^1].posNow, grabDistance);
    }
}
