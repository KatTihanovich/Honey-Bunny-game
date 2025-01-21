using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float startingHealth;
    public float CurrentHealth { get; private set; }
    private bool isDead = false;

    [Header("Animation Settings")]
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset hit, death;

    public event System.Action<float> OnHealthChanged;
    public event System.Action OnDeath;
    private string currentAnimation;

    // Reference to the RestartWindow script
    public RestartWindow restartWindow;

    private void Awake()
    {
        CurrentHealth = startingHealth;
        // Ensure we have a reference to the RestartWindow
        if (restartWindow == null)
        {
            restartWindow = FindObjectOfType<RestartWindow>();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return; // Player is dead, no more damage can be taken
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, startingHealth);
        OnHealthChanged?.Invoke(CurrentHealth);

        SetAnimation(hit, false, 1f);

        if (CurrentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        SetAnimation(death, false, 1f);
        isDead = true;
        OnDeath?.Invoke();

        // Show the restart window when the player dies
        if (restartWindow != null)
        {
            restartWindow.ShowRestartWindow();
        }
    }

    public void AddHealth(float value)
    {
        if (isDead) return; // Can't add health if the player is dead

        CurrentHealth = Mathf.Clamp(CurrentHealth + value, 0, startingHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public bool IsDead() => isDead;

    public void Respawn()
    {
        if (!isDead) return;

        CurrentHealth = startingHealth;
        isDead = false;
        OnHealthChanged?.Invoke(CurrentHealth);

        // Optionally, reset the player's position, animation, etc.
        // Reset animations, etc.
        SetAnimation(hit, false, 1f); // You can change this to any other animation
    }

    private void SetAnimation(AnimationReferenceAsset animation, bool loop, float timescale)
    {
        if (animation.name.Equals(currentAnimation))
        {
            return;
        }

        skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timescale;
        currentAnimation = animation.name;
    }
}
