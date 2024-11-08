using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float moveSped = 5f;

    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private bool _cachedQueryStartInColliders;
    private Vector2 _frameVelocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool JumpDown = Input.GetButtonDown("Jump");
        _frameVelocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSped, _rb.linearVelocity.y);
    }


    void FixedUpdate()
    {
        _rb.linearVelocity = _frameVelocity;
    }
}
