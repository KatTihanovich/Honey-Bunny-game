using UnityEngine;
using Game.Audio;

public class ButtonTrigger : MonoBehaviour
{
    private Animator buttonAnimator;
    public MovingPlatformForButton linkedPlatform;

    private int objectsInZone = 0;
    private bool isActive = false; // внутренний флаг состояния

    private ISoundManager _soundManager;

    private void Awake()
    {
        buttonAnimator = GetComponent<Animator>();

        if (buttonAnimator == null)
        {
            Debug.LogWarning("Animator не найден на кнопке!");
        }
    }

    private void Start()
    {
        _soundManager = SoundManagerNew.Instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Pushable"))
        {
            objectsInZone++;
            UpdateState();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Pushable"))
        {
            objectsInZone = Mathf.Max(0, objectsInZone - 1);
            UpdateState();
        }
    }

    private void UpdateState()
    {
        bool shouldBeActive = objectsInZone > 0;

        if (shouldBeActive != isActive)
        {
            isActive = shouldBeActive;

            if (isActive)
            {
                buttonAnimator?.SetTrigger("Active");
                linkedPlatform?.SetDirection(true);
                _soundManager.PlaySound("ButtonOn");
            }
            else
            {
                buttonAnimator?.SetTrigger("Deactive");
                linkedPlatform?.SetDirection(false);
                _soundManager.PlaySound("ButtonOff");
            }
        }
    }
}
