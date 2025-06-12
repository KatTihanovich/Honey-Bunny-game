using UnityEngine;

namespace Enemy
{
    public class BossAttackArea : MonoBehaviour
    {
        public GameObject receiver;

        private bool playerInside;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInside = true;
                Debug.Log("Игрок вошел в триггер!");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Игрок вышел из триггера!");
                playerInside = false;
            }
        }


        private void FixedUpdate()
        {
            if (playerInside)
            {
                receiver?.SendMessage("OnPlayerEntered", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}