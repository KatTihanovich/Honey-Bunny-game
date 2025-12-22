using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RopePendulum : MonoBehaviour
{
    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;
        
        public RopeSegment(Vector2 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }
    
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private LineRenderer lineRenderer;
    
    [Header("Rope Settings")]
    public Vector2 ropeStartPoint = new Vector2(0, 5);
    public bool useTransformPosition = true;
    public int segmentCount = 25;
    public float segmentLength = 0.3f;
    
    [Header("Physics")]
    public Vector2 gravity = new Vector2(0f, -2f);
    public float damping = 0.995f;
    public float verticalDamping = 0.9f; // Дополнительное гашение вертикальной скорости
    public int constraintRuns = 60;
    
    [Header("Platform (Attached Object)")]
    public Transform platform;
    public int platformAttachSegment = 15;
    public float platformMass = 5f;
    public bool limitVerticalMovement = true;
    public float maxVerticalSpeed = 3f;
    
    [Header("Player Interaction")]
    public Transform player;
    public float grabDistance = 2f;
    public KeyCode grabKey = KeyCode.E;
    public KeyCode releaseKey = KeyCode.R;
    public Vector2 handOffset = new Vector2(0, 1);
    public float releaseImpulse = 3f; 
    private Vector2 lastPlayerPosition;
    private bool isGrabbed = false;
    
    [Header("Rope End Visualization")]
    public GameObject ropeEndMarker;
    
    [Header("Collision (Optional)")]
    public bool enableCollisions = false;
    public float collisionRadius = 0.1f;
    public int segmentInterval = 2;
    public float bounceFactor = 0.5f;
    
    void OnEnable()
    {
        if (!Application.isPlaying && ropeSegments.Count == 0)
        {
            InitializeRope();
        }
    }
    
    void Start()
    {
        if (Application.isPlaying)
        {
            InitializeRope();
        }
    }
    
    void InitializeRope()
    {
        ropeSegments.Clear();
        
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
            
        lineRenderer.positionCount = segmentCount;
        
        if (useTransformPosition)
        {
            ropeStartPoint = transform.position;
        }
        
        for (int i = 0; i < segmentCount; i++)
        {
            Vector2 pos = ropeStartPoint - new Vector2(0, segmentLength * i);
            ropeSegments.Add(new RopeSegment(pos));
        }
        
        if (platform != null && platformAttachSegment < segmentCount)
        {
            platform.position = ropeSegments[platformAttachSegment].posNow;
        }
        
        DrawRope();
    }
    
    void OnValidate()
    {
        if (!Application.isPlaying && enabled)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null) InitializeRope();
            };
            #endif
        }
    }
    
    void Update()
    {
        if (!Application.isPlaying)
        {
            DrawRope();
            return;
        }
        
        HandlePlayerInput();

        SyncPlatform();

        DrawRope();
        UpdateRopeEndMarker();
    }
    
    void HandlePlayerInput()
{
    if (player == null) return;
    
    Vector2 ropeEndPos = ropeSegments[segmentCount - 1].posNow;
    float distToEnd = Vector2.Distance(player.position, ropeEndPos);
    
    // Захват
    if (Input.GetKeyDown(grabKey) && distToEnd <= grabDistance && !isGrabbed)
    {
        isGrabbed = true;
        Debug.Log("Grabbed rope end!");
    }
    
    // Отпускание с мощным импульсом
    if (Input.GetKeyDown(releaseKey) && isGrabbed)
    {
        isGrabbed = false;
        
        // Определяем направление броска
        Vector2 throwDirection;
        
        // Проверяем, нажата ли клавиша направления
        float horizontal = Input.GetAxis("Horizontal");
        
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            // Бросаем в сторону нажатой клавиши (A или D)
            throwDirection = new Vector2(horizontal, 0.3f).normalized; // Небольшой подъем вверх
        }
        else
        {
            // Автоматически бросаем в сторону текущего раскачивания
            Vector2 centerPos = ropeSegments[segmentCount / 2].posNow;
            Vector2 endPos = ropeSegments[segmentCount - 1].posNow;
            throwDirection = (endPos - centerPos).normalized;
        }
        
        // Применяем СИЛЬНЫЙ импульс ко всем нижним сегментам
        for (int i = Mathf.Max(0, segmentCount - 12); i < segmentCount; i++)
        {
            RopeSegment segment = ropeSegments[i];
            
            // Применяем импульс напрямую к скорости
            segment.posOld = segment.posNow - throwDirection * releaseImpulse;
            
            ropeSegments[i] = segment;
        }
        
        Debug.Log($"Released with impulse {releaseImpulse} in direction {throwDirection}!");
    }
}

    
    void UpdateRopeEndMarker()
    {
        if (ropeEndMarker != null)
        {
            ropeEndMarker.transform.position = ropeSegments[segmentCount - 1].posNow;
        }
    }
    
    void FixedUpdate()
    {
        if (!Application.isPlaying) return;
        
        Simulate();
        
        for (int i = 0; i < constraintRuns; i++)
        {
            ApplyConstraints();
            
            if (enableCollisions && i % segmentInterval == 0)
            {
                HandleCollisions();
            }
        }
        
        SyncPlatform();
    }
    
    void DrawRope()
    {
        if (lineRenderer == null) return;
        
        Vector3[] ropePositions = new Vector3[segmentCount];
        
        for (int i = 0; i < segmentCount; i++)
        {
            ropePositions[i] = ropeSegments[i].posNow;
        }
        
        lineRenderer.SetPositions(ropePositions);
    }
    
    // РЕШЕНИЕ 3: Разный damping для горизонтальной и вертикальной скорости
    void Simulate()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            RopeSegment segment = ropeSegments[i];
            
            Vector2 velocity = segment.posNow - segment.posOld;
            
            // Разный damping для осей
            velocity.x *= damping; // Горизонтальная скорость сохраняется
            velocity.y *= damping * verticalDamping; // Вертикальная гасится быстрее
            
            segment.posOld = segment.posNow;
            
            float gravityMultiplier = 1f;
            if (i == platformAttachSegment && platform != null)
            {
                gravityMultiplier = platformMass;
            }
            
            segment.posNow += velocity + gravity * gravityMultiplier * Time.fixedDeltaTime;
            
            ropeSegments[i] = segment;
        }
    }
    
    void ApplyConstraints()
    {
        // Закрепить первый сегмент
        RopeSegment firstSegment = ropeSegments[0];
        firstSegment.posNow = ropeStartPoint;
        ropeSegments[0] = firstSegment;
        
        // Если игрок держит веревку, конец следует за игроком
        if (isGrabbed && player != null)
        {
            RopeSegment lastSegment = ropeSegments[segmentCount - 1];
            lastSegment.posNow = (Vector2)player.position + handOffset;
            ropeSegments[segmentCount - 1] = lastSegment;
        }
        
        // Ограничения расстояния
        for (int i = 0; i < segmentCount - 1; i++)
        {
            RopeSegment current = ropeSegments[i];
            RopeSegment next = ropeSegments[i + 1];
            
            float dist = Vector2.Distance(current.posNow, next.posNow);
            float difference = dist - segmentLength;
            Vector2 dir = (current.posNow - next.posNow).normalized;
            Vector2 change = dir * difference;
            
            if (i != 0)
            {
                current.posNow -= change * 0.5f;
                next.posNow += change * 0.5f;
            }
            else
            {
                next.posNow += change;
            }
            
            ropeSegments[i] = current;
            ropeSegments[i + 1] = next;
        }
        
        // Повторно закрепляем к игроку
        if (isGrabbed && player != null)
        {
            RopeSegment lastSegment = ropeSegments[segmentCount - 1];
            lastSegment.posNow = (Vector2)player.position + handOffset;
            ropeSegments[segmentCount - 1] = lastSegment;
        }
    }
    
void SyncPlatform()
{
    if (platform == null) return;
    
    if (platformAttachSegment < segmentCount && platformAttachSegment >= 0)
    {
        // ВСЕГДА синхронизируем платформу с позицией сегмента
        Vector2 newPos = ropeSegments[platformAttachSegment].posNow;
        
        // Убрали ограничение вертикальной скорости - платформа строго следует за сегментом
        platform.position = newPos;
        
        // Вычисляем угол наклона платформы
        if (platformAttachSegment > 0 && platformAttachSegment < segmentCount - 1)
        {
            Vector2 prevPos = ropeSegments[platformAttachSegment - 1].posNow;
            Vector2 nextPos = ropeSegments[platformAttachSegment + 1].posNow;
            Vector2 dir = nextPos - prevPos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
    }
}

    void HandleCollisions()
    {
        for (int i = 1; i < segmentCount; i++)
        {
            RopeSegment segment = ropeSegments[i];
            Vector2 velocity = segment.posNow - segment.posOld;
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(segment.posNow, collisionRadius);
            
            foreach (var col in colliders)
            {
                if (platform != null && col.transform == platform)
                    continue;
                
                Vector2 closestPoint = col.ClosestPoint(segment.posNow);
                float dist = Vector2.Distance(closestPoint, segment.posNow);
                
                if (dist < collisionRadius)
                {
                    Vector2 normal = (segment.posNow - closestPoint).normalized;
                    
                    if (normal == Vector2.zero)
                    {
                        normal = (segment.posNow - (Vector2)col.transform.position).normalized;
                    }
                    
                    segment.posNow = closestPoint + normal * collisionRadius;
                    
                    Vector2 reflectedVelocity = Vector2.Reflect(velocity, normal) * bounceFactor;
                    segment.posOld = segment.posNow - reflectedVelocity;
                }
            }
            
            ropeSegments[i] = segment;
        }
    }
    
    void OnDrawGizmos()
    {
        if (ropeSegments.Count > 0)
        {
            Vector2 ropeEndPos = ropeSegments[segmentCount - 1].posNow;
            
            if (isGrabbed)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.yellow;
            }
            Gizmos.DrawWireSphere(ropeEndPos, 0.3f);
            
            Gizmos.color = isGrabbed ? Color.green : Color.red;
            Gizmos.DrawWireSphere(ropeEndPos, grabDistance);
        }
        
        if (platform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(platform.position, 0.2f);
        }
        
        Gizmos.color = Color.magenta;
        Vector2 anchor = useTransformPosition ? (Vector2)transform.position : ropeStartPoint;
        Gizmos.DrawWireSphere(anchor, 0.2f);
        
        if (ropeSegments.Count > platformAttachSegment)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(ropeSegments[platformAttachSegment].posNow, 0.15f);
        }
    }
}
