// NeckerStateMachine.cs - Главный контроллер моба
using UnityEngine;

public class NeckerStateMachine : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public Transform player;
    
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float patrolDistance = 5f; // Дистанция патрулирования
    public float idleTime = 1f; // Время ожидания на краю
    
    [Header("Combat Settings")]
    public float detectionRange = 7f; // Зона видимости игрока
    public float attackRange = 1.5f; // Дистанция атаки
    public int maxHealth = 100;
    
    [HideInInspector] public int currentHealth;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Vector3 leftBound;
    [HideInInspector] public Vector3 rightBound;
    [HideInInspector] public bool movingRight = true;
    
    // Все состояния
    public IdleState idleState = new IdleState();
    public WalkState walkState = new WalkState();
    public AttackState attackState = new AttackState();
    public DamageState damageState = new DamageState();
    public DeathState deathState = new DeathState();
    
    private EnemyState currentState;
    
    void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
        
        // Вычисляем границы патрулирования
        leftBound = startPosition + Vector3.left * patrolDistance;
        rightBound = startPosition + Vector3.right * patrolDistance;
        
        // Находим игрока автоматически, если не назначен
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        
        // Стартуем с состояния Walk
        currentState = walkState;
        currentState.EnterState(this);
    }
    
    void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }
    
    public void SwitchState(EnemyState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }
    
    // Метод для получения урона
    public void TakeDamage(int damage)
    {
        if (currentState == deathState) return;
        
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            SwitchState(deathState);
        }
        else
        {
            SwitchState(damageState);
        }
    }
    
    // Проверка расстояния до игрока
    public float GetDistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, player.position);
    }
    
    // Разворот моба
    public void FlipTowards(Vector3 target)
    {
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
