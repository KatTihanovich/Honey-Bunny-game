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
    public AnimationReferenceAsset hit, death, idle;

    public event System.Action<float> OnHealthChanged;
    public event System.Action OnDeath;
    private string currentAnimation;

    private void Awake()
    {
        CurrentHealth = startingHealth;
        skeletonAnimation.state.Complete += OnAnimationComplete; // Подписка на событие
    }

    private void OnDestroy()
    {
        skeletonAnimation.state.Complete -= OnAnimationComplete; // Отписка от события
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, startingHealth);
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            isDead = true;
            OnDeath?.Invoke();
            SetAnimation(death, false); // Анимация смерти
        }
        else
        {
            SetAnimation(hit, false); // Анимация удара
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
        SetAnimation(idle, true); // Возвращаем Idle
    }

    private void SetAnimation(AnimationReferenceAsset animation, bool loop)
    {
        if (animation == null || currentAnimation == animation.name)
            return;

        skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = 1f;
        currentAnimation = animation.name;
    }

    private void OnAnimationComplete(Spine.TrackEntry trackEntry)
    {
        if (currentAnimation == hit.name) // Если завершилась анимация удара
        {
            SetAnimation(idle, true); // Возвращаем Idle
        }
    }
}
