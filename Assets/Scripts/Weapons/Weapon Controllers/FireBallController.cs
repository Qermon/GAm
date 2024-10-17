using UnityEngine;

public class FireBallController : Weapon
{
    public GameObject fireBallPrefab; // Префаб огненного шара
    public float spawnCooldown = 4f; // Время между спавном снарядов
    public float projectileLifetime = 5f; // Время жизни снаряда
    private float spawnTimer; // Таймер спавна снарядов

    protected override void Start()
    {
        base.Start();
        spawnTimer = spawnCooldown; // Инициализируем таймер спавна
    }

    protected override void Update()
    {
        base.Update();

        // Обновляем таймер спавна
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f) // Если время для спавна истекло
        {
            SpawnFireBall();
            spawnTimer = spawnCooldown; // Сбрасываем таймер
        }
    }

    private void SpawnFireBall()
    {
        GameObject nearestEnemy = FindNearestEnemy(); // Ищем ближайшего врага
        if (nearestEnemy != null)
        {
            // Создаем огненный шар
            GameObject fireBall = Instantiate(fireBallPrefab, transform.position, Quaternion.identity);
            fireBall.tag = "Weapon"; // Устанавливаем тег
            FireBall fireBallScript = fireBall.AddComponent<FireBall>(); // Добавляем компонент для логики снаряда
            fireBallScript.Initialize(nearestEnemy.transform.position, projectileSpeed, projectileLifetime, this); // Передаем ссылку на текущее оружие

        }
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Находим всех врагов
        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy; // Обновляем ближайшего врага
            }
        }

        return nearestEnemy; // Возвращаем ближайшего врага
    }
}


public class FireBall : MonoBehaviour
{
    private Vector3 direction; // Направление полета снаряда
    private float speed; // Скорость
    private float lifetime; // Время жизни
    private Weapon weapon; // Ссылка на оружие

    public void Initialize(Vector3 targetPosition, float projectileSpeed, float projectileLifetime, Weapon weaponInstance)
    {
        direction = (targetPosition - transform.position).normalized; // Вычисляем направление к ближайшему врагу
        speed = projectileSpeed;
        lifetime = projectileLifetime;
        weapon = weaponInstance; // Сохраняем ссылку на родительское оружие

        // Поворачиваем снаряд в сторону цели
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        Destroy(gameObject, lifetime); // Уничтожаем снаряд через lifetime секунд
    }

    private void Update()
    {
        // Двигаем снаряд по направлению
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // Если попали во врага
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                float finalDamage = weapon.CalculateDamage(); // Рассчитываем финальный урон
                enemy.TakeDamage((int)finalDamage); // Наносим урон врагу
                Debug.Log("Урон огненного шара нанесён: " + finalDamage);
            }
        }
    }
}

