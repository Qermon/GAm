using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BleedStrike : Weapon
{
    public GameObject projectilePrefab; // Префаб снаряда
    public float slowEffect = 0.15f; // Замедление врага
    public float bleedDuration = 3f; // Длительность кровотечения
    public float projectileLifetime = 3f; // Время жизни снаряда

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchBleedStrike()); // Запуск корутины атаки
    }

    private IEnumerator LaunchBleedStrike()
    {
        while (true)
        {
            if (IsEnemyInRange()) // Проверка наличия врага в радиусе
            {
                LaunchProjectile();
            }
            yield return new WaitForSeconds(1f / attackSpeed); // Задержка между атаками
        }
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        return enemies.Length > 0;
    }

    private void LaunchProjectile()
    {
        Collider2D nearestEnemy = FindNearestEnemy();
        if (nearestEnemy == null) return;

        Vector3 targetPosition = nearestEnemy.transform.position;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon"; // Устанавливаем тег
        projectile.AddComponent<BleedProjectile>().Initialize(this, targetPosition, projectileLifetime, bleedDuration, slowEffect, damage);
    }

    private Collider2D FindNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        float minDistance = float.MaxValue;
        Collider2D nearestEnemy = null;

        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
}

public class BleedProjectile : MonoBehaviour
{
    private Vector3 direction;
    private float projectileLifetime;
    private float bleedDuration;
    private float slowEffect;
    private float initialDamage;
    private List<Enemy> hitEnemies = new List<Enemy>();

    public void Initialize(BleedStrike weapon, Vector3 targetPosition, float lifetime, float bleedDuration, float slowEffect, float initialDamage)
    {
        this.projectileLifetime = lifetime;
        this.bleedDuration = bleedDuration;
        this.slowEffect = slowEffect;
        this.initialDamage = initialDamage;

        direction = (targetPosition - transform.position).normalized; // Направление на врага в момент спавна
        StartCoroutine(DestroyAfterLifetime()); // Запуск корутины для уничтожения через 3 сек.
    }

    private void Update()
    {
        // Двигаем снаряд
        transform.position += direction * Time.deltaTime * 5f;

        // Поворачиваем снаряд в направлении движения
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(projectileLifetime);
        Destroy(gameObject); // Уничтожаем снаряд через заданное время
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                ApplyEffects(enemy);
                hitEnemies.Add(enemy);
            }
        }
    }

    private void ApplyEffects(Enemy enemy)
    {
        // Мгновенный урон
        enemy.TakeDamage((int)initialDamage);

        // Замедление, если текущее замедление меньше 10%
        float currentSlowEffect = enemy.GetCurrentSlowEffect(); // Предполагаем, что у вас есть метод для получения текущего замедления
        if (currentSlowEffect < 0.1f) // Проверяем, меньше ли текущего замедления 10%
        {
            enemy.ModifySpeed(1f - slowEffect, bleedDuration); // Применяем замедление
        }
        // Эффект кровотечения
        StartCoroutine(ApplyBleedEffect(enemy));
    }

    private IEnumerator ApplyBleedEffect(Enemy enemy)
    {
        yield return new WaitForSeconds(1f);

        // Проверяем на null только после ожидания
        if (enemy == null) yield break;

        float elapsed = 0f;
        float bleedDamage = initialDamage * 0.05f; // 5% урона каждую секунду

        while (elapsed < bleedDuration)
        {
            if (enemy != null) // Проверяем на null перед вызовом метода TakeDamage
            {
                enemy.TakeDamage((int)bleedDamage);
            }

            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }

        if (enemy != null) // Проверка перед вызовом ModifySpeed
        {
            // Возвращаем врагу оригинальную скорость по завершении кровотечения
            enemy.ModifySpeed(1f / (1f - slowEffect), bleedDuration);
        }
    }

}
