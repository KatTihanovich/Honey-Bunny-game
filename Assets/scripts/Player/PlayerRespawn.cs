using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    private Transform currentCheckpoint;
    private HealthNew playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<HealthNew>();
    }

    public void CheckRespawn()
    {
        if (currentCheckpoint == null)
        {
   
            UIManager uiManager = FindFirstObjectByType<UIManager>();

            if (uiManager != null)
            {
                uiManager.GameOver();
            }
            else
            {
                Debug.LogWarning("UIManager не найден в сцене!");
            }

            Debug.LogWarning("Нет установленной точки респауна!");
            return;
        }

        StartCoroutine(RespawnWithDelay(2f));
    }


    private IEnumerator RespawnWithDelay(float delay)
    {
      
        yield return new WaitForSeconds(delay);

       
        transform.position = currentCheckpoint.position;
        playerHealth.RestoreFull();

 
        GetComponent<HealthNew>().enabled = true;
        GetComponent<PlayerController>().enabled = true;
        GetComponent<PlayerController>().SetDeadState(false);

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Respawn");
        }

        // Логирование респавна
        Debug.Log("Игрок респавнится через " + delay + " сек");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            Debug.Log("Точка респауна установлена");
            currentCheckpoint = collision.transform;
            collision.GetComponent<Collider2D>().enabled = false; 

        }
    }
}
