using System.Collections.Generic;
using UnityEngine;

public class FireBallController : Weapon
{
    public GameObject fireBallPrefab; // Префаб огненного шара
    public float projectileLifetime = 5f; // Время жизни снаряда
    private new float attackTimer; // Таймер для атаки

    protected override void Start()
    {
        base.Start();
        ResetAttackTimer();


    }

    protected override void Update()
    {
        base.Update();

        // Обновляем таймер атаки
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f) // Если время для атаки истекло
        {
            SpawnFireBall();
            ResetAttackTimer();
        }
    }

    private void ResetAttackTimer()
    {
        attackTimer = 1f / attackSpeed; // Сбрасываем таймер с учетом скорости атаки
    }

    private void SpawnFireBall()
    {
        GameObject nearestEnemy = FindNearestEnemy(); // Ищем ближайшего врага
        if (nearestEnemy != null)
        {
            // Создаем огненный шар
            GameObject fireBall = Instantiate(fireBallPrefab, transform.position, Quaternion.identity);
            fireBall.tag = "Weapon"; // Устанавливаем тег
            FireBall fireBallScript = fireBall.AddComponent<FireBall>(); // Добавляем компонент для логики снаряда
            fireBallScript.Initialize(nearestEnemy.transform.position, projectileSpeed, projectileLifetime, this); // Передаем параметры
        }
    }

    private GameObject FindNearestEnemy()
    {
        // Находим всех врагов в радиусе attackRange на слоях Mobs и MobsFly
        int enemyLayerMask = LayerMask.GetMask("Mobs", "MobsFly");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayerMask);

        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy.gameObject; // Обновляем ближайшего врага
            }
        }

        return nearestEnemy; // Возвращаем ближайшего врага
    }
}

public class FireBall : MonoBehaviour
{
    private Vector3 direction; // Направление полета снаряда
    private float speed; // Скорость
    private float lifetime; // Время жизни
    private Weapon weapon; // Ссылка на оружие

    // Словарь для отслеживания времени последней атаки по врагу
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>();

    public void Initialize(Vector3 targetPosition, float projectileSpeed, float projectileLifetime, Weapon weaponInstance)
    {
        direction = (targetPosition - transform.position).normalized; // Вычисляем направление к врагу
        speed = projectileSpeed;
        lifetime = projectileLifetime;
        weapon = weaponInstance; // Сохраняем ссылку на оружие

        // Поворачиваем снаряд в сторону цели
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        Destroy(gameObject, lifetime); // Уничтожаем снаряд через lifetime секунд
    }

    private void Update()
    {
        // Двигаем снаряд по направлению
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // Если попали во врага
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && CanAttackEnemy(enemy.gameObject)) // Проверяем, можем ли атаковать врага
            {
                float finalDamage = weapon.CalculateDamage(); // Рассчитываем урон
                enemy.TakeDamage((int)finalDamage); // Наносим урон
                Debug.Log("Урон огненного шара нанесён: " + finalDamage);
                UpdateLastAttackTime(enemy.gameObject); // Обновляем время последней атаки
            }
        }
    }

    // Проверка, можно ли атаковать врага (учитывая время последней атаки)
    private bool CanAttackEnemy(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            float timeSinceLastAttack = Time.time - lastAttackTimes[enemy];
        }
        return true; // Если враг ещё не атакован, можем атаковать
    }

    // Обновление времени последней атаки
    private void UpdateLastAttackTime(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            lastAttackTimes[enemy] = Time.time; // Обновляем время последней атаки
        }
        else
        {
            lastAttackTimes.Add(enemy, Time.time); // Добавляем врага в словарь
        }
    }
}
