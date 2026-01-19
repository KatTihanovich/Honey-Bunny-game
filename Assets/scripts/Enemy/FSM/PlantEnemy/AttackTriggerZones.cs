using UnityEngine;

public class AttackTriggerZones : MonoBehaviour
{
    [SerializeField] private PlantAI plantAI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            plantAI.SetPlayerInRange(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            plantAI.SetPlayerInRange(false);
    }
}
