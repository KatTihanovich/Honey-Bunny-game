using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    public GateController gate;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок вошел в зону ворот.");
            gate.TryOpenGate(); // Вызываем без аргументов
        }
    }
}
