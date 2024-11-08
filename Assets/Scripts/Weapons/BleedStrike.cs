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

        // Устанавливаем размер снаряда
        AdjustProjectileSize(projectile);

        projectile.AddComponent<BleedProjectile>().Initialize(this, targetPosition, projectileLifetime, bleedDuration, slowEffect, damage, criticalDamage);
    }

    // Метод для изменения размера снаряда
    private void AdjustProjectileSize(GameObject projectile)
    {
        if (projectile != null)
        {
            projectile.transform.localScale = new Vector3(projectileSize, projectileSize, 1); // Устанавливаем размер
        }
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
    private bool isCriticalHit;
    private Weapon weapon;

    private AudioSource audioSource; // Источник звука для снаряда

    public void Initialize(BleedStrike weapon, Vector3 targetPosition, float lifetime, float bleedDuration, float slowEffect, float initialDamage, float criticalDamage)
    {
        this.projectileLifetime = lifetime;
        this.bleedDuration = bleedDuration;
        this.slowEffect = slowEffect;
        this.initialDamage = initialDamage;

        // Сохраняем ссылку на оружие для доступа к критическому урону
        this.weapon = weapon;

        // Определяем, является ли удар критическим, используя случайное значение
        float randomValue = Random.value; // Генерация случайного числа от 0 до 1
        isCriticalHit = randomValue < weapon.criticalChance; // Условие для критического удара

        direction = (targetPosition - transform.position).normalized;

        // Получаем компонент AudioSource и настраиваем его
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play(); // Воспроизводим звук сразу при появлении снаряда
        }

        StartCoroutine(DestroyAfterLifetime());
    }

    private void Update()
    {
        // Двигаем снаряд
        transform.position += direction * Time.deltaTime * 5f;

        // Поворачиваем снаряд в направлении движения
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Обновляем панораму звука в зависимости от направления
        if (audioSource != null)
        {
            float pan = Mathf.Clamp(direction.x, -0.4f, 0.4f); // Если влево, то -1, если вправо, то 1
            audioSource.panStereo = pan;
        }
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
        // Рассчитываем урон
        float damageToDeal = isCriticalHit ? initialDamage * (1 + weapon.criticalDamage / 100f) : initialDamage;

        // Наносим мгновенный урон и передаем флаг isCriticalHit
        enemy.TakeDamage((int)damageToDeal, isCriticalHit);

        // Применяем эффект замедления
        float currentSlowEffect = enemy.GetCurrentSlowEffect();
        if (currentSlowEffect < 0.1f)
        {
            enemy.ModifySpeed(1f - slowEffect, bleedDuration);
        }

        // Запускаем эффект кровотечения
        StartCoroutine(ApplyBleedEffect(enemy));
    }

    private IEnumerator ApplyBleedEffect(Enemy enemy)
    {
        yield return new WaitForSeconds(1f);

        if (enemy == null) yield break;

        float elapsed = 0f;
        float bleedDamage = initialDamage * 0.05f;

        while (elapsed < bleedDuration)
        {
            if (enemy != null)
            {
                // Наносим урон с учетом критического удара
                enemy.TakeDamage((int)bleedDamage, isCriticalHit);
            }

            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }

        if (enemy != null)
        {
            enemy.ModifySpeed(1f / (1f - slowEffect), bleedDuration);
        }
    }
}
