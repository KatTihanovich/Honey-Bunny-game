using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SlowField : MonoBehaviour
{
    [Range(0f, 1f)] public float slowFactor = 0.3f; // во сколько раз замедлять падение
    public float extraDrag = 5f;                     // сопротивление воздуха внутри поля

    private void OnTriggerEnter(Collider other)
    {
        // Игнорируем самого игрока
        if (other.CompareTag("Player")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            // уменьшаем скорость падения
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * slowFactor, rb.linearVelocity.z);

            // усиливаем сопротивление воздуха
            rb.linearDamping = extraDrag;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            // возвращаем нормальное сопротивление воздуха
            rb.linearDamping = 0f;
        }
    }
}
