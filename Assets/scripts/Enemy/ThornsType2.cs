using System.Collections;
using UnityEngine;

public class ThornsType2 : MonoBehaviour
{
    [Header("Idle Pulse Settings")]
    public float idlePulseSpeed = 2f;
    public float idlePulseAmount = 0.02f;

    [Header("Hit Pulse Settings")]
    public float hitPulseScale = 1.04f;
    public float hitPulseDuration = 0.2f;

    [Header("Damage Settings")]
    public int damage = 5;
    public float damageInterval = 1f; // Интервал между ударами

    private Vector3 originalScale;
    private Coroutine idlePulseCoroutine;
    private Coroutine hitPulseCoroutine;

    private GameObject playerInTrigger = null;
    private float nextDamageTime = 0f;

    private void Start()
    {
        originalScale = transform.localScale;
        idlePulseCoroutine = StartCoroutine(IdlePulse());
    }

    private IEnumerator IdlePulse()
    {
        while (true)
        {
            float timer = 0f;
            while (timer < Mathf.PI * 2)
            {
                float scaleOffset = Mathf.Sin(timer) * idlePulseAmount;
                transform.localScale = originalScale + Vector3.one * scaleOffset;
                timer += Time.deltaTime * idlePulseSpeed;
                yield return null;
            }
        }
    }

    private IEnumerator HitPulse()
    {
        if (idlePulseCoroutine != null)
            StopCoroutine(idlePulseCoroutine);

        transform.localScale = originalScale * hitPulseScale;

        yield return new WaitForSeconds(hitPulseDuration);

        transform.localScale = originalScale;

        idlePulseCoroutine = StartCoroutine(IdlePulse());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject == playerInTrigger)
                playerInTrigger = null;
        }
    }

    private void Update()
    {
        if (playerInTrigger != null && Time.time >= nextDamageTime)
        {
            HealthNew player = playerInTrigger.GetComponent<HealthNew>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("Игрок получил урон от ловушки");

                nextDamageTime = Time.time + damageInterval;

                if (hitPulseCoroutine != null)
                    StopCoroutine(hitPulseCoroutine);

                hitPulseCoroutine = StartCoroutine(HitPulse());
            }
        }
    }
}
