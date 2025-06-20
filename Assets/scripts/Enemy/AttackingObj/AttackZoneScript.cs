using UnityEngine;

public class AttackTriggerZone : MonoBehaviour
{
    [SerializeField] private MonsterAI monsterAI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            monsterAI.SetPlayerInRange(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            monsterAI.SetPlayerInRange(false);
        }
    }
}
