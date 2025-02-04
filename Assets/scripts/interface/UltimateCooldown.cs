using UnityEngine;
using UnityEngine.UI;

public class UltimateCooldown : MonoBehaviour
{
    [Header("Pulse Full Animation")] 
    public float scaleAmplitude = 0.1f;
    public float scaleSpeed = 2.0f;
    private Vector3 initialScale;

    [Header("Full color")] public Color fullColor = Color.cyan;

    [Header("Ultimate")] public bool isAvailable;
    public int enemyCountToFill = 3;
    private int ultimateProgressCount;
    private Image ultimateCooldownImage;
    public GameObject ultButton;
    private Button ultButtonComponent;

    void Start()
    {
        initialScale = transform.localScale;

        ultimateCooldownImage = gameObject.GetComponent<Image>();
        ultButtonComponent = ultButton.GetComponent<Button>();

        SetActive(isAvailable);
        ultimateCooldownImage.fillAmount = (float) ultimateProgressCount / enemyCountToFill;
    }

    void Update()
    {
        if (isAvailable)
        {
            float scaleFactor = 1 + Mathf.Sin(Time.time * scaleSpeed) * scaleAmplitude;
            transform.localScale = initialScale * scaleFactor;
        }
    }

    private void SetActive(bool isActive)
    {
        if (ultButtonComponent != null && ultimateCooldownImage != null)
        {
            ultButtonComponent.interactable = isActive;
            ultimateCooldownImage.color = isActive ? fullColor : Color.white;
        }
        isAvailable = isActive;
    }
    
    public void AddPower()
    {
        ultimateProgressCount += 1;
        ultimateCooldownImage.fillAmount = (float) ultimateProgressCount / enemyCountToFill;

        if (ultimateProgressCount == enemyCountToFill)
        {
            SetActive(true);
        }
    }
    
    public void UsePower()
    {
        ultimateProgressCount = 0;
        ultimateCooldownImage.fillAmount = 0;
        
        Handheld.Vibrate();
        
        SetActive(false);
    }
}