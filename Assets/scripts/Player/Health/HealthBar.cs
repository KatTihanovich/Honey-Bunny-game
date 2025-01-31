using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image currenthealthBar;

    public void Update()
    {
        // for god sake why it in a Update ?!
        currenthealthBar.fillAmount = playerHealth.CurrentHealth / 10;
    }
}