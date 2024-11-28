using System.Collections;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private float Speed;
    [SerializeField] private float Gravity;
    [SerializeField] private float waitDuration;
    Vector3 targetPos;

    PlayerMovement playerMovement;
    Rigidbody2D rb;
    Vector2 moveDirection;

    Rigidbody2D playerRb;


    public GameObject ways;
    public Transform[] wayPoints;
    int pointIndex;
    int pointCount;
    int direction = 1;

    

    private void Awake()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

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
    }

    // Update is called once per frame
    private void Update()
    {
        if (Vector2.Distance(transform.position, targetPos) < 0.05f)
        {
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
    }

    void DirectionCalculate()
    {
        moveDirection = (targetPos - transform.position).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerMovement.isOnPlatform = true;
            Debug.Log(playerMovement.isOnPlatform);
            // playerMovement.platformRb = rb;
            playerRb.gravityScale = playerRb.gravityScale * Gravity;
            Debug.Log(playerRb.gravityScale);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerMovement.isOnPlatform = false;
            playerRb.gravityScale = playerRb.gravityScale / Gravity;
            Debug.Log(playerRb.gravityScale);
        }
    }
}
