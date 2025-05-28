using UnityEngine;
using Game.Audio;


[RequireComponent(typeof(Rigidbody2D))]
public class PushableObject : MonoBehaviour
{
    [SerializeField] private float _pushSpeed = 2f;
    private Rigidbody2D _rb;


    private bool _beingPushed;
    private float _pushDir;
    private bool _wasGrounded;
    private bool _wasFalling;
    private ISoundManager _soundManager;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask _groundLayer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.freezeRotation = true;
   
        _rb.linearDamping = 5f;
        _soundManager = SoundManagerNew.Instance;
    }

    private void FixedUpdate()
    {
        if (_beingPushed)
        {
            // Vector2 targetPos = _rb.position + Vector2.right * _pushDir * _pushSpeed * Time.fixedDeltaTime;
            // _rb.MovePosition(targetPos);
        }
        HandleFallingSound();
    }


    public void StartPushing(float direction)
    {
        _beingPushed = true;
        _pushDir = Mathf.Sign(direction);
    }

    public void StopPushing()
    {
        _beingPushed = false;
    }

    private void HandleFallingSound()
    {
        bool isGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _groundLayer);
        bool isFalling = _rb.linearVelocity.y < -0.1f;

        if (!_wasFalling && isFalling && !_wasGrounded)
        {
            _wasFalling = true;
        }

        if (_wasFalling && isGrounded)
        {
            _wasFalling = false;
            _soundManager.PlaySound("StoneFall");
        }

        _wasGrounded = isGrounded;
    }
}
