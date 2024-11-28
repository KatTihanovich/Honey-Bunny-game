using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;
    private float attackCooldown;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        //if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown)        
        //    Attack();

        //    cooldownTimer += Time.deltaTime;        
    }
      
    private void Attack()
    {
        anim.SetTrigger("Attack");
        cooldownTimer = 0;

        fireballs[0].transform.position = firePoint.position;
        fireballs[0].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}
