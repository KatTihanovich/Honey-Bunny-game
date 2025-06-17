using UnityEngine;
using UnityEngine.Audio;
using Game.Audio;
using System.Collections;

public class BurderEnemy : MonoBehaviour
{
    private static readonly int IdleTrigger = Animator.StringToHash("Idle");
    private static readonly int WalkTrigger = Animator.StringToHash("Walk");
    private static readonly int SwitchToRunTrigger = Animator.StringToHash("Switch_to_run");
    private static readonly int RunTrigger = Animator.StringToHash("Run");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    private static readonly int DamageTrigger = Animator.StringToHash("Damage");
    private static readonly int DeathTrigger = Animator.StringToHash("Death");
    private static readonly int SpawnThornTrigger = Animator.StringToHash("SpawnThorn");

    [Header("Patrol Settings")]
    [SerializeField] private bool alwaysChasePlayer = false;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float agroDistance = 5f;
    [SerializeField] private float lostDistance = 7f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float rangeX = 1.5f;
    [SerializeField] private float rangeY = 1.2f;
    [SerializeField] private float colliderDistanceX = 0.5f;
    [SerializeField] private float colliderDistanceY = 0f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;

    [Header("Movement Settings")]
    [SerializeField] private float walkDistance = 4f;
    [SerializeField] private float runDistance = 2f;

    [Header("Flee Settings")]
    [SerializeField] private float fleeDistance = 15f;
    [SerializeField] private float fleeSpeed = 10f;
    [SerializeField] private float fleeDelay = 0.5f;

    [Header("Thorn Spawn")]
    [SerializeField] private bool canSpawnThorns = true;
    [SerializeField] private float thornCooldown = 3f;
    [SerializeField] private GameObject _thornObject;

    private Animator anim;
    private Vector3 baseScale;
    private HealthNew selfHealth;
    private HealthNew playerHealth;
    private ISoundManager soundManager;

    private Transform playerTransform;


    private float attackTimer = Mathf.Infinity;
    private float thornTimer = 0f;
    private bool isDead = false;
    private bool isSpawningThorn = false;

    private enum State { Idle, Walk, Run, Flee }
    private State currentState = State.Idle;

    private Transform currentPatrolPoint;
    private bool isChasing = false;

    private float fleeTimer = 0f;
    private Vector3 fleeDirection;
    private bool isFleeing = false;

    private void Awake()
    {
        soundManager = SoundManagerNew.Instance;
    }


    private void Start()
    {

    
        currentPatrolPoint = pointA;
        anim = GetComponent<Animator>();
        baseScale = transform.localScale;
        selfHealth = GetComponent<HealthNew>();

        if (selfHealth != null)
        {
            selfHealth.OnDamaged += HandleDamaged;
            selfHealth.OnDeath += HandleDeath;
        }



        PlayerController player = FindFirstObjectByType<PlayerController>();

        if (player != null)
            playerTransform = player.transform; 

        SetAnimation(State.Walk); // �������� � ��������������
    }

    private void Update()
    {
        if (isDead)
        {
            Debug.Log("Enemy is dead, skipping update");
            return;
        }

        if (isFleeing)
        {
            fleeTimer += Time.deltaTime;
            transform.position += fleeDirection * fleeSpeed * Time.deltaTime;

            if (fleeTimer >= fleeDistance / fleeSpeed)
            {
                isFleeing = false;
                SetAnimation(State.Walk);
                Debug.Log("Fleeing ended, returning to patrol");
            }

            return;
        }

        attackTimer += Time.deltaTime;
        thornTimer += Time.deltaTime;

        float distanceToPlayer = playerTransform != null ? Vector2.Distance(transform.position, playerTransform.position) : Mathf.Infinity;

        if (alwaysChasePlayer)
        {
            Debug.Log(playerTransform.name);
            if (playerTransform != null && playerHealth.enabled)
            {
                isChasing = true;
                RotateTowardsPlayer();
                MoveTowards(playerHealth.transform.position);

                if (distanceToPlayer <= runDistance)
                {
                    if (currentState != State.Run)
                    {
                        SetAnimation(State.Run);
                        anim.SetTrigger(SwitchToRunTrigger);
                    }
                }
                else if (distanceToPlayer <= walkDistance)
                {
                    if (currentState != State.Walk)
                    {
                        SetAnimation(State.Walk);
                    }
                }
                else
                {
                    if (currentState != State.Idle)
                    {
                        SetAnimation(State.Idle);
                    }
                }

                if (attackTimer >= attackCooldown)
                {
                    anim.SetTrigger(AttackTrigger);
                    attackTimer = 0;
                }
            }
            else
            {
                // ���� ����� �� ������/���� - ����� ������ �� ����� ��� ���� �� ������� (�� �������)
                SetAnimation(State.Idle);
                isChasing = false;
            }
        }
        else
        {
            // ������ ��������� � ���������������
            if (PlayerInSight() && distanceToPlayer <= agroDistance)
            {
                isChasing = true;
                RotateTowardsPlayer();
                MoveTowards(playerHealth.transform.position);

                if (distanceToPlayer <= runDistance)
                {
                    if (currentState != State.Run)
                    {
                        SetAnimation(State.Run);
                        anim.SetTrigger(SwitchToRunTrigger);
                    }
                }
                else if (distanceToPlayer <= walkDistance)
                {
                    if (currentState != State.Walk)
                    {
                        SetAnimation(State.Walk);
                    }
                }
                else
                {
                    if (currentState != State.Idle)
                    {
                        SetAnimation(State.Idle);
                    }
                }

                if (attackTimer >= attackCooldown)
                {
                    anim.SetTrigger(AttackTrigger);
                    attackTimer = 0;
                }
            }
            else if (isChasing && distanceToPlayer > lostDistance)
            {
                isChasing = false;
                SetAnimation(State.Walk);
                Debug.Log("Lost player, returning to patrol");
            }
            else if (!isChasing)
            {
                if (canSpawnThorns && thornTimer >= thornCooldown && !isSpawningThorn)
                {
                    isSpawningThorn = true;
                    Debug.Log("Triggering thorn spawn during patrol");
                    anim.SetTrigger(SpawnThornTrigger);
                    thornTimer = 0;
                }
                else if (!isSpawningThorn)
                {
                    Patrol();
                }
            }
        }
    }

    private void Patrol()
    {
        if (isSpawningThorn) return;

        SetAnimation(State.Walk);
        MoveTowards(currentPatrolPoint.position);

        float distance = Mathf.Abs(transform.position.x - currentPatrolPoint.position.x);
        if (distance < 0.2f)
        {
            currentPatrolPoint = currentPatrolPoint == pointA ? pointB : pointA;
        }

        Vector3 direction = currentPatrolPoint.position - transform.position;
        Vector3 localScale = baseScale;
        localScale.x = direction.x > 0 ? Mathf.Abs(baseScale.x) : -Mathf.Abs(baseScale.x);
        transform.localScale = localScale;
    }

    private void MoveTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;

        direction = direction.normalized;
        transform.position += new Vector3(direction.x, 0f, 0f) * moveSpeed * Time.deltaTime;
    }

    private bool PlayerInSight()
    {
        Vector2 origin = boxCollider.bounds.center + transform.up * transform.localScale.y * colliderDistanceY +
                         transform.right * transform.localScale.x * colliderDistanceX;

        Vector2 size = new Vector2(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY);
        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0, Vector2.zero, 0, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            playerHealth = hit.transform.GetComponent<HealthNew>();
            return playerHealth != null && playerHealth.enabled;
        }

        playerHealth = null;
        return false;
    }

    private void RotateTowardsPlayer()
    {
        if (playerHealth == null) return;

        Vector3 direction = playerHealth.transform.position - transform.position;
        Vector3 localScale = baseScale;
        localScale.x = direction.x > 0 ? Mathf.Abs(baseScale.x) : -Mathf.Abs(baseScale.x);
        transform.localScale = localScale;
    }

    private void SetAnimation(State newState)
    {
        currentState = newState;
        ResetAllTriggers();

        switch (newState)
        {
            case State.Idle:
                anim.SetTrigger(IdleTrigger);
                break;
            case State.Walk:
                anim.SetTrigger(WalkTrigger);
                break;
            case State.Run:
                anim.SetTrigger(RunTrigger);
                break;
        }
    }

    private void ResetAllTriggers()
    {
        anim.ResetTrigger(IdleTrigger);
        anim.ResetTrigger(WalkTrigger);
        anim.ResetTrigger(SwitchToRunTrigger);
        anim.ResetTrigger(RunTrigger);
        anim.ResetTrigger(AttackTrigger);
        anim.ResetTrigger(DamageTrigger);
        anim.ResetTrigger(DeathTrigger);

    }

    public void BurderAttack()
    {
        if (playerHealth != null && PlayerInSight())
        {
            soundManager.PlaySound("WhipAttack");
            playerHealth.TakeDamage(damage);
        }
    }

    private void HandleDamaged(float damage)
    {
        if (isDead) return;

        ResetAllTriggers();
        anim.SetTrigger(DamageTrigger);
        soundManager.PlaySound("Damage");

     
        StopCoroutine(StartFleeing()); 
        StartCoroutine(StartFleeing());
    }

    private IEnumerator StartFleeing()
    {
        isFleeing = true;
        fleeTimer = 0f; 

        ResetAllTriggers();
        anim.SetTrigger(SwitchToRunTrigger);

        yield return new WaitForSeconds(fleeDelay);

        if (playerHealth != null)
        {
            fleeDirection = (transform.position - playerHealth.transform.position).normalized;
            RotateTowardsDirection(fleeDirection);
        }

        anim.SetTrigger(RunTrigger);
    }

    private void RotateTowardsDirection(Vector3 direction)
    {
        Vector3 localScale = baseScale;
        localScale.x = direction.x > 0 ? Mathf.Abs(baseScale.x) : -Mathf.Abs(baseScale.x);
        transform.localScale = localScale;
    }

    private void HandleDeath()
    {
        if (isDead) return;
        isDead = true;
        ResetAllTriggers();
        anim.SetTrigger(DeathTrigger);
        soundManager.PlaySound("Death");

        if (TryGetComponent<Collider2D>(out var col)) col.enabled = false;
        if (TryGetComponent<Rigidbody2D>(out var rb)) rb.linearVelocity = Vector2.zero;

        Destroy(gameObject.transform.parent.gameObject, 3f);
        EndWindow.IncreaseEnemyCount();
    }

    private void OnDrawGizmosSelected()
    {
        if (boxCollider == null) return;
        Gizmos.color = Color.magenta;

        Vector2 origin = boxCollider.bounds.center + transform.up * transform.localScale.y * colliderDistanceY +
                         transform.right * transform.localScale.x * colliderDistanceX;

        Vector2 size = new Vector2(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY);
        Gizmos.DrawWireCube(origin, size);
    }

    public void SpawnThorn()
    {
        Debug.Log("SpawnThorn called");
        if (_thornObject != null)
        {
            Instantiate(_thornObject, transform.position, Quaternion.identity);
        }
        isSpawningThorn = false;
        SetAnimation(State.Walk); // ������������ � ��������������
    }
}