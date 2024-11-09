using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : Weapon
{
    public GameObject fireBallPrefab; // Префаб огненного шара
    public float projectileLifetime = 5f; // Время жизни снаряда
    private new float attackTimer; // Таймер для атаки
    public float stunDuration = 0f; // Длительность стана

    protected override void Start()
    {
        base.Start();
        ResetAttackTimer();
    }

    public override void Update()
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

            // Изменяем размер снаряда на основе переменной projectileSize
            AdjustProjectileSize(fireBall);

            FireBall fireBallScript = fireBall.AddComponent<FireBall>(); // Добавляем компонент для логики снаряда
            fireBallScript.Initialize(nearestEnemy.transform.position, projectileSpeed, projectileLifetime, stunDuration, this); // Передаем параметры

            // Настраиваем звук огненного шара
            AudioSource audioSource = fireBall.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.spatialBlend = 0f; // 2D звук
                audioSource.minDistance = 1f; // Минимальное расстояние для звука
                audioSource.maxDistance = 15f; // Максимальное расстояние для звука

                // Вычисляем угол между направлением снаряда и правой стороной игрока
                Vector3 directionToEnemy = nearestEnemy.transform.position - transform.position;
                float angle = Vector3.SignedAngle(Vector3.right, directionToEnemy, Vector3.forward);

                // Нормализуем угол для панорамы от -1 до 1
                float pan;
                if (angle >= -90 && angle <= 90)
                {
                    pan = Mathf.InverseLerp(-90f, 90f, angle); // Справа от игрока: 0 до 1
                }
                else
                {
                    pan = -Mathf.InverseLerp(90f, 270f, Mathf.Abs(angle)); // Слева от игрока: 0 до -1
                }

                audioSource.panStereo = pan; // Устанавливаем панораму звука
                audioSource.Play(); // Проигрываем звук
            }
        }
    }

    // Метод для изменения размера снаряда
    private void AdjustProjectileSize(GameObject fireBall)
    {
        if (fireBall != null)
        {
            // Изменяем размер снаряда на основе переменной projectileSize
            fireBall.transform.localScale = new Vector3(projectileSize, projectileSize, 1);
        }
    }

    private GameObject FindNearestEnemy()
    {
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
                nearestEnemy = enemy.gameObject;
            }
        }

        return nearestEnemy;
    }

    public void IncreaseProjectileStunEffect(float percentage)
    {
        stunDuration += percentage;
    }
}
    

public class FireBall : MonoBehaviour
{
    private Vector3 direction; // Направление полета снаряда
    private float speed; // Скорость
    private float lifetime; // Время жизни
    private float stunDuration; // Длительность стана
    private Weapon weapon; // Ссылка на оружие
    private float initialDamage;

    // Словарь для отслеживания времени последней атаки по врагу
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>();

    public void Initialize(Vector3 targetPosition, float projectileSpeed, float projectileLifetime, float stunDuration, Weapon weaponInstance)
    {
        direction = (targetPosition - transform.position).normalized; // Вычисляем направление к врагу
        speed = projectileSpeed;
        lifetime = projectileLifetime;
        this.stunDuration = stunDuration; // Присваиваем stunDuration
        weapon = weaponInstance; // Сохраняем ссылку на оружие

        // Устанавливаем значение урона
        initialDamage = weapon.damage; // Предполагая, что CalculateDamage() возвращает базовый урон

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
                float damageToDeal = weapon.CalculateDamage(); // Получаем урон от оружия
                bool isCriticalHit = damageToDeal > weapon.damage; // Проверяем, был ли критический удар

                // Наносим урон врагу
                enemy.TakeDamage((int)damageToDeal, isCriticalHit); // Учитываем критический удар

                enemy.Stun(stunDuration);
            }
        }
    }

    // Проверка, можно ли атаковать врага (учитывая время последней атаки)
    private bool CanAttackEnemy(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            float timeSinceLastAttack = Time.time - lastAttackTimes[enemy];
            if (timeSinceLastAttack < 1f) // Можно атаковать не чаще, чем раз в секунду
            {
                return false;
            }
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

