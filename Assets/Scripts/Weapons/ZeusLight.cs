using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZeusLight : Weapon
{
    public GameObject projectilePrefab;
    public int maxBounces = 5;
    public float bounceDamageReduction = 0.1f;

    public float splitChance = 0f; // шанс на разделение снаряда

    public override void Update()
    {
        base.Update(); // Если нужно вызвать родительский метод
       
    
    // Обновляем панораму звука каждый кадр, пока снаряд в движении
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            Vector3 directionToProjectile = transform.position - Camera.main.transform.position;
            float angle = Vector3.SignedAngle(Vector3.right, directionToProjectile, Vector3.forward);
            float pan = Mathf.InverseLerp(-90f, 90f, angle);
            audioSource.panStereo = pan;
        }

        // Ваше обычное обновление снаряда
    }


    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchProjectileCoroutine());
    }

    private IEnumerator LaunchProjectileCoroutine()
    {
        while (true)
        {
            LaunchProjectile();
            yield return new WaitForSeconds(1f / attackSpeed);
        }
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon";
        projectile.AddComponent<ZeusProjectile>().Initialize(this, maxBounces, bounceDamageReduction, splitChance, damage);

        // Получаем AudioSource из префаба снаряда
        AudioSource audioSource = projectile.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.spatialBlend = 1f; // 3D звук
            audioSource.minDistance = 1f; // Минимальное расстояние для звука
            audioSource.maxDistance = 15f; // Максимальное расстояние для звука

            // Вычисляем направление от игрока к снаряду
            Vector3 directionToProjectile = projectile.transform.position - transform.position;

            // Находим угол между направлением снаряда и направлением взгляда игрока
            float angle = Vector3.SignedAngle(Vector3.right, directionToProjectile, Vector3.forward);

            // Нормализуем угол для панорамы от -1 до 1
            float pan = Mathf.Clamp(Mathf.InverseLerp(-90f, 90f, angle), -1f, 1f); // Нормализация угла для панорамы от -1 до 1

            // Устанавливаем панораму звука
            audioSource.panStereo = pan;

            // Настройка громкости
            audioSource.volume = 1f;

            // Проигрываем звук
            audioSource.Play();
        }
    }

    public void zeusLightCountBounceBuff()
    {
        maxBounces += 2;
    }    

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly", "Boss"));
        return enemies.Length > 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void IncreaseProjectileSplitEffect(float percentage)
    {
        splitChance += percentage;

        if (splitChance > 0.5f)
        {
            splitChance = 0.5f;
        }
    }
}


public class ZeusProjectile : MonoBehaviour
{
    private ZeusLight weapon; // Ссылка на оружие
    private Enemy target; // Текущая цель
    private int bouncesLeft; // Счетчик оставшихся отскоков
    private float currentDamage; // Текущий урон
    private float bounceDamageReduction; // Процент уменьшения урона
    private bool isMoving = false; // Флаг для отслеживания движения
    private float maxLifetime = 10f; // Максимальное время жизни снаряда
    private float lifetimeTimer; // Таймер для отслеживания времени жизни
    private float splitChance; // Шанс разделения снаряда

    public void Initialize(ZeusLight weapon, int maxBounces, float bounceDamageReduction, float splitChance, float initialDamage)
    {
        this.weapon = weapon;
        this.bouncesLeft = maxBounces; // Оставшиеся отскоки
        this.currentDamage = initialDamage;  // Урон снаряда
        this.bounceDamageReduction = bounceDamageReduction;
        this.lifetimeTimer = maxLifetime;
        this.splitChance = splitChance;


        FindRandomTarget(); // Находим первую цель
        if (target != null)
        {
            Debug.Log("Target found: " + target.name); // Отладочное сообщение
            isMoving = true;
        }
        else
        {
            Destroy(gameObject); // Уничтожаем снаряд, если целей нет
        }
    }

    private void FindRandomTarget()
    {
        // Находим врагов в радиусе 1.5
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Mobs", "MobsFly", "Boss"));
        if (enemies.Length > 0)
        {
            target = enemies[Random.Range(0, enemies.Length)].GetComponent<Enemy>(); // Выбираем случайного врага
            Debug.Log("Random target selected: " + target.name);
        }
        else
        {
            Debug.Log("No enemies found in range.");
            target = null; // Если врагов нет, сбрасываем цель
        }
    }

    private void FindNearestTarget()
    {
        // Находим врагов в радиусе 1.5
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Mobs", "MobsFly", "Boss"));
        float closestDistance = float.MaxValue;
        Enemy closestEnemy = null;

        foreach (Collider2D enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && enemy != target)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        target = closestEnemy;
        if (target != null)
        {
            Debug.Log("Nearest target found: " + target.name);
        }
        else
        {
            Debug.Log("No nearest target found.");
        }
    }

    private void Update()
    {
        lifetimeTimer -= Time.deltaTime; // Уменьшаем время жизни
        if (lifetimeTimer <= 0)
        {
            Debug.Log("Projectile lifetime expired; destroying.");
            Destroy(gameObject); // Уничтожаем снаряд, если время жизни закончилось
        }

        if (isMoving && target != null)
        {
            MoveTowardsTarget(); // Обновляем позицию снаряда
        }
        else if (target == null)
        {
            Destroy(gameObject); // Уничтожаем снаряд, если нет цели
        }
    }

    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * weapon.projectileSpeed * Time.deltaTime;

            // Поворачиваем снаряд в сторону движения
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    private void DealDamage(Enemy enemy)
    {
        // Проверяем вероятность критического удара
        bool isCriticalHit = Random.value < weapon.criticalChance;
        float damageToDeal = isCriticalHit ? currentDamage * (1 + weapon.criticalDamage / 100f) : currentDamage;

        // Наносим урон врагу
        enemy.TakeDamage((int)damageToDeal, isCriticalHit);
        currentDamage -= currentDamage * bounceDamageReduction; // Уменьшаем урон
        bouncesLeft--; // Уменьшаем количество отскоков
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("Projectile collided with enemy: " + enemy.name);
                DealDamage(enemy);

                // Проверка на разделение снаряда
                if (Random.value < splitChance)
                {
                    Debug.Log("Projectile split triggered!");

                    // Запускаем логику для создания двух новых снарядов
                    LaunchSplitProjectiles(enemy);
                }

                if (bouncesLeft > 0)
                {
                    FindNearestTarget(); // Ищем ближайшую цель
                    if (target == null)
                    {
                        Debug.Log("No more targets; destroying projectile.");
                        Destroy(gameObject); // Уничтожаем снаряд, если целей больше нет
                    }
                }
                else
                {
                    Debug.Log("Max bounces reached; destroying projectile.");
                    Destroy(gameObject); // Уничтожаем снаряд, если отскоков больше нет
                }
            }
        }
    }
    private void LaunchSplitProjectiles(Enemy initialTarget)
    {
       
        
            // Новый урон для разделенных снарядов — 75% от текущего
            float splitDamage = currentDamage * 0.75f; // Урон будет на 25% меньше

            // Первый новый снаряд
            GameObject splitProjectile1 = Instantiate(weapon.projectilePrefab, transform.position, Quaternion.identity);
            ZeusProjectile splitProjectile1Script = splitProjectile1.AddComponent<ZeusProjectile>();
            splitProjectile1Script.Initialize(weapon, bouncesLeft, bounceDamageReduction, splitChance, splitDamage);

            // Второй новый снаряд
            GameObject splitProjectile2 = Instantiate(weapon.projectilePrefab, transform.position, Quaternion.identity);
            ZeusProjectile splitProjectile2Script = splitProjectile2.AddComponent<ZeusProjectile>();
            splitProjectile2Script.Initialize(weapon, bouncesLeft, bounceDamageReduction, splitChance, splitDamage);

            // Устанавливаем цели:
            splitProjectile1Script.target = initialTarget; // Первый новый снаряд сохраняет свою цель
            splitProjectile2Script.target = FindFarthestTarget(initialTarget);
        
    }

    private Enemy FindFarthestTarget(Enemy excludedEnemy)
    {
        // Находим врагов в радиусе 2
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 2f, LayerMask.GetMask("Mobs", "MobsFly", "Boss"));
        float farthestDistance = 0f;
        Enemy farthestEnemy = null;

        foreach (Collider2D enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && enemy != excludedEnemy)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance > farthestDistance)
                {
                    farthestDistance = distance;
                    farthestEnemy = enemy;
                }
            }
        }

        return farthestEnemy;
    }

    private void OnBecameInvisible()
    {
        // Уничтожаем снаряд, если он вышел за пределы экрана
        Debug.Log("Projectile left the screen; destroying.");
        Destroy(gameObject);
    }
}
