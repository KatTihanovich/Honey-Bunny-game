using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{ 
    private Transform currentCheckpoint;
    private HealthNew playerHealth;
    private UIManager uiManager;

    private void Awake()
    {
        playerHealth = GetComponent<HealthNew>();
    }

    public void CheckRespawn()
    {
        if (currentCheckpoint != null)
        {
            uiManager.GameOver();
            return;
        }
        transform.position = currentCheckpoint.position;
        playerHealth.RestoreFull();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Checkpoint")
        {
            currentCheckpoint = collision.transform;
            collision.GetComponent<Collider2D>().enabled = false;
          
        }
    }
}
