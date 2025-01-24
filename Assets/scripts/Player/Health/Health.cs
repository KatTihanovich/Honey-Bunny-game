using Spine.Unity;
using UnityEngine;
using System.Collections;
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float startingHealth;
    public float CurrentHealth { get; private set; }
    private bool isDead = false;

    public event System.Action<float> OnHealthChanged;



    // Reference to the SkeletonAnimation


    private void Awake()
    {
        CurrentHealth = startingHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // If already dead, no further damage can be taken

        // Reduce health and invoke the health changed event
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, startingHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public void AddHealth(float value)
    {
        if (isDead) return; // No health can be added if the character is dead

        // Increase health and invoke the health changed event
        CurrentHealth = Mathf.Clamp(CurrentHealth + value, 0, startingHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }
    public void Respawn()
    {
        CurrentHealth = startingHealth;
        isDead = false;
        OnHealthChanged?.Invoke(CurrentHealth);
    }
}
