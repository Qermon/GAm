using System.Collections;
using UnityEngine;

public class BoomerangWeapon : MonoBehaviour
{
    public GameObject boomerangPrefab; // Префаб бумеранга
    public float attackInterval = 1.0f; // Интервал между атаками
    public int boomerangDamage = 10; // Урон бумеранга
    public float returnSpeed = 5f; // Скорость возвращения бумеранга
    public float maxDistance = 5f; // Максимальное расстояние полета бумеранга

    private void Start()
    {
        StartCoroutine(ThrowBoomerang()); // Запускаем корутину для атаки
    }

    private IEnumerator ThrowBoomerang()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval); // Ждем перед следующим броском
            SpawnBoomerang();
        }
    }

    private void SpawnBoomerang()
    {
        GameObject spawnedBoomerang = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);
        BoomerangBehaviour boomerangBehaviour = spawnedBoomerang.AddComponent<BoomerangBehaviour>(); // Добавляем поведение бумеранга
        boomerangBehaviour.Initialize(boomerangDamage, transform.position, maxDistance, returnSpeed); // Устанавливаем параметры
    }
}

public class BoomerangBehaviour : MonoBehaviour
{
    private int damage; // Урон бумеранга
    private Vector3 startPosition; // Начальная позиция бумеранга
    private float maxDistance; // Максимальное расстояние полета
    private float returnSpeed; // Скорость возвращения бумеранга
    private bool returning; // Флаг возвращения бумеранга

    private void Update()
    {
        if (returning)
        {
            // Возвращаемся к игроку
            transform.position = Vector3.MoveTowards(transform.position, startPosition, returnSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPosition) < 0.1f)
            {
                Destroy(gameObject); // Уничтожаем бумеранг при достижении игрока
            }
        }
        else
        {
            // Находим ближайшего врага и атакуем
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, maxDistance, LayerMask.GetMask("Enemy"));
            if (enemies.Length > 0)
            {
                Collider2D closestEnemy = enemies[0];
                float closestDistance = Vector3.Distance(transform.position, closestEnemy.transform.position);

                // Ищем ближайшего врага
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
        }
    }

    public void Initialize(int boomerangDamage, Vector3 playerPosition, float maxDistance, float returnSpeed)
    {
        damage = boomerangDamage;
        startPosition = playerPosition;
        this.maxDistance = maxDistance;
        this.returnSpeed = returnSpeed; // Устанавливаем скорость возвращения
    }
}
