using UnityEngine;

public class InfoTrigger : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject infoPanel;

    private void Start()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            ShowInfo();
        }
    }

    private void ShowInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
            Time.timeScale = 0; 
        }
    }

    public void ClosePanel()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
            Time.timeScale = 1; 
        }
    }
}
