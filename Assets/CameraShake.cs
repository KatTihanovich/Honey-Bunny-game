using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float intensity = 0.1f;
    public float speed = 1.0f;

    private Vector3 _originalPosition;
    private bool _isShaking;

    void Start()
    {
        _originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (_isShaking)
        {
            transform.localPosition = _originalPosition + Random.insideUnitSphere * intensity;
        }
    }

    public void StartShaking()
    {
        if (!_isShaking)
        {
            _originalPosition = transform.localPosition;
            _isShaking = true;
        }
    }

    public void StopShaking()
    {
        if (_isShaking)
        {
            _isShaking = false;
            transform.localPosition = _originalPosition;
        }
    }
}
