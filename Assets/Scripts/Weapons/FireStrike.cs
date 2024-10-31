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
        projectile.AddComponent<FireProjectile>().Initialize(this, damage); // Передаем урон в снаряд
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
    private float projectileSpeed = 10f; // Скорость полета
    private List<Enemy> hitEnemies = new List<Enemy>(); // Список пораженных врагов

    public void Initialize(FireStrike weapon, float damage)
    {
        initialDamage = damage;
        FindNearestEnemyDirection();

        StartCoroutine(DestroyAfterLifetime(3f)); // Уничтожаем через 3 секунды после спавна
    }

    private void FindNearestEnemyDirection()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Mobs", "MobsFly"));
        if (enemies.Length > 0)
        {
            Transform nearestEnemy = enemies[0].transform;
            Vector3 direction = (nearestEnemy.position - transform.position).normalized;
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
        enemy.TakeDamage((int)initialDamage); // Наносим прямой урон
    }

    private IEnumerator ApplyBurningEffect(Enemy enemy)
    {
        // Начальный урон от горения — 25% от урона снаряда
        float remainingBurnDamage = initialDamage * 0.25f;
        float elapsedTime = 0f;

        // Ждём 1 секунду перед первым тиком урона от горения
        yield return new WaitForSeconds(1f);

        // Наносим урон каждые 1 секунду в течение оставшихся 4 секунд
        while (elapsedTime < burnDuration)
        {
            enemy.TakeDamage((int)remainingBurnDamage); // Наносим урон на текущем тике
            remainingBurnDamage *= 0.95f; // Уменьшаем урон на 5% от предыдущего значения
            elapsedTime += 1f; // Переход к следующему тику
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator DestroyAfterLifetime(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
