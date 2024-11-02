using System.Collections;
using UnityEngine;

public class Skeletonboss : MonoBehaviour
{
    // Характеристики босса
    public int goldAmount = 50;
    public int maxHealth = 500;
    public float currentHealth; // Сделать public для доступа
    public float bossMoveSpeed = 1f;
    public int meleeDamage = 20;
    public float meleeAttackRange = 5f;
    public float meleeAttackCooldown = 2f;

    private bool isDead = false;
    private Transform player;
    private float meleeAttackTimer = 0f;

    // Флаги для контроля призыва мобов
    private bool hasSummonedAt70 = false;
    private bool hasSummonedAt50 = false;
    private bool hasSummonedAt30 = false;

    // Публичные поля для крови и опыта
    public GameObject experienceItemPrefab;
    public int experienceAmount = 100;

    void Start()
    {
        currentHealth = maxHealth;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player не найден!");
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        meleeAttackTimer -= Time.deltaTime;

        // Проверка на уровни здоровья и призыв мобов
        CheckHealthForSummoning();

        if (Vector2.Distance(transform.position, player.position) <= meleeAttackRange && meleeAttackTimer <= 0f)
        {
            MeleeAttack();
        }

        MoveTowardsPlayer();
    }

    // Проверка здоровья для призыва мобов
    void CheckHealthForSummoning()
    {
        if (currentHealth <= maxHealth * 0.7f && !hasSummonedAt70)
        {
            SummonMobs();
            hasSummonedAt70 = true;
        }

        if (currentHealth <= maxHealth * 0.5f && !hasSummonedAt50)
        {
            SummonMobs();
            hasSummonedAt50 = true;
        }

        if (currentHealth <= maxHealth * 0.3f && !hasSummonedAt30)
        {
            SummonMobs();
            hasSummonedAt30 = true;
        }
    }

    void MeleeAttack()
    {
        meleeAttackTimer = meleeAttackCooldown;

        // Запускаем анимацию удара
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("skill_1"); // Устанавливаем триггер для анимации удара
        }

        // Наносим урон игроку
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(meleeDamage);
        }
    }

    // Призыв мобов
    void SummonMobs()
    {
        // Запускаем анимацию призыва
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("skill_2"); // Устанавливаем триггер для анимации призыва
        }

        // Логика призыва мобов
        Debug.Log("Босс призвал мобов!");
    }

    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, bossMoveSpeed * Time.deltaTime);

            // Устанавливаем триггер анимации бега
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("run"); // Устанавливаем триггер для анимации бега
            }

            FlipSprite(direction);
        }
    }

    void FlipSprite(Vector2 direction)
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Босс повержен!");

        // Начисление золота и опыта
        PlayerGold playerGold = player.GetComponent<PlayerGold>();
        if (playerGold != null)
        {
            playerGold.AddGold(goldAmount);
        }

        SpawnExperience();
        Destroy(gameObject);
    }

    void SpawnExperience()
    {
        if (experienceItemPrefab != null)
        {
            Instantiate(experienceItemPrefab, transform.position, Quaternion.identity);
        }
    }
}
