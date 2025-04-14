using UnityEngine;
using UnityEngine.Audio;

public class MeleeEnemy_0 : MonoBehaviour
{
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Attack = Animator.StringToHash("Attack");

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
    [SerializeField] private EnemyPatrol patrolScript;

    [Header("Animation Settings")]
    private HealthNew playerHealth;
    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;
    private Vector3 baseScale;
    private HealthNew _healthNew;


    [Header("Audio Settings")]
    [SerializeField] public AudioMixerGroup audioMixerGroup; 
    public AudioClip attackSound;
    [SerializeField] private float volume = 1.0f;

    private void Start()
    {
        baseScale = transform.localScale;
        anim = GetComponent<Animator>();
        _healthNew = GetComponent<HealthNew>();

        if (_healthNew != null)
        {
            _healthNew.OnDeath += HandleDeath;
        }
    }

    public void HandleDeath() 
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            anim.SetBool(Run, false);
      
            if (cooldownTimer >= attackCooldown)
            {
                patrolScript.StartWaiting();
                anim.SetTrigger(Attack);
                RotateTowardsPlayer();
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        if (rotateTowardsPlayer)
        {
            Vector3 direction = playerHealth.transform.position - transform.position;
            Vector3 localScale = baseScale;
            if (direction.x > 0)
                localScale.x = Mathf.Abs(baseScale.x);
            else
                localScale.x = -Mathf.Abs(baseScale.x);

            transform.localScale = localScale;
        }
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.up * (transform.localScale.y * colliderDistanceY) + transform.right * (transform.localScale.x * colliderDistanceX),
            new Vector3(boxCollider.bounds.size.x * rangeX, boxCollider.bounds.size.y * rangeY, boxCollider.bounds.size.z),
            0, Vector2.zero, 0, playerLayer);

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
        if (playerHealth != null && PlayerInSight())
        {
            Play(attackSound);
            playerHealth.TakeDamage(20f);
            Debug.Log("Player damaged by enemy!");
        }
        else
        {
            Debug.Log("No player found to damage.");
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
