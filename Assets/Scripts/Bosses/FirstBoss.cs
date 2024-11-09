using System.Collections;
using UnityEngine;

public class FirstBoss : Enemy
{
    public float bossAttackRange = 1.0f; // Уменьшенная дальность атаки
    public float bossMoveSpeed = 0.5f; // Уменьшенная скорость движения
    public int bossGoldAmount = 50; // Количество золота, выпадающее с босса
    public Animator animator; // Ссылка на аниматор
    public GameObject lightningPrefab; // Префаб молнии
    public float lightningSpawnRadius = 3.0f; // Радиус спавна молний
    public float invulnerabilityDuration = 10.0f; // Длительность неуязвимости

    private Rigidbody2D rb; // Добавляем Rigidbody2D для физики
    private bool isRegenerating = false; // Флаг для регенерации
    private bool isInvulnerable = false; // Флаг для неуязвимости
    private Weapon weapon;

    protected override void Start()
    {
        weapon = GetComponent<Weapon>();
        base.Start();
        attackRange = bossAttackRange;
        enemyMoveSpeed = bossMoveSpeed;
        goldAmount = bossGoldAmount;

        // Инициализация Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    protected override void Update()
    {
        if (isDead || player == null) return;

        attackTimer -= Time.deltaTime;

        // Проверяем, нужно ли начать регенерацию
        if (!isRegenerating && currentHealth <= maxHealth * 0.5f)
        {
            Debug.Log("Босс начинает регенерацию."); // Отладочное сообщение
            StartCoroutine(Regeneration());
        }

        // Если босс неуязвим, он не должен двигаться
        if (isInvulnerable) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && attackTimer <= 0f)
        {
            // Запускаем ближнюю атаку
            animator.SetTrigger("meleeAttack");
            DealDamageToPlayer();
            attackTimer = attackCooldown; // Сбрасываем таймер для ближней атаки
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    private IEnumerator Regeneration()
    {
        isRegenerating = true;
        isInvulnerable = true; // Устанавливаем неуязвимость

        animator.SetTrigger("regeneration"); // Запускаем анимацию регенерации

        // Ожидание окончания анимации
        yield return new WaitForSeconds(1.0f); // Длительность анимации регенерации

        // Восстанавливаем здоровье
        currentHealth += 20; // Можно регулировать количество восстанавливаемого здоровья
        Debug.Log("Босс восстанавливает здоровье."); // Отладочное сообщение

        // Спавн молний
        for (int i = 0; i < 5; i++) // Спавн 5 молний
        {
            SpawnLightning();
            yield return new WaitForSeconds(0.5f); // Задержка между спавном молний
        }

        // Возвращаем неуязвимость
        yield return new WaitForSeconds(invulnerabilityDuration); // Длительность неуязвимости

        isRegenerating = false;
        isInvulnerable = false; // Босс больше не неуязвим
        Debug.Log("Босс больше не неуязвим."); // Отладочное сообщение
    }

    private void SpawnLightning()
    {
        // Генерация случайной позиции в радиусе
        Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * lightningSpawnRadius;
        Instantiate(lightningPrefab, randomPosition, Quaternion.identity); // Создаем молнию
    }

    protected override void MoveTowardsPlayer()
    {
        if (player != null && !isInvulnerable) // Если неуязвим, не двигаться
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > attackRange)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.MovePosition((Vector2)transform.position + direction * enemyMoveSpeed * Time.deltaTime);
                FlipSprite(direction);

                // Включение анимации idle
                animator.SetBool("IsMoving", true);
            }
            else
            {
                // Если босс близко к игроку, останавливаем движение
                animator.SetBool("IsMoving", false);
            }
        }
    }

    // Метод, вызываемый в конце анимации атаки
    public void DealDamageToPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage((int)damage);
            Debug.Log("Босс наносит урон игроку!");
        }
    }

    public override void TakeDamage(int damage, bool isCriticalHit, bool isDoubleDamage = false) // Убедитесь, что параметр присутствует
    {
        if (isDead) return;

        // Обработка критического удара
        if (isCriticalHit)
        {
            damage = (int)(damage + damage / 100 * weapon.criticalDamage); // Увеличиваем урон, если это критический удар
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
    }


    protected override void Die()
    {
        base.Die();
        Debug.Log("Босс погибает!");
    }
}
