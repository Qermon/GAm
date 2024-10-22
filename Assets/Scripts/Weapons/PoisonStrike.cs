using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoisonStrike : Weapon
{
    public GameObject projectilePrefab; // Префаб снаряда
    public int maxTargets = 5; // Максимальное количество целей, по которым может пройти снаряд
    public float poisonDuration = 5f; // Продолжительность отравления
    public float projectileLifetime = 5f; // Время жизни снаряда
    public float activationRange = 10f; // Радиус, в котором снаряды начинают спавниться

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchPoisonStrike()); // Запуск корутины атаки
    }

    private IEnumerator LaunchPoisonStrike()
    {
        while (true) // Бесконечный цикл для постоянного запуска снарядов
        {
            if (IsEnemyInRange()) // Проверяем, есть ли враг в радиусе активации
            {
                LaunchProjectile(); // Запускаем снаряд
            }
            yield return new WaitForSeconds(1f / attackSpeed); // Ждем, исходя из скорости атаки
        }
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, activationRange, LayerMask.GetMask("Mobs", "MobsFly"));
        return enemies.Length > 0; // Если есть хотя бы один враг в радиусе активации, возвращаем true
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon"; // Устанавливаем тег
        projectile.AddComponent<PoisonProjectile>().Initialize(this, maxTargets, poisonDuration, damage * 0.5f); // Устанавливаем урон от яда
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationRange); // Рисуем радиус активации
    }
}

public class PoisonProjectile : MonoBehaviour
{
    private PoisonStrike weapon; // Ссылка на оружие
    private Enemy target; // Текущая цель
    private int targetsHit = 0; // Счетчик пораженных целей
    private int maxTargets; // Максимальное количество целей
    private float poisonDuration; // Продолжительность отравления
    private float poisonDamagePerSecond; // Урон от яда в секунду
    private List<Enemy> hitEnemies = new List<Enemy>(); // Список пораженных врагов
    private float projectileSpeed; // Скорость снаряда
    private float projectileLifetime; // Время жизни снаряда

    public void Initialize(PoisonStrike weapon, int maxTargets, float poisonDuration, float poisonDamagePerSecond)
    {
        this.weapon = weapon;
        this.maxTargets = maxTargets;
        this.poisonDuration = poisonDuration; // Присваиваем значение продолжительности отравления
        this.poisonDamagePerSecond = poisonDamagePerSecond; // Присваиваем значение урона от яда в секунду
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
                    // Если нет цели в радиусе 1.5, уничтожаем снаряд
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

    private void FindNearestTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Mobs", "MobsFly")); // Ищем врагов в радиусе 1.5
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


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && CanHitEnemy(enemy)) // Проверяем, можно ли ударить врага
            {
                DealDamage(enemy); // Наносим мгновенный урон
                StartCoroutine(ApplyPoisonEffect(enemy)); // Применяем эффект яда
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
        float finalDamage = weapon.CalculateDamage(); // Рассчитываем мгновенный урон
        Debug.Log($"Direct damage dealt to {enemy.name}: {finalDamage}");
        enemy.TakeDamage((int)finalDamage); // Наносим мгновенный урон
        hitEnemies.Add(enemy); // Добавляем врага в список пораженных
    }

    private IEnumerator ApplyPoisonEffect(Enemy enemy)
    {
        Debug.Log($"Enemy {enemy.name} is poisoned for {poisonDuration} seconds with {poisonDamagePerSecond} damage per second!");

        float elapsedTime = 0f;

        while (elapsedTime < poisonDuration)
        {
            enemy.TakeDamage((int)poisonDamagePerSecond); // Наносим урон от яда
            Debug.Log($"Applying poison damage: {poisonDamagePerSecond} to {enemy.name}. Total elapsed time: {elapsedTime} seconds.");

            elapsedTime += 1f; // Ждем 1 секунду перед следующей порцией урона
            yield return new WaitForSeconds(1f);
        }

        Debug.Log($"Poison effect on {enemy.name} has ended after {poisonDuration} seconds.");
    }
}
