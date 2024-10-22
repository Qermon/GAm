using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangController : Weapon
{
    public GameObject boomerangPrefab; // Префаб бумеранга
    public float attackInterval = 1.0f; // Интервал между атаками
    public float speed = 10f; // Скорость бумеранга
    public float returnSpeed = 5f; // Скорость возвращения бумеранга
    public float maxDistance = 5f; // Максимальное расстояние полета
    public float activationRange = 10f; // Радиус активации для поиска врагов

    private new void Start()
    {
        StartCoroutine(ShootBoomerangs()); // Запускаем корутину для броска бумерангов
    }

    private IEnumerator ShootBoomerangs()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval); // Ждем перед следующим броском
            ShootBoomerang();
        }
    }

    private void ShootBoomerang()
    {
        GameObject closestEnemy = FindClosestEnemy(); // Находим ближайшего врага
        if (closestEnemy != null) // Проверяем, есть ли враги
        {
            Vector3 directionToEnemy = (closestEnemy.transform.position - transform.position).normalized; // Получаем направление к врагу
            GameObject spawnedBoomerang = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);
            BoomerangBehaviour boomerangBehaviour = spawnedBoomerang.AddComponent<BoomerangBehaviour>(); // Добавляем поведение бумеранга
            boomerangBehaviour.Initialize(directionToEnemy, speed, (int)CalculateDamage(), transform, returnSpeed, maxDistance); // Устанавливаем параметры
        }
    }

    private GameObject FindClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, activationRange, LayerMask.GetMask("Mobs", "MobsFly")); // Находим всех врагов в радиусе activationRange
        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.gameObject; // Запоминаем ближайшего врага
            }
        }
        return closestEnemy; // Возвращаем ближайшего врага
    }
}

public class BoomerangBehaviour : MonoBehaviour
{
    private Vector3 direction; // Направление движения бумеранга
    private float speed; // Скорость бумеранга
    private int damage; // Урон бумеранга
    private Transform player; // Ссылка на игрока
    private float returnSpeed; // Скорость возвращения
    private float maxDistance; // Максимальное расстояние полета
    private bool returning; // Флаг возвращения

    private Vector3 startPosition; // Начальная позиция бумеранга
    private float distanceTraveled; // Пройденное расстояние

    // Словарь для отслеживания времени последней атаки по каждому врагу
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>();
    private float attackCooldown = 0.3f; // Время между атаками по одному и тому же врагу (1 секунда)

    private void Start()
    {
        startPosition = transform.position; // Устанавливаем начальную позицию
        Destroy(gameObject, 5f); // Уничтожаем бумеранг через 5 секунд, если не вернулся
    }

    private void Update()
    {
        if (returning)
        {
            // Возвращаемся к игроку
            if (player != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, returnSpeed * Time.deltaTime);
            }
            // Проверяем, достигли ли игрока
            if (Vector3.Distance(transform.position, player.position) < 0.1f)
            {
                Destroy(gameObject); // Уничтожаем бумеранг при достижении игрока
            }
        }
        else
        {
            // Двигаем бумеранг в заданном направлении
            transform.position += direction * speed * Time.deltaTime;
            distanceTraveled += speed * Time.deltaTime;

            // Проверяем на столкновение с врагом
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Mobs", "MobsFly")); // Находим всех врагов в радиусе 0.5f

            foreach (var enemy in enemies)
            {
                if (CanAttackEnemy(enemy.gameObject)) // Проверяем, можно ли атаковать врага
                {
                    // Наносим урон врагу
                    enemy.GetComponent<Enemy>().TakeDamage(damage);
                    UpdateLastAttackTime(enemy.gameObject); // Обновляем время последней атаки
                }
            }

            // Проверяем, не превысило ли расстояние
            if (distanceTraveled >= maxDistance)
            {
                returning = true; // Начинаем возвращение, если достигли максимального расстояния
            }
        }
    }

    public void Initialize(Vector3 newDirection, float boomerangSpeed, int boomerangDamage, Transform playerTransform, float returnSpeed, float maxDistance)
    {
        direction = newDirection.normalized; // Нормализуем направление
        speed = boomerangSpeed; // Устанавливаем скорость
        damage = boomerangDamage; // Устанавливаем урон
        player = playerTransform; // Сохраняем ссылку на игрока
        this.returnSpeed = returnSpeed; // Устанавливаем скорость возвращения
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
