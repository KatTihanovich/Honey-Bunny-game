using UnityEngine;

public class FocusTrigger : MonoBehaviour
{
    public Transform focusObject; 
    public FocusSettingsForObject focusSettings;
    private bool hasShownObstacle = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Entered trigger with: {other.name}");
        if (other.CompareTag("Player"))
        {
            if (!hasShownObstacle)
            {
                hasShownObstacle = true;
                CameraFocus cam = Camera.main.GetComponent<CameraFocus>();
                StartCoroutine(cam.FocusOnObject(focusObject, focusSettings));
            }
        }
    }
}
