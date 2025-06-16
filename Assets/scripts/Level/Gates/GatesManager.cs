using UnityEngine;

public class GateController : MonoBehaviour
{
    public Animator gateAnimator;
    public int requiredStars = 3;
    public Collider2D gateCollider;

    private bool isOpen = false;

    private void Start()
    {
        if (gateCollider != null)
        {
            gateCollider.isTrigger = false; // Ворота изначально закрыты
        }

        Debug.Log("Всего монет у игрока: " + TotalCoinTracker.GetTotalCoins());
    }

    public void TryOpenGate()
    {
        if (isOpen) return;

        int playerStars = TotalCoinTracker.GetTotalCoins(); // Получаем общее количество монет

        Debug.Log($"Проверка ворот: у игрока {playerStars}, нужно {requiredStars}");

        if (playerStars >= requiredStars)
        {
            Debug.Log("Ворота открываются!");
            gateAnimator.SetTrigger("Open");
            isOpen = true;

            if (gateCollider != null)
            {
                gateCollider.isTrigger = true; // Ворота становятся проходимыми
            }
        }
        else
        {
            Debug.Log("Недостаточно монет! Ворота остаются закрытыми.");
        }
    }
}
