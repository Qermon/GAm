using UnityEngine;

public class WitchsProjectile : MonoBehaviour
{
    public float speed = 10f; // Скорость снаряда
    public float lifetime = 5f; // Время жизни снаряда
    private Rigidbody2D rb; // Ссылка на Rigidbody2D
    private Vector2 direction; // Направление движения снаряда
    public float damage = 10f;
    public float baseDamage = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime); // Уничтожаем снаряд через заданное время

        // Проверяем, установлено ли направление
        if (direction != Vector2.zero)
        {
            rb.velocity = direction * speed; // Устанавливаем скорость снаряда

            // Поворачиваем снаряд в сторону движения
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Вычисляем угол поворота
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Поворачиваем снаряд
        }
        else
        {
            Debug.LogError("Направление снаряда не установлено!");
        }
    }

    public void UpdateStats(float projectile)
    {
        damage *= projectile;
    }

    public void RefreshStats()
    {
        damage = baseDamage;
    }

    // Метод для установки направления снаряда
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized; // Устанавливаем новое направление
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Снаряд столкнулся с: " + collision.gameObject.name); // Проверяем, с чем сталкивается снаряд

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Снаряд попал в игрока!");

            // Проверяем, что это не CircleCollider2D (радиус сбора)
            if (collision is CircleCollider2D)
            {
                // Игнорируем коллайдер радиуса сбора
                return;
            }

            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage); // Наносим урон
                Debug.Log("Снаряд нанес урон игроку! Текущее здоровье: " + playerHealth.currentHealth);
            }
            else
            {
                Debug.LogError("PlayerHealth компонент не найден на: " + collision.gameObject.name);
            }
            Destroy(gameObject); // Уничтожаем снаряд
        }
        else if (collision.CompareTag("Wall"))
        {
            Debug.Log("Снаряд столкнулся со стеной и был уничтожен!");
            Destroy(gameObject);
        }
    }
}
