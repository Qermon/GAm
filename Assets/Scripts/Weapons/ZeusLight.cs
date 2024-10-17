using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZeusLight : Weapon
{
    public GameObject projectilePrefab; // Префаб снаряда
    public int maxBounces = 5; // Количество отскоков
    public float bounceDamageReduction = 0.1f; // Процент уменьшения урона на каждом отскоке
    public float targetRadius = 7f; // Радиус поиска целей
    public float spawnCooldown = 2f; // Кулдаун между спавном снарядов

    private bool canSpawnProjectile = true; // Флаг, можно ли спавнить новый снаряд

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchProjectileWithCooldown()); // Запускаем корутину для стрельбы с кулдауном
    }

    private IEnumerator LaunchProjectileWithCooldown()
    {
        while (true) // Бесконечный цикл для постоянного спавна снарядов
        {
            if (canSpawnProjectile)
            {
                LaunchProjectile(); // Спавним снаряд
                canSpawnProjectile = false; // Ставим флаг, что спавн временно недоступен
                yield return new WaitForSeconds(spawnCooldown); // Ждем кулдаун
                canSpawnProjectile = true; // Разрешаем спавн следующего снаряда
            }

            yield return null; // Ждем до следующего кадра
        }
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon"; // Устанавливаем тег для снаряда
        projectile.AddComponent<ZeusProjectile>().Initialize(this, maxBounces, bounceDamageReduction); // Инициализируем снаряд
    }
}

public class ZeusProjectile : MonoBehaviour
{
    private ZeusLight weapon; // Ссылка на оружие
    private Enemy target; // Текущая цель
    private int bouncesLeft; // Счетчик оставшихся отскоков
    private float currentDamage; // Текущий урон
    private float bounceDamageReduction; // Процент уменьшения урона
    private bool isMoving = false; // Флаг для отслеживания движения
    private float maxLifetime = 5f; // Максимальное время жизни снаряда
    private float lifetimeTimer; // Таймер для отслеживания времени жизни

    public void Initialize(ZeusLight weapon, int maxBounces, float bounceDamageReduction)
    {
        this.weapon = weapon;
        this.bouncesLeft = maxBounces;
        this.currentDamage = weapon.damage;
        this.bounceDamageReduction = bounceDamageReduction;
        this.lifetimeTimer = maxLifetime;

        FindRandomTarget(); // Находим первую цель
        if (target != null)
        {
            Debug.Log("Target found: " + target.name); // Отладочное сообщение
            isMoving = true;
        }
        else
        {
            Debug.LogWarning("No available enemies; destroying projectile.");
            Destroy(gameObject); // Уничтожаем снаряд, если целей нет
        }
    }

    private void FindRandomTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, weapon.targetRadius, LayerMask.GetMask("Mobs", "MobsFly")); // Находим врагов в радиусе
        if (enemies.Length > 0)
        {
            target = enemies[Random.Range(0, enemies.Length)].GetComponent<Enemy>(); // Выбираем случайного врага
            Debug.Log("Random target selected: " + target.name);
        }
        else
        {
            Debug.Log("No enemies found in range.");
            target = null; // Если врагов нет, сбрасываем цель
        }
    }

    private void FindNearestTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, weapon.targetRadius, LayerMask.GetMask("Mobs", "MobsFly"));
        float closestDistance = float.MaxValue;
        Enemy closestEnemy = null;

        foreach (Collider2D enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && enemy != target)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        target = closestEnemy;
        if (target != null)
        {
            Debug.Log("Nearest target found: " + target.name);
        }
        else
        {
            Debug.Log("No nearest target found.");
        }
    }

    private void Update()
    {
        lifetimeTimer -= Time.deltaTime; // Уменьшаем время жизни
        if (lifetimeTimer <= 0)
        {
            Debug.Log("Projectile lifetime expired; destroying.");
            Destroy(gameObject); // Уничтожаем снаряд, если время жизни закончилось
        }

        if (isMoving && target != null)
        {
            MoveTowardsTarget(); // Обновляем позицию снаряда
        }
        else if (target == null)
        {
            Debug.LogWarning("No target; destroying projectile.");
            Destroy(gameObject); // Уничтожаем снаряд, если нет цели
        }
    }

    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * weapon.projectileSpeed * Time.deltaTime;

            // Поворачиваем снаряд в сторону движения
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    private void DealDamage(Enemy enemy)
    {
        Debug.Log("Dealing damage: " + currentDamage + " to " + enemy.name);
        enemy.TakeDamage((int)currentDamage); // Наносим урон врагу
        currentDamage -= currentDamage * bounceDamageReduction; // Уменьшаем урон
        bouncesLeft--; // Уменьшаем количество отскоков
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("Projectile collided with enemy: " + enemy.name);
                DealDamage(enemy);

                if (bouncesLeft > 0)
                {
                    FindNearestTarget(); // Ищем ближайшую цель
                    if (target == null)
                    {
                        Debug.Log("No more targets; destroying projectile.");
                        Destroy(gameObject); // Уничтожаем снаряд, если целей больше нет
                    }
                }
                else
                {
                    Debug.Log("Max bounces reached; destroying projectile.");
                    Destroy(gameObject); // Уничтожаем снаряд, если отскоков больше нет
                }
            }
        }
    }

    private void OnBecameInvisible()
    {
        // Уничтожаем снаряд, если он вышел за пределы экрана
        Debug.Log("Projectile left the screen; destroying.");
        Destroy(gameObject);
    }
}
