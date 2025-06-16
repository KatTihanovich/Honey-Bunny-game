using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    public GateController gate;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            int playerStars = CoinManager.Instance.totalCoins;
            Debug.Log($"Игрок вошел в зону ворот. У него {playerStars} звезд.");
            gate.TryOpenGate(playerStars);
        }
    }
}