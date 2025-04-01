using UnityEngine;

// Определяет класс для движущейся платформы, которая может нести игрока
public class MovingPlatform : MonoBehaviour
{
    // Скорость, с которой платформа движется вверх и вниз (доступно для редактирования в инспекторе)
    [SerializeField] private float _moveSpeed = 2f;

    // Общее расстояние вертикального движения платформы (доступно для редактирования в инспекторе)
    [SerializeField] private float _moveDistance = 3f;

    // Ссылка на компонент Rigidbody2D платформы
    private Rigidbody2D rb;

    // Начальная позиция платформы
    private Vector2 startPos;

    // Предыдущая позиция платформы (используется для расчета скорости)
    private Vector2 prevPos;

    // Вызывается при инициализации объекта, до Start()
    private void Awake()
    {
        // Получаем компонент Rigidbody2D, прикрепленный к этому объекту
        rb = GetComponent<Rigidbody2D>();

        // Устанавливаем тип Kinematic, чтобы вручную контролировать движение
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Включаем полные кинематические контакты для правильного обнаружения столкновений
        rb.useFullKinematicContacts = true;

        // Сохраняем начальную позицию и устанавливаем ее как предыдущую
        startPos = prevPos = transform.position;
    }

    // Вызывается каждый фиксированный шаг времени (обновление физики)
    private void FixedUpdate()
    {
        // Сохраняем текущую позицию перед перемещением
        prevPos = rb.position;

        // Вычисляем вертикальное смещение с помощью PingPong (колеблется между 0 и _moveDistance)
        // Вычитаем половину _moveDistance для центрирования движения вокруг startPos
        float yOffset = Mathf.PingPong(Time.time * _moveSpeed, _moveDistance) - _moveDistance * 0.5f;

        // Создаем новую позицию, используя начальную X и вычисленное смещение Y
        Vector2 newPosition = new Vector2(startPos.x, startPos.y + yOffset);

        // Перемещаем платформу в новую позицию
        rb.MovePosition(newPosition);
    }

    // Вызывается каждый фиксированный кадр при продолжающемся столкновении с другим объектом
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Пытаемся получить Rigidbody2D у объекта столкновения (скорее всего, игрока)
        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

        // Проверяем, найден ли Rigidbody2D и является ли он динамическим (подвержен физике)
        if (playerRb != null && playerRb.bodyType == RigidbodyType2D.Dynamic)
        {
            // Вычисляем скорость платформы на основе изменения позиции
            Vector2 platformVelocity = (rb.position - prevPos) / Time.fixedDeltaTime;

            // Получаем нормаль точки контакта (направление столкновения)
            Vector2 contactNormal = collision.contacts[0].normal;

            // Проверяем, идет ли столкновение сверху (нормаль направлена вниз)
            if (contactNormal.y < -0.5f)
            {
                // Получаем текущую скорость игрока
                Vector2 playerVelocity = playerRb.linearVelocity;

                // Если платформа движется вверх и игрок не падает быстрее платформы
                if (platformVelocity.y > 0 && playerVelocity.y >= 0 && playerVelocity.y <= platformVelocity.y)
                {
                    // Синхронизируем вертикальную скорость игрока со скоростью платформы вверх
                    playerVelocity.y = platformVelocity.y;
                }
                // Если платформа движется вниз, заставляем игрока двигаться вниз вместе с ней
                else if (platformVelocity.y < 0)
                {
                    playerVelocity.y = platformVelocity.y;
                }

                // Добавляем горизонтальную скорость платформы к скорости игрока (если есть)
                playerVelocity.x += platformVelocity.x;

                // Применяем измененную скорость к игроку
                playerRb.linearVelocity = playerVelocity;
            }
        }
    }

    // Отрисовывает визуальную отладочную информацию в окне сцены
    private void OnDrawGizmos()
    {
        // Устанавливаем цвет Gizmo в зеленый
        Gizmos.color = Color.green;

        // Используем startPos во время игры, transform.position в редакторе
        Vector2 pos = Application.isPlaying ? startPos : (Vector2)transform.position;

        // Рисуем линию, показывающую полный диапазон движения
        Gizmos.DrawLine(pos + Vector2.up * _moveDistance * 0.5f, pos - Vector2.up * _moveDistance * 0.5f);
    }
}