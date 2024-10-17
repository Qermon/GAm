using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireStrike : Weapon
{
    public GameObject projectilePrefab; // Префаб снаряда
    public int maxTargets = 5; // Максимальное количество целей, по которым может пройти снаряд
    public float burnDuration = 3f; // Продолжительность горения
    public float projectileLifetime = 5f; // Время жизни снаряда

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchFireStrike()); // Запуск корутины атаки
    }

    private IEnumerator LaunchFireStrike()
    {
        while (true) // Бесконечный цикл для постоянного запуска снарядов
        {
            LaunchProjectile(); // Запускаем снаряд
            yield return new WaitForSeconds(1f / attackSpeed); // Ждем, исходя из скорости атаки
        }
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon"; // Устанавливаем тег
        projectile.AddComponent<FireProjectile>().Initialize(this, maxTargets, burnDuration, damage * 0.5f); // Устанавливаем урон от горения
    }
}

public class FireProjectile : MonoBehaviour
{
    private FireStrike weapon; // Ссылка на оружие
    private Enemy target; // Текущая цель
    private int targetsHit = 0; // Счетчик пораженных целей
    private int maxTargets; // Максимальное количество целей
    private float burnDuration; // Продолжительность горения
    private float burnDamagePerSecond; // Урон от горения в секунду
    private List<Enemy> hitEnemies = new List<Enemy>(); // Список пораженных врагов
    private float projectileSpeed; // Скорость снаряда
    private float projectileLifetime; // Время жизни снаряда

    public void Initialize(FireStrike weapon, int maxTargets, float burnDuration, float burnDamagePerSecond)
    {
        this.weapon = weapon;
        this.maxTargets = maxTargets;
        this.burnDuration = burnDuration; // Присваиваем значение продолжительности горения
        this.burnDamagePerSecond = burnDamagePerSecond; // Присваиваем значение урона от горения в секунду
        this.projectileSpeed = weapon.projectileSpeed;
        this.projectileLifetime = weapon.projectileLifetime;

        FindNearestTarget(); // Находим ближайшую цель

        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            StartCoroutine(MoveProjectile(direction)); // Запускаем корутину для движения снаряда
        }
        else
        {
            Destroy(gameObject); // Уничтожаем снаряд, если врагов нет
            Debug.LogWarning("No available enemies; destroying projectile.");
        }
    }

    private void FindNearestTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f, LayerMask.GetMask("Mobs", "MobsFly")); // Ищем врагов
        float closestDistance = float.MaxValue;
        foreach (Collider2D enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && !hitEnemies.Contains(enemy)) // Не выбираем уже пораженных врагов
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = enemy;
                }
            }
        }
    }

    private IEnumerator MoveProjectile(Vector3 direction)
    {
        float lifetime = projectileLifetime;

        while (lifetime > 0 && targetsHit < maxTargets) // Снаряд живет до истечения времени или пока не поражены все цели
        {
            if (target == null)
            {
                FindNearestTarget(); // Ищем нового врага, если текущий был уничтожен
                if (target == null)
                {
                    Destroy(gameObject); // Уничтожаем снаряд, если больше нет целей
                    yield break;
                }
                direction = (target.transform.position - transform.position).normalized; // Обновляем направление
            }

            transform.position += direction * projectileSpeed * Time.deltaTime; // Двигаем снаряд

            // Поворачиваем снаряд в сторону движения
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            yield return null; // Ждем до следующего кадра

            lifetime -= Time.deltaTime; // Уменьшаем время жизни снаряда

            // Если враг был уничтожен, убираем его из списка целей
            if (target != null && !target.gameObject.activeInHierarchy)
            {
                target = null;
            }
        }

        Destroy(gameObject); // Уничтожаем снаряд по истечении времени или после максимального количества попаданий
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && CanHitEnemy(enemy)) // Проверяем, можно ли ударить врага
            {
                DealDamage(enemy); // Наносим урон
                StartCoroutine(ApplyBurningEffect(enemy)); // Применяем эффект горения
                targetsHit++; // Увеличиваем счетчик пораженных целей

                if (targetsHit >= maxTargets)
                {
                    Destroy(gameObject); // Уничтожаем снаряд, если поражено максимальное количество целей
                }
            }
        }
    }

    private bool CanHitEnemy(Enemy enemy)
    {
        return !hitEnemies.Contains(enemy); // Проверяем, не был ли враг уже поражен
    }

    private void DealDamage(Enemy enemy)
    {
        float finalDamage = weapon.CalculateDamage(); // Рассчитываем урон
        Debug.Log($"Direct damage dealt to {enemy.name}: {finalDamage}");
        enemy.TakeDamage((int)finalDamage); // Наносим урон
        hitEnemies.Add(enemy); // Добавляем врага в список пораженных
    }

    private IEnumerator ApplyBurningEffect(Enemy enemy)
    {
        Debug.Log($"Enemy {enemy.name} is burning for {burnDuration} seconds with {burnDamagePerSecond} damage per second!");

        float elapsedTime = 0f;

        while (elapsedTime < burnDuration)
        {
            enemy.TakeDamage((int)burnDamagePerSecond); // Наносим урон от горения
            Debug.Log($"Applying burn damage: {burnDamagePerSecond} to {enemy.name}. Total elapsed time: {elapsedTime} seconds.");

            elapsedTime += 1f; // Ждем 1 секунду перед следующей порцией урона
            yield return new WaitForSeconds(1f);
        }

        Debug.Log($"Burn effect on {enemy.name} has ended after {burnDuration} seconds.");
    }
}
