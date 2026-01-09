using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public abstract class RopeBase : MonoBehaviour
{
    protected struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }

    [Header("Rope Settings")]
    public int segmentCount = 35;
    public float segmentLength = 0.25f;
    public Vector2 ropeStartPoint;
    public bool useTransformPosition = true;

    [Header("Physics")]
    public Vector2 gravity = new(0, -9.8f);
    [Range(0f, 1f)] public float damping = 0.995f;
    public int constraintRuns = 50;

    protected List<RopeSegment> ropeSegments;
    protected LineRenderer lineRenderer;

    protected virtual void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        InitializeRope();
    }

    protected virtual void InitializeRope()
    {
        ropeSegments = new List<RopeSegment>();

        if (useTransformPosition)
            ropeStartPoint = transform.position;

        for (int i = 0; i < segmentCount; i++)
        {
            Vector2 pos = ropeStartPoint + Vector2.down * segmentLength * i;
            ropeSegments.Add(new RopeSegment(pos));
        }

        lineRenderer.positionCount = segmentCount;
    }

    protected virtual void FixedUpdate()
    {
        Simulate();

        for (int i = 0; i < constraintRuns; i++)
            ApplyConstraints();

        OnAfterConstraints();
    }

    protected virtual void Simulate()
    {
        for (int i = 0; i < ropeSegments.Count; i++)
        {
            RopeSegment seg = ropeSegments[i];
            Vector2 velocity = (seg.posNow - seg.posOld) * damping;

            seg.posOld = seg.posNow;

            if (i != 0)
                seg.posNow += velocity + gravity * Time.fixedDeltaTime;
            else
                seg.posNow += velocity;

            ropeSegments[i] = seg;
        }
    }

    protected virtual void ApplyConstraints()
    {
        RopeSegment first = ropeSegments[0];
        first.posNow = ropeStartPoint;
        ropeSegments[0] = first;

        for (int i = 0; i < ropeSegments.Count - 1; i++)
        {
            RopeSegment a = ropeSegments[i];
            RopeSegment b = ropeSegments[i + 1];

            Vector2 delta = b.posNow - a.posNow;
            float dist = delta.magnitude;
            float diff = (dist - segmentLength) / dist;

            if (i == 0)
                b.posNow -= delta * diff;
            else
            {
                a.posNow += delta * diff * 0.5f;
                b.posNow -= delta * diff * 0.5f;
            }

            ropeSegments[i] = a;
            ropeSegments[i + 1] = b;
        }
    }

    protected virtual void Update()
    {
        DrawRope();
    }

    protected virtual void DrawRope()
    {
        for (int i = 0; i < ropeSegments.Count; i++)
            lineRenderer.SetPosition(i, ropeSegments[i].posNow);
    }

    protected virtual void OnAfterConstraints() { }
}
