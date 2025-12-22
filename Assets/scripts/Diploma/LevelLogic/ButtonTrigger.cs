using UnityEngine;
using Game.Audio;

public class ButtonTrigger : MonoBehaviour
{
    private Animator buttonAnimator;
    public MovingPlatformForButton linkedPlatform;
    
    [Header("Detection Settings")]
    public string targetTag = "RopePlatform"; 
    
    private bool isOpen = false;
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
        Debug.Log($"Button is waiting for tag: '{targetTag}'");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ОТЛАДКА: Показываем все объекты, входящие в триггер
        Debug.Log($"Trigger entered by: '{other.gameObject.name}' with tag: '{other.tag}'");
        
        if (!isOpen && other.CompareTag(targetTag))
        {
            ActivateButton();
        }
        else if (!isOpen)
        {
            Debug.Log($"Tag mismatch: expected '{targetTag}', got '{other.tag}'");
        }
    }

    private void ActivateButton()
    {
        isOpen = true; 
        
        buttonAnimator?.SetTrigger("Active");
        linkedPlatform?.SetDirection(true);
        _soundManager?.PlaySound("ButtonOn");
        
        Debug.Log("Button permanently activated by rope platform!");
    }
}
