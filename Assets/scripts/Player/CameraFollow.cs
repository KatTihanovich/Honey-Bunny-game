using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private readonly Vector3 offset = new(0f, 3f, -25f);
    private const float SmoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;
    private CameraShake cameraShake; 

    private void Awake()
    {
        cameraShake = GetComponent<CameraShake>();

        if (cameraShake == null)
        {
            cameraShake = FindObjectOfType<CameraShake>();
        }

        if (cameraShake == null)
        {
            Debug.LogWarning("CameraShake component not found in the scene.");
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            var targetPosition = target.position + offset;
            Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);

            if (cameraShake.IsShaking)
            {
                smoothPosition += Random.insideUnitSphere * cameraShake.intensity;
            }

            transform.position = smoothPosition;
        }
    }
}
