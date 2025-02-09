using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class BossHpBar : MonoBehaviour
    {
        public GameObject hpBarFilled;
        public Health health;
        
        public float maxHP = 5f;
        
        private Image hpBarFill;
        
        void Start()
        {
            if (health != null)
            {
                health.OnHealthChanged += HandleHealthChanged;
            }

            hpBarFill = hpBarFilled.GetComponent<Image>();
        }
        
        private void HandleHealthChanged(float currentHealth)
        {
            if (currentHealth > 0)
            {
                hpBarFill.fillAmount = currentHealth / maxHP;
            }
            else
            {
               gameObject.SetActive(false);
            }
        }
        
    }
}
