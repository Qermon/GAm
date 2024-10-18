using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : Weapon
{
    public GameObject knifePrefab; // Префаб кенжала
    public float attackInterval = 1.0f; // Интервал между атаками
    public float speed = 10f; // Скорость кенжала
    public float maxDistance = 5f; // Максимальное расстояние полета

    private new void Start()
    {
        StartCoroutine(ShootKnives()); // Запускаем корутину для броска кенжалов
    }

    private IEnumerator ShootKnives()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval); // Ждем перед следующим броском
            ShootKnife();
        }
    }

    private void ShootKnife()
    {
        GameObject targetEnemy = FindEnemyWithMostHealth(); // Находим врага с наибольшим здоровьем
        if (targetEnemy != null) // Проверяем, есть ли враги
        {
            Vector3 directionToEnemy = (targetEnemy.transform.position - transform.position).normalized; // Получаем направление к врагу
            GameObject spawnedKnife = Instantiate(knifePrefab, transform.position, Quaternion.identity);
            KnifeBehaviour knifeBehaviour = spawnedKnife.AddComponent<KnifeBehaviour>(); // Добавляем поведение кенжала
            knifeBehaviour.Initialize(directionToEnemy, speed, (int)CalculateDamage(), transform, maxDistance); // Устанавливаем параметры
        }
    }

    private GameObject FindEnemyWithMostHealth()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f, LayerMask.GetMask("Mobs", "MobsFly")); // Находим всех врагов в радиусе 10f
        GameObject strongestEnemy = null;
        float highestHealth = -1;

        foreach (Collider2D enemy in enemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null && enemyScript.currentHealth > highestHealth) // Используем currentHealth
            {
                highestHealth = enemyScript.currentHealth; // Запоминаем здоровье врага
                strongestEnemy = enemy.gameObject; // Запоминаем врага с наибольшим здоровьем
            }
        }
        return strongestEnemy; // Возвращаем врага с наибольшим здоровьем
    }
}

public class KnifeBehaviour : MonoBehaviour
{
    private Vector3 direction; // Направление движения кенжала
    private float speed; // Скорость кенжала
    private int damage; // Урон кенжала
    private Transform player; // Ссылка на игрока
    private float maxDistance; // Максимальное расстояние полета
    private float distanceTraveled; // Пройденное расстояние

    // Словарь для отслеживания времени последней атаки по каждому врагу
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>();
    private float attackCooldown = 1f; // Время между атаками по одному и тому же врагу (1 секунда)

    private void Start()
    {
        Destroy(gameObject, 5f); // Уничтожаем кенжал через 5 секунд, если не вернулся
    }

    private void Update()
    {
        // Двигаем кенжал в заданном направлении
        transform.position += direction * speed * Time.deltaTime;
        distanceTraveled += speed * Time.deltaTime;

        // Поворачиваем кенжал в сторону движения
        if (direction != Vector3.zero) // Проверяем, не нулевая ли вектор
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Получаем угол
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Поворачиваем кенжал
        }

        // Проверяем на столкновение с врагом
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Mobs", "MobsFly")); // Находим всех врагов в радиусе 0.5f

        foreach (var enemy in enemies)
        {
            // Проверяем, можем ли мы атаковать этого врага (прошла ли 1 секунда с последней атаки)
            if (CanAttackEnemy(enemy.gameObject))
            {
                // Наносим урон врагу
                enemy.GetComponent<Enemy>().TakeDamage(damage);
                UpdateLastAttackTime(enemy.gameObject); // Обновляем время последней атаки по этому врагу
            }
        }

        // Проверяем, не превысило ли расстояние
        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject); // Уничтожаем кенжал, если достигли максимального расстояния
        }
    }

    public void Initialize(Vector3 newDirection, float knifeSpeed, int knifeDamage, Transform playerTransform, float maxDistance)
    {
        direction = newDirection.normalized; // Нормализуем направление
        speed = knifeSpeed; // Устанавливаем скорость
        damage = knifeDamage; // Устанавливаем урон
        player = playerTransform; // Сохраняем ссылку на игрока
        this.maxDistance = maxDistance; // Устанавливаем максимальное расстояние
    }

    // Метод для проверки, можем ли мы атаковать врага (на основе времени последней атаки)
    private bool CanAttackEnemy(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            float timeSinceLastAttack = Time.time - lastAttackTimes[enemy];
            return timeSinceLastAttack >= attackCooldown; // Проверяем, прошло ли больше attackCooldown секунд
        }
        return true; // Если атаки по этому врагу еще не было, можем атаковать
    }

    // Метод для обновления времени последней атаки
    private void UpdateLastAttackTime(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            lastAttackTimes[enemy] = Time.time; // Обновляем время последней атаки
        }
        else
        {
            lastAttackTimes.Add(enemy, Time.time); // Добавляем запись о времени атаки, если врага еще нет в словаре
        }
    }
}
