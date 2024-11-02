using UnityEngine;

public class SkeletonBoss : Enemy
{
    public float attackRange = 1.0f; // Дальность атаки
    public float moveSpeed = 1.0f; // Скорость движения
    public int goldAmount = 50; // Количество золота, выпадающее с босса
    private Transform player; // Ссылка на игрока
    private float attackCooldown = 1.0f; // Время между атаками
    private float skeletonAttackTimer; // Таймер атаки (переименован)
    private Animator animator; // Ссылка на Animator

    // Новые переменные для второй атаки
    public GameObject lightningPrefab; // Префаб молнии
    public float lightningRadius = 1.0f; // Радиус спавна молний
    public float lightningDuration = 3.0f; // Длительность атаки молний
    private bool isLightningAttacking = false; // Указывает, находится ли босс в атаке молний
    private float[] healthThresholds = { 0.75f, 0.50f, 0.25f, 0.10f }; // Порог здоровья для молний

    // Новые переменные для спавна мобов
    public GameObject mobPrefab; // Префаб моба
    public int mobsToSpawn = 10; // Количество мобов для призыва
    private bool isInvulnerable = false; // Указывает, может ли босс получать урон
    private bool hasSpawnedMobs = false; // Флаг для отслеживания вызванных мобов
    private bool isHitAnimating = false; // Указывает, находится ли босс в анимации

    protected override void Start()
    {
        base.Start();
        skeletonAttackTimer = 0f; // Инициализация таймера атаки
        animator = GetComponent<Animator>(); // Получаем компонент Animator

        // Находим игрока в сцене
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found!");
        }
    }

    protected override void Update()
    {
        if (isDead || player == null) return;

        skeletonAttackTimer -= Time.deltaTime; // Обновляем таймер атаки

        // Проверяем расстояние до игрока
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (isInvulnerable)
        {
            // В случае, если босс неуязвим, проверяем завершение анимации
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("hit_2"))
            {
                isInvulnerable = false; // Убираем неуязвимость
            }
            return; // Не продолжаем выполнение, пока босс неуязвим
        }

        if (isHitAnimating) // Проверяем состояние анимации
        {
            return; // Не выполняем действия, пока анимация не завершена
        }

        if (distanceToPlayer <= attackRange && skeletonAttackTimer <= 0f)
        {
            AttackPlayer(); // Атака игрока
        }
        else if (distanceToPlayer > attackRange)
        {
            MoveTowardsPlayer(); // Движение к игроку
        }
        else
        {
            animator.SetBool("run", false); // Останавливаем анимацию бега
        }

        // Проверяем здоровье босса для активации атаки молний и вызова мобов
        CheckHealthForLightningAttack();
    }

    protected override void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            FlipSprite(direction); // Поворачиваем спрайт в сторону движения

            animator.SetBool("run", true); // Устанавливаем анимацию бега
        }
    }

    protected override void AttackPlayer()
    {
        skeletonAttackTimer = attackCooldown; // Сбрасываем таймер атаки
        animator.SetBool("run", false); // Останавливаем анимацию бега
        animator.SetTrigger("skill_1"); // Запускаем первую атаку

        // Делаем задержку перед нанесением урона
        Invoke("DealDamage", 0.5f); // Убедитесь, что время задержки совпадает с продолжительностью анимации атаки

        Debug.Log("Скелет наносит урон игроку!");
    }

    private void CheckHealthForLightningAttack()
    {
        float healthPercentage = (float)currentHealth / maxHealth; // Процент текущего здоровья

        foreach (float threshold in healthThresholds)
        {
            if (healthPercentage <= threshold && !isLightningAttacking)
            {
                if (healthPercentage <= 0.50f && !hasSpawnedMobs)
                {
                    // Если здоровье ниже 50%, начинаем цикл анимации hit_2 и спавн мобов
                    StartCoroutine(StartInvulnerabilityAndSpawnMobs());
                    hasSpawnedMobs = true; // Устанавливаем флаг, чтобы не вызывать мобов снова
                }
                else if (healthPercentage > 0.50f)
                {
                    StartLightningAttack(); // Атака молний
                }
                break; // Останавливаем проверку после первой активации
            }
        }
    }

    private System.Collections.IEnumerator StartInvulnerabilityAndSpawnMobs()
    {
        isInvulnerable = true; // Босс становится неуязвимым
        isHitAnimating = true; // Устанавливаем флаг анимации
        animator.SetTrigger("hit_2"); // Запускаем анимацию hit_2

        for (int i = 0; i < mobsToSpawn; i++)
        {
            // Спавн моба в случайной позиции вокруг босса
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * 1.0f; // Призыв на небольшом расстоянии от босса
            Instantiate(mobPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.5f); // Задержка между спавном мобов
        }

        // Ждем окончания анимации hit_2
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isInvulnerable = false; // Босс больше не неуязвим
        isHitAnimating = false; // Сбрасываем флаг анимации
    }

    private void StartLightningAttack()
    {
        isLightningAttacking = true;
        animator.SetTrigger("skill_2"); // Запускаем анимацию молний

        // Запускаем корутину для спавна молний
        StartCoroutine(SpawnLightning());
    }

    private System.Collections.IEnumerator SpawnLightning()
    {
        float endTime = Time.time + lightningDuration;

        while (Time.time < endTime)
        {
            // Спавн молнии в случайной позиции вокруг босса
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * lightningRadius;
            GameObject lightningInstance = Instantiate(lightningPrefab, spawnPosition, Quaternion.identity);

            // Уничтожаем экземпляр молнии через задержку
            Destroy(lightningInstance, 1.0f); // Настройте время здесь, как долго вы хотите, чтобы она оставалась

            yield return new WaitForSeconds(0.5f); // Задержка между спавнами молний
        }

        isLightningAttacking = false; // Сбрасываем флаг после завершения атаки
    }

    private void DealDamage()
    {
        if (isInvulnerable) return; // Не наносим урон, если босс неуязвим

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage((int)damage); // Наносим урон игроку
        }
    }

    public override void TakeDamage(int damage)
    {
        if (isInvulnerable) return; // Не наносим урон, если босс неуязвим

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    protected override void Die()
    {
        isDead = true;
        Debug.Log("Скелет погибает!");
        // Логика для смерти, например, выпадение золота
        PlayerGold playerGold = player.GetComponent<PlayerGold>();
        if (playerGold != null)
        {
            playerGold.AddGold(goldAmount);
        }
        Destroy(gameObject);
    }

    protected void FlipSprite(Vector2 direction)
    {
        Vector3 localScale = transform.localScale;
        if (direction.x > 0)
        {
            localScale.x = Mathf.Abs(localScale.x);
        }
        else if (direction.x < 0)
        {
            localScale.x = -Mathf.Abs(localScale.x);
        }
        transform.localScale = localScale;
    }
}
