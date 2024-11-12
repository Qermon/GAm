using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangController : Weapon
{
    public GameObject boomerangPrefab; // Префаб бумеранга
    public float speed = 10f; // Скорость бумеранга
    public float returnSpeed = 5f; // Скорость возвращения бумеранга
    public float maxDistance = 5f; // Максимальное расстояние полета
    public float doubleDamageChance = 1f; // Шанс на двойной урон (от 0 до 1)

    private new void Start()
    {
        base.Start();
        StartCoroutine(ShootBoomerangs()); // Запускаем корутину для броска бумерангов
    }

    private IEnumerator ShootBoomerangs()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / attackSpeed); // Ждем перед следующим броском, используя скорость атаки
            ShootBoomerang();
        }
    }

    private void ShootBoomerang()
    {
        GameObject closestEnemy = FindClosestEnemy(); // Находим ближайшего врага
        if (closestEnemy != null) // Проверяем, есть ли враги
        {
            Vector3 directionToEnemy = (closestEnemy.transform.position - transform.position).normalized;

            // Рассчитываем урон и критический удар
            float finalDamage = CalculateDamage();
            bool isCriticalHit = finalDamage > damage;

            // Проверка на шанс двойного урона
            bool isDoubleDamage = Random.value <= doubleDamageChance;

            // Если сработал шанс двойного урона, удваиваем урон
            if (isDoubleDamage)
            {
                finalDamage *= 2;
            }

            GameObject spawnedBoomerang = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);
            spawnedBoomerang.name = "Boomerang"; // Назначаем имя объекту

            // Изменяем размер бумеранга
            AdjustProjectileSize(spawnedBoomerang);

            BoomerangBehaviour boomerangBehaviour = spawnedBoomerang.AddComponent<BoomerangBehaviour>(); // Добавляем поведение бумеранга
            boomerangBehaviour.Initialize(directionToEnemy, speed, (int)finalDamage, isCriticalHit, isDoubleDamage, transform, returnSpeed, maxDistance); // Передаем значения
        }
    }

    // Метод для изменения размера бумеранга
    private void AdjustProjectileSize(GameObject boomerang)
    {
        if (boomerang != null)
        {
            // Изменяем размер бумеранга на основе переменной projectileSize
            boomerang.transform.localScale = new Vector3(projectileSize, projectileSize, 1);
        }
    }

    private GameObject FindClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly", "Boss")); // Находим всех врагов в радиусе attackRange
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
    public void IncreaseProjectileDoubleDomageEffect(float percentage)
    {
        doubleDamageChance += percentage;
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
    private bool isCriticalHit; // Флаг критического удара
    private bool isDoubleDamage; // Флаг двойного урона

    private Vector3 startPosition; // Начальная позиция бумеранга
    private float distanceTraveled; // Пройденное расстояние

    private AudioSource audioSource; // Источник звука для бумеранга

    // Словарь для отслеживания времени последней атаки по каждому врагу
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>();
    private float attackCooldown = 0.3f; // Время между атаками по одному и тому же врагу (0.3 секунды)

    private void Start()
    {
        startPosition = transform.position; // Устанавливаем начальную позицию
        Destroy(gameObject, 5f); // Уничтожаем бумеранг через 5 секунд, если не вернулся

        // Получаем AudioSource на бумеранге
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            // Настройка панорамы звука в зависимости от направления
            float pan = Mathf.Clamp(direction.x, -0.4f, 0.4f); // -1 для левого, 1 для правого
            audioSource.panStereo = pan; // Устанавливаем панораму в AudioSource

            // Воспроизведение звука
            audioSource.Play();
        }
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
                return;
            }
        }
        else
        {
            // Двигаем бумеранг в заданном направлении
            transform.position += direction * speed * Time.deltaTime;
            distanceTraveled += speed * Time.deltaTime;

            // Проверяем, не превысило ли расстояние
            if (distanceTraveled >= maxDistance)
            {
                returning = true; // Начинаем возвращение, если достигли максимального расстояния
            }
        }

        // Проверяем на столкновение с врагом в радиусе бумеранга при движении в любом направлении
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 0.25f, LayerMask.GetMask("Mobs", "MobsFly", "Boss"));
        foreach (var enemy in enemies)
        {
            if (CanAttackEnemy(enemy.gameObject)) // Проверяем, можно ли атаковать врага
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage, isCriticalHit, isDoubleDamage); // Передаем флаг двойного урона


                UpdateLastAttackTime(enemy.gameObject); // Обновляем время последней атаки
            }
        }
    }

    public void Initialize(Vector3 newDirection, float boomerangSpeed, int boomerangDamage, bool criticalHit, bool doubleDamage, Transform playerTransform, float returnSpeed, float maxDistance)
    {
        direction = newDirection.normalized;
        speed = boomerangSpeed;
        damage = boomerangDamage;
        isCriticalHit = criticalHit; // Устанавливаем флаг критического удара
        isDoubleDamage = doubleDamage; // Устанавливаем флаг двойного урона
        player = playerTransform;
        this.returnSpeed = returnSpeed;
        this.maxDistance = maxDistance;

        // Настройка панорамы для звука
        if (audioSource != null)
        {
            // Применяем панораму на основе направления движения
            float pan = Mathf.Clamp(direction.x, -1f, 1f); // Если влево, то -1, если вправо, то 1
            audioSource.panStereo = pan;
        }
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
