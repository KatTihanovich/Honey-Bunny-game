using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInputHandler : MonoBehaviour
{
    public float VerticalOffset { get; private set; }

    private const float shiftAmount = 4f;         // Насколько сдвигается камера
    private const float smoothSpeed = 5f;         // Скорость плавного перехода
    private float currentOffset = 0f;

    private void Update()
    {
        if (Keyboard.current == null) return;

        float targetOffset = 0f;

        if (Keyboard.current.wKey.isPressed)
        {
            targetOffset = shiftAmount;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            targetOffset = -shiftAmount;
        }

        // Плавно приближаемся к нужному смещению
        currentOffset = Mathf.Lerp(currentOffset, targetOffset, Time.deltaTime * smoothSpeed);
        VerticalOffset = currentOffset;
    }
}