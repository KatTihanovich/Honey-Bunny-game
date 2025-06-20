using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float intensity = 0.1f;
    public bool IsShaking { get; private set; }

    private bool wasShakingBeforePause = false;

    private void Update()
    {
        if (Time.timeScale == 0f && IsShaking)
        {
            wasShakingBeforePause = true;
            StopShaking();
        }
        else if (Time.timeScale > 0f && wasShakingBeforePause)
        {
            StartShaking();
            wasShakingBeforePause = false;
        }
    }

    public void StartShaking()
    {
        IsShaking = true;
    }

    public void StopShaking()
    {
        IsShaking = false;
    }
}
