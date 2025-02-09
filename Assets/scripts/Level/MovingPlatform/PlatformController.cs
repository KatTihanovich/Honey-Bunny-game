using System.Collections;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private float Speed;
    [SerializeField] private float waitDuration;
    Vector3 targetPos;
    Rigidbody2D rb;
    Vector2 moveDirection;
    private Vector3 previousPosition;



    public GameObject ways;
    public Transform[] wayPoints;
    int pointIndex;
    int pointCount;
    int direction = 1;

    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        wayPoints = new Transform[ways.transform.childCount];
        for(int i = 0; i < ways.gameObject.transform.childCount; i++)
        {
            wayPoints[i] = ways.transform.GetChild(i).gameObject.transform;
        }
    }

    private void Start()
    {
        pointIndex = 1;
        pointCount = wayPoints.Length;
        targetPos = wayPoints[1].transform.position;
        DirectionCalculate();
        previousPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
    // Move the platform towards the target at each physics step (no overshoot).
    float step = Speed * Time.fixedDeltaTime;
    transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

    // Check if we arrived at or passed the target
    if (Vector3.Distance(transform.position, targetPos) < 0.001f)
    {
        // Force it exactly on the waypoint
        transform.position = targetPos;
        NextPoint();
    }
}

    void NextPoint()
    {
        transform.position = targetPos;
        moveDirection = Vector3.zero;

        if (pointIndex == pointCount - 1)
        {
            direction = -1;
        }

        if (pointIndex == 0)
        {
            direction = 1;
        }

        pointIndex += direction;
        targetPos = wayPoints[pointIndex].transform.position;
        StartCoroutine(WaitNextPoint());
    }

    IEnumerator WaitNextPoint()
    {
        yield return new WaitForSeconds(waitDuration);
        DirectionCalculate();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * Speed;
        if (rb != null)
        {
            Vector3 deltaPosition = transform.position - previousPosition;

            foreach (Transform child in transform)
            {
                if (child.CompareTag("Player") && child.parent == transform)
                {
                    child.position += deltaPosition; // ������� ������ ������ ���� �� �� ��� �������� ������
                }
            }

            previousPosition = transform.position; // �������� ���������� ���������
        }
    }

    void DirectionCalculate()
    {
        moveDirection = (targetPos - transform.position).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Player"))
        //{
        //    Transform playerRoot = collision.transform.root;

        //    playerRoot.SetParent(transform);
        //    Debug.Log("Player attached to platform");
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Transform playerRoot = collision.transform.root;
            playerRoot.SetParent(null, false); // ���������� ������
            Debug.Log("Player detached from platform");
        }
    }
}
