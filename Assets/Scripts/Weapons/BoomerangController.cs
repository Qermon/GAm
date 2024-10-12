using System.Collections;
using UnityEngine;

public class BoomerangController : MonoBehaviour
{
    public GameObject boomerangPrefab; // Префаб бумеранга
    public float speed = 10f; // Скорость бумеранга
    public float attackInterval = 1.0f; // Интервал между атаками
    public int boomerangDamage = 10; // Урон бумеранга
    public float returnSpeed = 5f; // Скорость возвращения бумеранга
    public float maxDistance = 5f; // Максимальное расстояние полета

    private Vector3 lastMovedVector = Vector3.right; // Последний вектор движения (по умолчанию вправо)

    private void Start()
    {
        StartCoroutine(ShootBoomerangs());
    }

    private void Update()
    {
        // Обновляем последний вектор движения игрока
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            lastMovedVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).normalized;
        }
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
        if (lastMovedVector != Vector3.zero) // Проверяем, есть ли движение
        {
            GameObject spawnedBoomerang = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);
            BoomerangBehaviour boomerangBehaviour = spawnedBoomerang.AddComponent<BoomerangBehaviour>(); // Добавляем поведение бумеранга
            boomerangBehaviour.Initialize(lastMovedVector, speed, boomerangDamage, transform, returnSpeed, maxDistance); // Устанавливаем параметры
        }
    }
}

public class BoomerangBehaviour : MonoBehaviour
{
    private Vector3 direction;
    private float speed; // Скорость бумеранга
    private int damage; // Урон бумеранга
    private Transform player; // Ссылка на игрока
    private float returnSpeed; // Скорость возвращения
    private float maxDistance; // Максимальное расстояние полета
    private bool returning; // Флаг возвращения

    private Vector3 startPosition; // Начальная позиция бумеранга
    private float distanceTraveled; // Пройденное расстояние

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
            // Двигаем бумеранг в направлении врага
            transform.position += direction * speed * Time.deltaTime;
            distanceTraveled += speed * Time.deltaTime;

            // Проверяем на столкновение с врагом
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Enemy"));
            if (enemies.Length > 0)
            {
                // Находим ближайшего врага
                Collider2D closestEnemy = enemies[0];
                float closestDistance = Vector3.Distance(transform.position, closestEnemy.transform.position);

                foreach (var enemy in enemies)
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance < closestDistance)
                    {
                        closestEnemy = enemy;
                        closestDistance = distance;
                    }
                }

                // Наносим урон врагу
                closestEnemy.GetComponent<Enemy>().TakeDamage(damage);
                returning = true; // Начинаем возвращение
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
}
