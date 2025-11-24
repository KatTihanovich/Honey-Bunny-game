using UnityEngine;

public class WaveMovement : MonoBehaviour
{
    [Header("Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Settings")]
    public float speed = 2f;
    public float waveAmplitude = 1f;   // Глубина волны
    public float waveFrequency = 2f;   // Частота волны
    public bool isActive = false;

    private float t = 0f;

    void Update()
    {
        if (!isActive)
            return;

        // продвижение по траектории
        t += Time.deltaTime * speed;
        t = Mathf.Clamp01(t);

        // движение по прямой
        Vector3 straightPos = Vector3.Lerp(pointA.position, pointB.position, t);

        // волна строго вверх-вниз
        float wave = Mathf.Sin(t * Mathf.PI * waveFrequency) * waveAmplitude;

        // итоговая позиция
        transform.position = new Vector3(
            straightPos.x,
            straightPos.y + wave,
            straightPos.z
        );
    }

    public void Activate()
    {
        isActive = true;
        t = 0f;
    }


    // ----------------------------
    //   Gizmos — визуализация
    // ----------------------------
    private void OnDrawGizmos()
    {
        if (pointA == null || pointB == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pointA.position, pointB.position); // прямая линия

        // Рисуем волнистую траекторию
        Gizmos.color = Color.cyan;
        int segments = 50;
        Vector3 prevPoint = pointA.position;

        for (int i = 1; i <= segments; i++)
        {
            float tt = (float)i / segments;
            Vector3 straightPos = Vector3.Lerp(pointA.position, pointB.position, tt);
            float wave = Mathf.Sin(tt * Mathf.PI * waveFrequency) * waveAmplitude;

            Vector3 wavePos = new Vector3(
                straightPos.x,
                straightPos.y + wave,
                straightPos.z
            );

            Gizmos.DrawLine(prevPoint, wavePos);
            prevPoint = wavePos;
        }
    }
}
