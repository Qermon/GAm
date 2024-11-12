using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 7f; // Скорость полета стрелы
    public float damage = 5f; // Урон, который наносит стрела
    public float baseDamage = 5f;
    private Transform target; // Цель (игрок)

    void Start()
    {
        // Находим игрока по тегу "Player"
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // Проверяем, что цель найдена
        if (target == null)
        {
            Debug.LogError("Игрок с тегом Player не найден на сцене.");
        }
    }

    void Update()
    {
        // Если цель существует, направляем стрелу в сторону игрока
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            // Поворачиваем стрелу в сторону игрока
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    public void UpdateStats(float projectile)
    {
        damage += baseDamage / 100 * projectile;
    }

    public void RefreshStats()
    {
        damage = baseDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем столкновение с объектом, имеющим тег "Wall" или "Weapon"
        if (collision.CompareTag("Wall") || collision.CompareTag("Weapon"))
        {
            Destroy(gameObject); // Уничтожаем стрелу
            return;
        }

        // Проверяем столкновение с игроком
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            // Проверяем, что это не CircleCollider2D (радиус сбора)
            if (collision is CircleCollider2D)
            {
                // Игнорируем коллайдер радиуса сбора
                return;
            }

            // Наносим урон игроку только если это не CircleCollider2D
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);
            }

            Destroy(gameObject); // Уничтожаем стрелу после удара
        }
    }

}
