using UnityEngine;
using UnityEngine.Audio;
using Game.Audio;

public abstract class PatrolBase : MonoBehaviour
{
    public abstract void StartWaiting();
}

public class MeleeEnemy_0 : MonoBehaviour
{
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int GotHit = Animator.StringToHash("GotHit");
    private static readonly int Dead = Animator.StringToHash("Dead");

    private bool isDead = false;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float rangeX;
    [SerializeField] private float rangeY;
    [SerializeField] private float colliderDistanceX;
    [SerializeField] private float colliderDistanceY;
    [SerializeField] private float damage=20f;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;

    [Header("Movement Settings")]
    [SerializeField] private bool rotateTowardsPlayer = true;
    [SerializeField] private PatrolBase patrolScript;

    [Header("Animation Settings")]
    private HealthNew playerHealth;
    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;
    private Vector3 baseScale;
    private HealthNew _healthNew;

    [Header("Damage on Touch")]
    private float contactDamageCooldown = 0f; 
    [SerializeField] private float contactDamageDelay = 1.5f;

    [Header("Audio Settings")]
    [SerializeField] public AudioMixerGroup audioMixerGroup; 
    public AudioClip attackSound;
    [SerializeField] private float volume = 1.0f;
    private ISoundManager _soundManager;

    private void Awake()
    {
        _soundManager = SoundManagerNew.Instance;
    }

    private void Start()
    {
        baseScale = transform.localScale;
        anim = GetComponent<Animator>();
        _healthNew = GetComponent<HealthNew>();

        if (_healthNew != null)
        {
            _healthNew.OnDamaged += HandleDamaged;
            _healthNew.OnDeath += HandleDeath;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player") && contactDamageCooldown >= contactDamageDelay)
        {
            HealthNew playerHealth = collision.gameObject.GetComponent<HealthNew>();
            if (playerHealth != null && playerHealth.enabled)
            {
                Play(attackSound);
                //playerHealth.TakeDamage(damage);
                Debug.Log("Player took delayed contact damage!");

                contactDamageCooldown = 0f; 
            }
        }
    }

    public void HandleDeath() 
    {
        if (isDead) return;

        isDead = true;
        anim.SetBool("Dead", true);
        _soundManager.PlaySound("MobDeath");

        if (TryGetComponent<Collider2D>(out var col)) col.enabled = false;
        if (TryGetComponent<Rigidbody2D>(out var rb)) rb.linearVelocity = Vector2.zero;

        Destroy(gameObject, 2f); 
    }


    private void HandleDamaged(float damage)
    {
        if (isDead) return;
        anim.SetTrigger(GotHit);
        _soundManager.PlaySound("Damage");
    }

    private void Update()
    {
        if (isDead) return;
        cooldownTimer += Time.deltaTime;
        contactDamageCooldown += Time.deltaTime;

        if (PlayerInSight())
        {
            anim.SetBool(Run, false);
      
            if (cooldownTimer >= attackCooldown)
            {
                anim.SetTrigger(Attack);
                //RotateTowardsPlayer();
            }

        }
    }

//оставить только в одном скрипте( MeleeEnemy_0 или NeckkerPatrol)
    // private void RotateTowardsPlayer()
    // {
    //     if (rotateTowardsPlayer)
    //     {
    //         Vector3 direction = playerHealth.transform.position - transform.position;
    //         Vector3 localScale = baseScale;
    //         if (direction.x > 0)
    //             localScale.x = Mathf.Abs(baseScale.x);
    //         else
    //             localScale.x = -Mathf.Abs(baseScale.x);

    //         transform.localScale = localScale;
    //     }
    // }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.up * (transform.localScale.y * colliderDistanceY) + transform.right * (transform.localScale.x * colliderDistanceX),
            new Vector3(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY, boxCollider.bounds.size.z),
            0, Vector2.zero, 0, playerLayer);
        if (hit.collider != null) 
        {
            Debug.Log(hit.collider.gameObject.name);
        }
   
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            playerHealth = hit.transform.GetComponent<HealthNew>();
            PlayerController playerController = hit.transform.GetComponent<PlayerController>();

            if (playerHealth != null && playerHealth.enabled && playerController != null && playerController.enabled)
            {
                return true;
            }
        }

        playerHealth = null;
        return false;
    }

    public void DamagePlayer()
    {
       
    }


    public void NIPPER_ATTACK()
    {
        Debug.Log("Nipper Attack");
        if (playerHealth != null && PlayerInSight())
        {
            Play(attackSound);
            playerHealth.TakeDamage(damage);
            Debug.Log("Player damaged by enemy!");
        }
        else
        {
            Debug.Log("No player found to damage.");
        }

    }

    [SerializeField] private float attackRadius = 2f; // радиус удара
    [SerializeField] private Vector2 attackOffset = Vector2.zero; // смещение от центра врага (если нужно)


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (boxCollider == null) return;

        Vector2 origin = boxCollider.bounds.center + transform.up * transform.localScale.y * colliderDistanceY + transform.right * transform.localScale.x * colliderDistanceX;
        Vector2 size = new Vector2(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY);

        Gizmos.DrawWireCube(origin, size);
    }
    public void NECKKER_ATTACK()
    {
                    Debug.Log("11111.");
        Vector2 origin = boxCollider.bounds.center + transform.up * transform.localScale.y * colliderDistanceY + transform.right * transform.localScale.x * colliderDistanceX;
        Vector2 size = new Vector2(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY);

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            size,
            0f,
            Vector2.zero,
            0f,
            playerLayer
        );
    
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            HealthNew health = hit.collider.GetComponent<HealthNew>();
            if (health != null && health.enabled)
            {
                Play(attackSound);
                health.TakeDamage(damage);
                Debug.Log("Neckker hit player with BoxCast: " + hit.collider.name);
                cooldownTimer = 0; // сбрасываем кулдаун после атаки
            }
        }
        else
        {
            Debug.Log("Neckker attack missed (BoxCast).");
        }
    }
   

    private void Play(AudioClip clip) {
            if (clip != null && audioMixerGroup != null) {
                GameObject tempAudio = new GameObject("TempAudioClip");
                AudioSource audioSource = tempAudio.AddComponent<AudioSource>();

                audioSource.outputAudioMixerGroup = audioMixerGroup;
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.Play();

                Destroy(tempAudio, clip.length);
            }
        }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(
    //        boxCollider.bounds.center + transform.up * transform.localScale.y * colliderDistanceY + transform.right * transform.localScale.x * colliderDistanceX,
    //        new Vector3(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY, boxCollider.bounds.size.z));
    //}

}
