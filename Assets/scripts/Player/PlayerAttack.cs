using UnityEngine;
using UnityEngine.UI; // Make sure to add this to access the UI Button

public class PlayerAttack : MonoBehaviour
{
    [Header("UI Elements")]
    public Button attackButton; // Link this button to the attack button in the UI.
    [SerializeField] private Animator anim;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;
    private float attackCooldown = 1f; // Time in seconds between attacks
    private float cooldownTimer = Mathf.Infinity; // To track when the next attack is available
    private PlayerMovement playerMovement;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        // Adding listener to the button click event
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(Attack);
        }
    }

    private void Update()
    {
        // Handle cooldown logic in Update
        cooldownTimer += Time.deltaTime;

        // Check if attack is ready, and handle attack via the button click
        if (cooldownTimer >= attackCooldown && attackButton != null)
        {
            attackButton.interactable = true; // Enable button when ready to attack
        }
        else
        {
            attackButton.interactable = false; // Disable button during cooldown
        }
    }
      
    private void Attack()
    {
        // Ensure cooldown is handled before attacking
        if (cooldownTimer < attackCooldown) return;

        // Start the attack animation
        anim.SetTrigger("Attack");

        // Reset the cooldown timer
        cooldownTimer = 0;

        // Find an available fireball and fire it
        int fireballIndex = FindFireball();
        if (fireballIndex >= 0)
        {
            // Set fireball's position and direction
            GameObject fireball = fireballs[fireballIndex];
            fireball.transform.position = firePoint.position;
            fireball.SetActive(true);
            fireball.GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x)); // Fire based on player facing
        }
    }

    private int FindFireball()
    {
        // Check for an inactive fireball to reuse
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
            {
                return i;
            }
        }
        return -1; // Return -1 if no fireball is available (should be handled accordingly)
    }
}
