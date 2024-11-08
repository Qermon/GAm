using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireStrike : Weapon
{
    public GameObject projectilePrefab; // Префаб снаряда
    public float projectileLifetime = 3f; // Время жизни снаряда

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchFireStrike()); // Запуск корутины атаки
    }

    private IEnumerator LaunchFireStrike()
    {
        while (true)
        {
            if (IsEnemyInRange())
            {
                LaunchProjectile();
            }
            yield return new WaitForSeconds(1f / attackSpeed); // Ждем, исходя из скорости атаки
        }
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        return enemies.Length > 0;
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon";

        // Устанавливаем размер снаряда
        AdjustProjectileSize(projectile);

        projectile.AddComponent<FireProjectile>().Initialize(this, damage); // Передаем урон в снаряд
    }

    // Метод для изменения размера снаряда
    private void AdjustProjectileSize(GameObject projectile)
    {
        if (projectile != null)
        {
            projectile.transform.localScale = new Vector3(projectileSize, projectileSize, 1); // Устанавливаем размер
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

public class FireProjectile : MonoBehaviour
{
    private float initialDamage;
    private float burnDuration = 5f; // Длительность горения
    private float burnTickInterval = 1f; // Интервал тиков горения
    private float burnDamageFactor = 0.25f; // Начальный урон от горения (25% от урона)
    private float burnDecayFactor = 0.05f; // Уменьшение урона на 5% каждый тик
    private float projectileSpeed = 3.5f; // Скорость полета
    private List<Enemy> hitEnemies = new List<Enemy>(); // Список пораженных врагов
    private bool isCriticalHit;
    private Weapon weapon;

    private AudioSource audioSource; // Источник звука для снаряда
    private Vector3 direction; // Направление движения снаряда

    public void Initialize(FireStrike weapon, float damage)
    {
        this.weapon = weapon; // Сохраняем ссылку на оружие
        initialDamage = damage;

        // Получаем компонент AudioSource и настраиваем его
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play(); // Воспроизводим звук при запуске снаряда
        }

        FindNearestEnemyDirection();
        StartCoroutine(DestroyAfterLifetime(3f)); // Уничтожаем снаряд через 3 секунды
    }

    private void FindNearestEnemyDirection()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Mobs", "MobsFly"));
        if (enemies.Length > 0)
        {
            Transform nearestEnemy = enemies[0].transform;
            direction = (nearestEnemy.position - transform.position).normalized;
            StartCoroutine(MoveProjectile(direction));
            RotateTowardsDirection(direction);
        }
        else
        {
            Destroy(gameObject); // Уничтожаем снаряд, если врагов нет
        }
    }

    private IEnumerator MoveProjectile(Vector3 direction)
    {
        while (true)
        {
            transform.position += direction * projectileSpeed * Time.deltaTime;

            // Обновляем панораму звука в зависимости от направления
            if (audioSource != null)
            {
                float pan = Mathf.Clamp(direction.x, -0.4f, 0.4f); // Если влево, то -1, если вправо, то 1
                audioSource.panStereo = pan;
            }

            yield return null;
        }
    }

    private void RotateTowardsDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                ApplyDirectDamage(enemy);
                StartCoroutine(ApplyBurningEffect(enemy));
                hitEnemies.Add(enemy); // Добавляем в список пораженных врагов
            }
        }
    }

    private void ApplyDirectDamage(Enemy enemy)
    {
        // Проверяем вероятность критического удара
        bool isCriticalHit = Random.value < weapon.criticalChance; // Предполагая, что criticalChance хранится в процентах
        float damageToDeal = isCriticalHit ? initialDamage * (1 + weapon.criticalDamage / 100f) : initialDamage; // Учитываем критический урон

        enemy.TakeDamage((int)damageToDeal, isCriticalHit); // Наносим урон с учетом критического удара
    }

    private IEnumerator ApplyBurningEffect(Enemy enemy)
    {
        float remainingBurnDamage = initialDamage * burnDamageFactor;
        float elapsedTime = 0f;

        yield return new WaitForSeconds(burnTickInterval);

        while (elapsedTime < burnDuration)
        {
            if (enemy != null) // Проверяем на null перед нанесением урона
            {
                enemy.TakeDamage((int)remainingBurnDamage, isCriticalHit); // Наносим урон от горения с учетом критического удара
                remainingBurnDamage *= (1 - burnDecayFactor);
            }
            elapsedTime += burnTickInterval;
            yield return new WaitForSeconds(burnTickInterval);
        }
    }

    private IEnumerator DestroyAfterLifetime(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
