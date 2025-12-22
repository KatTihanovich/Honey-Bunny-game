using UnityEngine;
using System.Collections;

public class NeckkerCloneState : IState
{
    private Coroutine _cloneRoutine;
    private MonoBehaviour _coroutineRunner;

    public void Enter(GameObject actor, Blackboard blackboard)
    {
        blackboard.Set(BlackboardKeys.HasCloned, true);
        blackboard.Set(BlackboardKeys.CloneFinished, false);
        
        _coroutineRunner = actor.GetComponent<EnemyStateMachineRunner>();
        if (_coroutineRunner != null)
        {
            _cloneRoutine = _coroutineRunner.StartCoroutine(CloneRoutine(actor, blackboard));
        }
    }

    public void Tick(GameObject actor, Blackboard blackboard) { }

    public void Exit(GameObject actor, Blackboard blackboard)
    {
        if (_cloneRoutine != null && _coroutineRunner != null)
        {
            _coroutineRunner.StopCoroutine(_cloneRoutine);
            _cloneRoutine = null;
        }
    }

    private IEnumerator CloneRoutine(GameObject actor, Blackboard blackboard)
    {
        var anim = blackboard.GetOrDefault<Animator>(BlackboardKeys.Animator);
        var rb = blackboard.GetOrDefault<Rigidbody2D>(BlackboardKeys.Rigidbody);
        
        // Остановка движения
        rb.linearVelocity = Vector2.zero;
        anim?.SetBool("Run", false);
        
        // Анимация клонирования (если есть специальная анимация, иначе просто пауза)
        yield return new WaitForSeconds(0.5f);
        
        // Создание клона
        GameObject mobPrefab = blackboard.GetOrDefault<GameObject>(BlackboardKeys.MobPrefab);
        float cloneOffset = blackboard.GetOrDefault<float>(BlackboardKeys.CloneOffset);
        
        if (mobPrefab != null)
        {
            // Определяем направление для клона (противоположная сторона от игрока)
            var playerTransform = blackboard.GetOrDefault<Transform>(BlackboardKeys.PlayerTransform);
            float offset = cloneOffset;
            
            if (playerTransform != null)
            {
                float direction = Mathf.Sign(actor.transform.position.x - playerTransform.position.x);
                offset *= direction;
            }
            
            Vector3 clonePosition = actor.transform.position + new Vector3(offset, 0f, 0f);
            GameObject clone = Object.Instantiate(mobPrefab, clonePosition, actor.transform.rotation);
            
            // Настройка клона
            if (clone.TryGetComponent<HealthNew>(out var cloneHealth))
            {
                var selfHealth = blackboard.GetOrDefault<HealthNew>(BlackboardKeys.SelfHealth);
                float currentHP = selfHealth.CurrentHealth;
                
                // Установить HP клона = HP оригинала
                cloneHealth.TakeDamage(cloneHealth.MaxHealth - currentHP);
            }
            
            // Пометить клона, что он не может клонироваться
            if (clone.TryGetComponent<NeckkerAI>(out var cloneAI))
            {
                cloneAI.SetAsClone();
            }
            
            Debug.Log("Neckker cloned!");
        }
        
        yield return new WaitForSeconds(0.5f);
        
        blackboard.Set(BlackboardKeys.CloneFinished, true);
    }
}
