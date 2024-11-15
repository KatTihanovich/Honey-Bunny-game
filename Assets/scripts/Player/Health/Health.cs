using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    [Header ("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private bool dead;


    [Header("Health")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberofFlashes;
    [SerializeField] private SpriteRenderer spriteRend;




    private void Awake()
    {
        currentHealth = startingHealth;
        spriteRend = GetComponent<SpriteRenderer>();
    }
    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        if (currentHealth > 0)
        {
            StartCoroutine(Invunerability());
        }
        else
        {
            if (!dead)
            {
                GetComponent<PlayerMovement>().enabled = false;
                dead = true;
            }

        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(1);
        }
    }

    public void Respawn()
    {
        AddHealth(startingHealth);
        dead = false;
        GetComponent<PlayerMovement>().enabled = true;
        StartCoroutine(Invunerability());

    }
    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }
    private IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(8,9, true);
        for (int i = 0; i < numberofFlashes; i++)
        {

            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberofFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberofFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(8,9, false);
    }
}
