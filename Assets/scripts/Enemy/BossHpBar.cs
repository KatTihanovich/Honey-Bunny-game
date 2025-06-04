using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class BossHpBar : MonoBehaviour
    {
        [SerializeField] private GameObject hpBarFilled;
        [SerializeField] private HealthNew health;

        private Image hpBarFill;

        private void Start()
        {
            if (health == null)
            {
                Debug.LogError("Health reference is not set on " + gameObject.name);
                return;
            }

            hpBarFill = hpBarFilled.GetComponent<Image>();

            // Подписки на события здоровья
            health.OnDamaged += UpdateHpBar;
            health.OnHealed += UpdateHpBar;
            health.OnDeath += HandleDeath;

           
            UpdateHpBar(0); 
        }

        private void UpdateHpBar(float _)
        {
            if (hpBarFill != null && health != null)
            {
                hpBarFill.fillAmount = health.CurrentHealth / health.MaxHealth;
            }
        }

        private void HandleDeath()
        {
            gameObject.SetActive(false); 
        }

        private void OnDestroy()
        {
           
            if (health != null)
            {
                health.OnDamaged -= UpdateHpBar;
                health.OnHealed -= UpdateHpBar;
                health.OnDeath -= HandleDeath;
            }
        }
    }
}
