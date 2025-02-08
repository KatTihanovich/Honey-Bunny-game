using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class BossEnemyScript : MonoBehaviour
    {
        public Animator animator; // Ссылка на аниматор
        public string hideTrigger = "Hide"; // Имя триггера анимации скрытия
        public string appearTrigger = "Appear"; // Имя триггера анимации появления
        public float delay = 5f; // Задержка в секундах перед запуском анимации
        public bool isAlive = true; // Флаг состояния
        public Transform targetLocation; // Целевая точка телепортации
        private Vector3 initialScale;

        private void Start()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            initialScale = transform.localScale;
            StartCoroutine(TriggerAnimationLoop());
        }

        private void Update()
        {
            if (targetLocation != null)
            {
                float direction = Mathf.Sign(targetLocation.position.x - transform.position.x);
                transform.localScale = new Vector3(initialScale.x * -direction, initialScale.y, initialScale.z);
            }
        }

        private IEnumerator TriggerAnimationLoop()
        {
            while (isAlive)
            {
                animator.SetTrigger(appearTrigger);
                yield return new WaitForSeconds(delay);
                if (isAlive)
                {
                    animator.SetTrigger(hideTrigger);
                    yield return new WaitForSeconds(1f); // Небольшая задержка перед перемещением
                    if (targetLocation != null)
                    {
                        transform.position = new Vector3(targetLocation.position.x, transform.position.y,
                            transform.position.z);
                    }
                }
            }
        }
    }
}