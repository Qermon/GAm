using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FreezeStrike : Weapon
{
    public GameObject projectilePrefab; // Префаб снаряда
    public int maxTargets = 3; // Максимальное количество целей
    public float freezeDuration = 4f; // Продолжительность заморозки
    public float projectileLifetime = 5f; // Время жизни снаряда
    public float activationRange = 10f; // Радиус активации

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchFreezeStrike()); // Запуск корутины атаки
    }

    private IEnumerator LaunchFreezeStrike()
    {
        while (true)
        {
            if (IsEnemyInRange())
            {
                LaunchProjectile(); // Запуск снаряда
            }
            yield return new WaitForSeconds(1f / attackSpeed); // Задержка по скорости атаки
        }
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, activationRange, LayerMask.GetMask("Mobs", "MobsFly"));
        return enemies.Length > 0;
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon";
        projectile.AddComponent<FreezeProjectile>().Initialize(this, maxTargets, freezeDuration); // Устанавливаем параметры заморозки
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, activationRange); // Радиус активации
    }
}

public class FreezeProjectile : MonoBehaviour
{
    private FreezeStrike weapon;
    private Enemy target;
    private int targetsHit = 0;
    private int maxTargets;
    private float freezeDuration;
    private List<Enemy> hitEnemies = new List<Enemy>();
    private float projectileSpeed;
    private float projectileLifetime;

    public void Initialize(FreezeStrike weapon, int maxTargets, float freezeDuration)
    {
        this.weapon = weapon;
        this.maxTargets = maxTargets;
        this.freezeDuration = freezeDuration;
        this.projectileSpeed = weapon.projectileSpeed;
        this.projectileLifetime = weapon.projectileLifetime;

        FindNearestTarget(); // Поиск ближайшей цели

        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            StartCoroutine(MoveProjectile(direction));
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("No available enemies; destroying projectile.");
        }
    }

    private void FindNearestTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f, LayerMask.GetMask("Mobs", "MobsFly"));
        float closestDistance = float.MaxValue;
        foreach (Collider2D enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && !hitEnemies.Contains(enemy))
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

        while (lifetime > 0 && targetsHit < maxTargets)
        {
            if (target == null)
            {
                FindNearestTarget();
                if (target == null)
                {
                    Destroy(gameObject);
                    yield break;
                }
                direction = (target.transform.position - transform.position).normalized;
            }

            transform.position += direction * projectileSpeed * Time.deltaTime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            yield return null;

            lifetime -= Time.deltaTime;

            if (target != null && !target.gameObject.activeInHierarchy)
            {
                target = null;
            }
        }

        Destroy(gameObject);
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && CanHitEnemy(enemy))
            {
                float damage = weapon.CalculateDamage(); // Рассчитайте урон
                enemy.TakeDamage((int)damage); // Нанесите урон
                ApplyFreezeEffect(enemy); // Применение эффекта заморозки
                targetsHit++;

                if (targetsHit >= maxTargets)
                {
                    Destroy(gameObject);
                }
            }
        }
    }


    private bool CanHitEnemy(Enemy enemy)
    {
        return !hitEnemies.Contains(enemy);
    }

    private void ApplyFreezeEffect(Enemy enemy)
    {
        Debug.Log($"Enemy {enemy.name} is frozen for {freezeDuration} seconds!");
        StartCoroutine(FreezeEnemy(enemy)); // Заморозка врага на определенное время
        hitEnemies.Add(enemy);
    }

    private IEnumerator FreezeEnemy(Enemy enemy)
    {
        enemy.ModifySpeed(0.5f, freezeDuration); // Используем метод ModifySpeed для замедления

        yield return new WaitForSeconds(freezeDuration);

        Debug.Log($"Freeze effect on {enemy.name} has ended.");
    }
        
}
