using UnityEngine;

public class BossFightGlobal : MonoBehaviour
{
    private static readonly int RescuedTrigger = Animator.StringToHash("Rescued");
        
    private GameObject receiver;
    
    private GameObject cage;
    private Animator animator;
    
    public Health bossHealth;
    
    void Start()
    {
        receiver = GameObject.Find("Bunny");
        if (receiver == null)
        {
            Debug.LogError("[Cage] Bunny not found!!!");
        }

        cage = GameObject.FindWithTag("Honey");
        if (cage == null)
        {
            Debug.LogError("[Cage] Honey not found!!!");
        }
        animator = cage.GetComponent<Animator>();
        
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged += HandleHealthChanged;
        }
    }
    
    private void HandleHealthChanged(float currentHealth)
    {
        if (currentHealth <= 0)
        {
            Debug.Log("СВОБОДА");
            animator.SetTrigger(RescuedTrigger);
            
            //TODO: Здесь вызывать сообщение о завершении уровня!
        }
    }
}
