using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UltimateCooldown : MonoBehaviour
{
    [Header("Pulse Full Animation")]
    public float scaleAmplitude = 0.1f;      // How much the icon pulses
    public float scaleSpeed = 2.0f;          // Speed of the pulse animation
    private Vector3 initialScale;            // Store original scale for animation reset

    [Header("Full color")]
    public Color fullColor = Color.cyan;     // Color when ultimate is ready

    [Header("Ultimate Settings")]
    public bool isAvailable;                 // Whether ultimate is available
    public int enemyCountToFill = 3;         // How many enemies to defeat to fill ultimate
    private int ultimateProgressCount;       // Track current progress

    private Image ultimateCooldownImage;     // Reference to cooldown UI image


    void Start()
    {
        initialScale = transform.localScale; // Store initial scale for pulse effect
        ultimateCooldownImage = GetComponent<Image>(); // Get the Image component

        SetActive(isAvailable); // Set initial availability state
        ultimateCooldownImage.fillAmount = (float)ultimateProgressCount / enemyCountToFill; // Update visual
    }

    void Update()
    {
        // Pulse animation if ultimate is ready
        if (isAvailable)
        {
            float scaleFactor = 1 + Mathf.Sin(Time.time * scaleSpeed) * scaleAmplitude;
            transform.localScale = initialScale * scaleFactor;
        }
    }

    // Update ultimate availability and color
    private void SetActive(bool isActive)
    {
        if (ultimateCooldownImage != null)
        {
            ultimateCooldownImage.color = isActive ? fullColor : Color.white; // Change color
        }
        isAvailable = isActive; // Update availability flag
    }

    // Call this method to increase ultimate charge (e.g., when an enemy is defeated)
    public void AddPower()
    {
        if (isAvailable) return; // Ignore if already available

        ultimateProgressCount++;
        ultimateCooldownImage.fillAmount = (float)ultimateProgressCount / enemyCountToFill;

        // Activate ultimate if fully charged
        if (ultimateProgressCount >= enemyCountToFill)
        {
            SetActive(true);
        }
    }

    // Trigger the ultimate and reset progress
    public void UsePower()
    {
        if (!isAvailable) return; // Ignore if not available

        // Reset ultimate
        ultimateProgressCount = 0;
        ultimateCooldownImage.fillAmount = 0;

        // Provide feedback (vibration for mobile, can customize for other platforms)
        Handheld.Vibrate();

        // Custom ultimate logic (Add your ultimate attack effect here)
        Debug.Log("Ultimate Activated!");

        // Disable ultimate until recharged
        SetActive(false);
    }
}
