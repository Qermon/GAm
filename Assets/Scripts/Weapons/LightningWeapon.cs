using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningWeapon : Weapon
{
    public GameObject projectilePrefab; // Префаб снаряда
    public float projectileLifetime = 3f; // Время жизни снаряда
    public int numberOfProjectiles = 3; // Количество молний

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchLightning()); // Запуск корутины атаки
    }

    private IEnumerator LaunchLightning()
    {
        while (true)
        {
            if (IsEnemyInRange()) // Проверка наличия врага в радиусе
            {
                LaunchProjectiles();
            }
            yield return new WaitForSeconds(1f / attackSpeed); // Задержка между атаками
        }
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        return enemies.Length > 0; // Проверяем, есть ли враги в радиусе attackRange
    }

    private void LaunchProjectiles()
    {
        HashSet<Vector2> occupiedPositions = new HashSet<Vector2>(); // Хранение занятых позиций

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        if (enemies.Length == 0) return; // Если нет врагов, выходим

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            Vector2 spawnPosition = Vector2.zero; // Инициализируем переменную
            bool validPosition = false;
            int attempts = 0;

            while (!validPosition && attempts < 100) // Пробуем найти действительную позицию
            {
                spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * attackRange; // Используем attackRange

                // Проверяем, не пересекается ли с коллайдерами и не занята ли позиция
                if (!Physics2D.OverlapCircle(spawnPosition, 0.1f, LayerMask.GetMask("Wall")) && !occupiedPositions.Contains(spawnPosition))
                {
                    occupiedPositions.Add(spawnPosition); // Добавляем позицию в занятые
                    validPosition = true; // Позиция найдена
                }
                attempts++;
            }

            if (validPosition)
            {
                GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
                projectile.tag = "Weapon"; // Устанавливаем тег
                projectile.AddComponent<LightningProjectile>().Initialize(this, projectileLifetime, damage, criticalDamage);
            }
        }
    }
}

public class LightningProjectile : MonoBehaviour
{
    private float projectileLifetime;
    private float initialDamage;
    private bool isCriticalHit;
    private Weapon weapon;

    public void Initialize(LightningWeapon weapon, float lifetime, float initialDamage, float criticalDamage)
    {
        this.projectileLifetime = lifetime;
        this.initialDamage = initialDamage;
        this.weapon = weapon;

        // Определяем, является ли удар критическим
        float randomValue = Random.value; // Генерация случайного числа от 0 до 1
        isCriticalHit = randomValue < weapon.criticalChance; // Условие для критического удара

        StartCoroutine(DestroyAfterLifetime());
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
            if (enemy != null)
            {
                DealDamage(enemy);
                // Уничтожаем снаряд после удара
            }
        }
    }

    private void DealDamage(Enemy enemy)
    {
        // Рассчитываем урон с учетом критического удара
        float damageToDeal = isCriticalHit ? initialDamage * (1 + weapon.criticalDamage / 100f) : initialDamage;

        // Наносим урон врагу
        enemy.TakeDamage((int)damageToDeal, isCriticalHit);
    }
}
