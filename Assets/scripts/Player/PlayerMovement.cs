using Spine.Unity;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;

    private bool grounded;  //Temp fix
    private Rigidbody2D body; 
    private BoxCollider2D boxCollider;

    public bool isOnPlatform;
    public Rigidbody2D platformRb;
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, walking;
    public string currentState;
    public string currentAnimation;


    private void Start()
    {
        {
            currentState = "Idle";
            SetCharacterState(currentState);
        }
    }
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {

        float horizontalInput = Input.GetAxis("Horizontal");
        body.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.linearVelocity.y);

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Сохраняем текущий Y и Z
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Сохраняем текущий Y и Z



        if (horizontalInput !=0)
        {
            SetCharacterState ("Walking");
        }
        else
        {
            SetCharacterState("Idle");
        }

        if (Input.GetKey(KeyCode.Space) && grounded)
            Jump();

    }

    private void FixedUpdate()
    {
        if (isOnPlatform)
        {
            body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed + platformRb.linearVelocity.x, body.linearVelocity.y);
        }
        else
        {
           body.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.linearVelocity.y);
        }
    }

    private void Jump()
    {

        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
        grounded = false;
        //if (isGrounded())
        //{
        //    body.velocity = new Vector2(body.linearVelocity.x, jumpPower);
        //}

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground") ;
            grounded = true;
    }
    //private bool isGrounded()
    //{
    //    RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
    //    return raycastHit.collider != null;
    //}
    public void SetAnimation(AnimationReferenceAsset animation, bool loop, float timescale)
    {
        if (animation.name.Equals(currentAnimation))
        {
            return;
        }
        skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timescale;
        currentAnimation = animation.name;
    }

    public void SetCharacterState(string state)
    {
        if(state.Equals("Idle"))
        {
            SetAnimation(idle, true, 1f);
        }
        else if (state.Equals("Walking"))
        {
            SetAnimation(walking, true, 1f);
        }
    }
}
