using UnityEngine;

namespace Game.Combat
{
    public interface IAttack
    {
        float AttackDuration { get; }
        void PerformAttack(Transform attackPoint, float attackRadius, LayerMask enemyLayer, int damage, Vector3 playerPosition);
        void PerformSuperAttack(Transform attackPoint, float attackRadius, LayerMask enemyLayer, int damage, Vector3 playerPosition); 
        void Reset();
    }

    public class PlayerMeleeAttack : IAttack
    {
        public float AttackDuration { get; private set; }
        private bool _isActive;

        private int killCount = 2;
        private bool canUseSuperAttack = true;

        public PlayerMeleeAttack(float attackDuration)
        {
            AttackDuration = attackDuration;
            _isActive = false;
        }

        public void PerformAttack(Transform attackPoint, float attackRadius, LayerMask enemyLayer, int damage, Vector3 playerPosition)
        {
            if (_isActive) return;

            _isActive = true;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                HealthNew health = enemy.gameObject.GetComponent<HealthNew>();
                if (health != null && health.enabled)
                {
                    bool wasAlive = health.CurrentHealth > 0;
                    health.TakeDamage(damage);
                    Debug.Log($"Враг {enemy.name} получил {damage} урона");

                    if (wasAlive && health.CurrentHealth <= 0)
                    {
                        killCount++;
                        Debug.Log($"Убито врагов: {killCount}");

                        if (killCount >= 2 && !canUseSuperAttack)
                        {
                            canUseSuperAttack = true;
                     
                        }
                    }
                }
            }

            DebugDrawAttack(attackPoint.position, attackRadius);
        }

        public void PerformSuperAttack(Transform attackPoint, float attackRadius, LayerMask enemyLayer, int damage, Vector3 playerPosition)
        {
            if (!canUseSuperAttack)
            {
                Debug.Log("Супер удар ещё недоступен!");
                return;
            }

            Debug.Log("💥 Выполнен СУПЕР УДАР!");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius * 1.5f, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                HealthNew health = enemy.gameObject.GetComponent<HealthNew>();
                if (health != null && health.enabled)
                {
                    health.TakeDamage(damage * 2); // удвоенный урон
                    Debug.Log($"Враг {enemy.name} получил СУПЕР УРОН: {damage * 2}");
                }
            }

            killCount = 0;
            canUseSuperAttack = false;
            _isActive = true;

            DebugDrawAttack(attackPoint.position, attackRadius * 1.5f);
        }

        public void Reset()
        {
            _isActive = false;
        }

        private void DebugDrawAttack(Vector2 position, float radius)
        {
#if UNITY_EDITOR
            float duration = 0.1f;
            Vector3[] circlePoints = new Vector3[32];
            for (int i = 0; i < circlePoints.Length; i++)
            {
                float angle = i * Mathf.PI * 2f / circlePoints.Length;
                circlePoints[i] = position + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            }
            for (int i = 0; i < circlePoints.Length - 1; i++)
            {
                Debug.DrawLine(circlePoints[i], circlePoints[i + 1], Color.red, duration);
            }
            Debug.DrawLine(circlePoints[circlePoints.Length - 1], circlePoints[0], Color.red, duration);
#endif
        }
    }

    public interface IDamageable
    {
        void TakeDamage(int damage);
    }
}
