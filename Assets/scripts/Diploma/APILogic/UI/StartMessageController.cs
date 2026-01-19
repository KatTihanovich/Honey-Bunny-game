using UnityEngine;

public class StartMessageController : MonoBehaviour
{
    private static bool wasShown = false;

    [SerializeField] private GameObject startMessageCanvas;
    [SerializeField] private GameObject menuCanvas;

    void Start()
    {
        if (wasShown)
        {
            startMessageCanvas.SetActive(false);
            menuCanvas.SetActive(true);
        }
        else
        {
            startMessageCanvas.SetActive(true);
            menuCanvas.SetActive(false);
            wasShown = true;
        }
    }
}
