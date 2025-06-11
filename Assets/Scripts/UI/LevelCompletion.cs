using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelCompletion : MonoBehaviour
{
    [SerializeField] private GameObject endScreen;
    public GameObject toSelect; 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EventSystem.current.SetSelectedGameObject(toSelect);
            endScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
