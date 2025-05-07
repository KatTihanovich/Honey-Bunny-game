using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float intensity = 0.1f;
    public bool IsShaking { get; private set; }

    public void StartShaking()
    {
        IsShaking = true;
    }

    public void StopShaking()
    {
        IsShaking = false;
    }
}
