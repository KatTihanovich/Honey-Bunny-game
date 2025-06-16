using UnityEngine;

public class GateController : MonoBehaviour
{
    public Animator gateAnimator;
    public int requiredStars = 3;
    public Collider2D gateCollider; // Коллайдер ворот

    private bool isOpen = false;

    private void Start()
    {
        if (gateCollider != null)
        {
            gateCollider.isTrigger = false; // Ворота закрыты, нельзя проходить
        }
    }

    public void TryOpenGate(int playerStars)
    {
        Debug.Log($"Проверка ворот: у игрока {playerStars}, нужно {requiredStars}");

        if (isOpen) return; // Если уже открыты — ничего не делаем

        if (playerStars >= requiredStars)
        {
            Debug.Log("Ворота открываются!");
            gateAnimator.SetTrigger("Open");
            isOpen = true;
            
            if (gateCollider != null)
            {
                gateCollider.isTrigger = true; // Делаем ворота проходимыми
            }
        }
        else
        {
            Debug.Log("Недостаточно звезд! Ворота остаются закрытыми.");
        }
    }
}