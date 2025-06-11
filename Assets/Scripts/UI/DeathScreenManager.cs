using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeathScreenManager : MonoBehaviour
{
    public GameObject deathScreenUI;
    public GameObject firstSelected; 
    public HealthNew playerHealth;

    private void Start()
    {
        deathScreenUI.SetActive(false);
        playerHealth.OnDeath += ShowDeathScreen;
        
    }

    private void ShowDeathScreen()
    {
        Debug.Log("ShowDeathScreen CALLED");
        EventSystem.current.SetSelectedGameObject(firstSelected);
        deathScreenUI.SetActive(true);
        Time.timeScale = 0f; 
    }
}