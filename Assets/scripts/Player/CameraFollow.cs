using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private readonly Vector3 offset = new(0f, 3f, -25f);
    private const float SmoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;

    private void Update()
    {
        if (target != null)
        {
            var targetPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);
        }
    }
}
