using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : Weapon
{
    public GameObject knifePrefab; // Префаб кинжала
    public float speed = 10f; // Скорость кинжала
    public float maxDistance = 5f; // Максимальное расстояние полета

    private new void Start()
    {
        base.Start(); // Убедитесь, что вызывается базовый метод Start, если есть что-то важное в родительском классе
        StartCoroutine(ShootKnives()); // Запускаем корутину для броска кинжалов
    }

    private IEnumerator ShootKnives()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / attackSpeed); // Интервал между атаками, зависит от атакспида
            if (IsEnemyInRange()) // Проверяем, есть ли враги в радиусе атаки
            {
                ShootKnife();
            }
        }
    }

    private void ShootKnife()
    {
        GameObject targetEnemy = FindEnemyWithMostHealth(); // Находим врага с наибольшим здоровьем
        if (targetEnemy != null) // Проверяем, есть ли враги
        {
            Vector3 directionToEnemy = (targetEnemy.transform.position - transform.position).normalized; // Получаем направление к врагу
            GameObject spawnedKnife = Instantiate(knifePrefab, transform.position, Quaternion.identity);
            KnifeBehaviour knifeBehaviour = spawnedKnife.AddComponent<KnifeBehaviour>(); // Добавляем поведение кинжала
            knifeBehaviour.Initialize(directionToEnemy, speed, (int)CalculateDamage(), transform, maxDistance, this); // Устанавливаем параметры
        }
    }

    private GameObject FindEnemyWithMostHealth()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly")); // Находим всех врагов в радиусе атаки
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

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        return enemies.Length > 0; // Если есть хотя бы один враг в радиусе атаки, возвращаем true
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Рисуем радиус атаки в редакторе
    }
}

public class KnifeBehaviour : MonoBehaviour
{
    private Vector3 direction; // Направление движения кинжала
    private float speed; // Скорость кинжала
    private int baseDamage; // Базовый урон кинжала
    private Transform player; // Ссылка на игрока
    private float maxDistance; // Максимальное расстояние полета
    private float distanceTraveled; // Пройденное расстояние
    private Weapon weapon; // Ссылка на оружие

    // Словарь для отслеживания времени последней атаки по каждому врагу
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>();
    private float attackCooldown = 1f; // Время между атаками по одному и тому же врагу (1 секунда)

    private void Start()
    {
        Destroy(gameObject, 5f); // Уничтожаем кинжал через 5 секунд, если не вернулся
    }

    private void Update()
    {
        // Двигаем кинжал в заданном направлении
        transform.position += direction * speed * Time.deltaTime;
        distanceTraveled += speed * Time.deltaTime;

        // Поворачиваем кинжал в сторону движения
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        // Проверяем на столкновение с врагом
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Mobs", "MobsFly"));

        foreach (var enemy in enemies)
        {
            if (CanAttackEnemy(enemy.gameObject))
            {
                // Рассчитываем урон и проверяем, был ли он критическим
                float damageDealt = CalculateDamage(); // Рассчитываем урон
                bool isCriticalHit = damageDealt > weapon.damage; // Проверяем, был ли урон критическим
                enemy.GetComponent<Enemy>().TakeDamage((int)damageDealt, isCriticalHit); // Наносим урон
                UpdateLastAttackTime(enemy.gameObject); // Обновляем время последней атаки
            }
        }

        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }


    public void Initialize(Vector3 newDirection, float knifeSpeed, int knifeDamage, Transform playerTransform, float maxDistance, Weapon weapon)
    {
        direction = newDirection.normalized; // Нормализуем направление
        speed = knifeSpeed; // Устанавливаем скорость
        baseDamage = knifeDamage; // Устанавливаем базовый урон
        player = playerTransform; // Сохраняем ссылку на игрока
        this.maxDistance = maxDistance; // Устанавливаем максимальное расстояние
        this.weapon = weapon; // Сохраняем ссылку на оружие
    }

    private float CalculateDamage()
    {
        // Генерируем случайное значение
        float randomValue = Random.value;

        // Проверка на критический удар
        if (randomValue < weapon.criticalChance)
        {
            // Если критический удар, рассчитываем критический урон
            float critDamage = baseDamage + baseDamage * (weapon.criticalDamage / 100f);
            return critDamage; // Возвращаем критический урон
        }
        return baseDamage; // Возвращаем базовый урон
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