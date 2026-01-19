using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    void Awake()
    {
        // Фиксируем FPS на 60
        Application.targetFrameRate = 60;
    }
}
