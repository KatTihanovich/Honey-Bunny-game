using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PushableObject : MonoBehaviour
{
    [SerializeField] private float _pushSpeed = 2f;
    private Rigidbody2D _rb;


    private bool _beingPushed;
    private float _pushDir;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.freezeRotation = true;
   
        _rb.linearDamping = 5f;
    }

    private void FixedUpdate()
    {
        if (_beingPushed)
        {

            Vector2 targetPos = _rb.position + Vector2.right * _pushDir * _pushSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(targetPos);
        }
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
}
