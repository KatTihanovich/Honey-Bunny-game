using Spine.Unity;
using UnityEngine;

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

    private void Awake()
    {
        CurrentHealth = startingHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, startingHealth);
        OnHealthChanged?.Invoke(CurrentHealth);

        SetAnimation(hit, false, 1f);

        if (CurrentHealth <= 0 && !isDead)
        {
            SetAnimation(death, false, 1f);
            isDead = true;
            OnDeath?.Invoke();
        }
    }

    public void AddHealth(float value)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + value, 0, startingHealth);
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    public bool IsDead() => isDead;

    public void Respawn()
    {
        CurrentHealth = startingHealth;
        isDead = false;
        OnHealthChanged?.Invoke(CurrentHealth);
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


