using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform player; // Ссылка на игрока
    public float parallaxEffectMultiplier = 0.5f; // Коэффициент параллакса

    private float startPositionX;
    private float lastPlayerX;

    void Start()
    {
        startPositionX = transform.position.x;
        if (player != null)
        {
            lastPlayerX = player.position.x;
        }
    }

    void Update()
    {
        if (player != null)
        {
            float deltaX = player.position.x - lastPlayerX;
            transform.position += new Vector3(deltaX * parallaxEffectMultiplier, 0, 0);
            lastPlayerX = player.position.x;
        }
    }
}