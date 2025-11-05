using UnityEngine;

public class FrameRateController : MonoBehaviour
{
    public int targetFrameRate = 120;

    void Awake()
    {
        Application.targetFrameRate = targetFrameRate;

        if (Application.targetFrameRate != targetFrameRate)
        {
            Debug.LogWarning("Failed to set target frame rate. The platform may not support changing frame rate at runtime.");
        }
        else
        {
            Debug.Log("Target frame rate set to: " + targetFrameRate);
        }
    }
}
