using UnityEngine;
using UnityEngine.UI;

public class LevelCompletion : MonoBehaviour
{
    [SerializeField] private GameObject endScreen;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            endScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
