using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Основные характеристики мобов
    public int goldAmount = 10;
    public int maxHealth = 100;
    public float currentHealth; // Сделать public
    public float enemyMoveSpeed = 0f;
    public int damage = 0;
    public float attackRange = 0.1f;
    public float attackCooldown = 1f;
    protected bool isDead = false;

    protected Transform player; // Ссылка на игрока
    private float attackTimer = 0f; // Внутренний таймер для контроля атак

    private float originalMass; // Исходная масса
    private Rigidbody2D rb; // Rigidbody для изменения массы

    // Публичные поля для крови и опыта
    public GameObject experienceItemPrefab;
    public int experienceAmount = 20;
    public GameObject[] bloodPrefabs; // Массив текстур крови

    public bool IsDead
    {
        get { return isDead; }
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalMass = rb.mass; // Сохраняем исходную массу

        currentHealth = maxHealth;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Объект Player не найден в сцене!");
        }
    }

    protected virtual void Update()
    {
        if (isDead || player == null) return;

        attackTimer -= Time.deltaTime;
        if (Vector2.Distance(transform.position, player.position) <= attackRange && attackTimer <= 0f)
        {
            AttackPlayer();
        }

        if (enemyMoveSpeed <= 0)
        {
            rb.mass = originalMass + 100; // Увеличиваем массу
        }
        else
        {
            rb.mass = originalMass; // Возвращаем исходную массу
        }

        MoveTowardsPlayer();
    }

    public float GetCurrentHP()
    {
        return currentHealth;
    }

    // Метод для изменения скорости
    public void ModifySpeed(float speedMultiplier, float duration)
    {
        StartCoroutine(ApplySpeedChange(speedMultiplier, duration));
    }

    private IEnumerator ApplySpeedChange(float speedMultiplier, float duration)
    {
        float originalSpeed = enemyMoveSpeed;
        enemyMoveSpeed *= speedMultiplier; // Применяем изменение скорости

        yield return new WaitForSeconds(duration); // Ожидание конца действия

        enemyMoveSpeed = originalSpeed; // Восстанавливаем исходную скорость
    }


    // Метод для движения к игроку
    protected virtual void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemyMoveSpeed * Time.deltaTime);
            FlipSprite(direction); // Метод для поворота спрайта
        }
    }

    protected virtual void AttackPlayer()
    {
        attackTimer = attackCooldown;
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.HealOnKill(maxHealth); // Восстанавливаем здоровье игроку
        }

        // Начисляем золото игроку
        PlayerGold playerGold = player.GetComponent<PlayerGold>();
        if (playerGold != null)
        {
            playerGold.AddGold(goldAmount);
        }

        SpawnExperience();
        SpawnBlood();
        Destroy(gameObject);
    }

    protected virtual void SpawnExperience()
    {
        if (experienceItemPrefab != null)
        {
            Instantiate(experienceItemPrefab, transform.position, Quaternion.identity);
        }
    }

    protected virtual void SpawnBlood()
    {
        if (bloodPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, bloodPrefabs.Length);
            GameObject blood = Instantiate(bloodPrefabs[randomIndex], transform.position, Quaternion.identity);
            blood.tag = "Blood";
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += (int)amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log(gameObject.name + " был вылечен на " + amount + " единиц.");
    }

    protected virtual void FlipSprite(Vector2 direction)
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

    public void SetDamage(float newDamage)
    {
        damage = (int)newDamage;
    }
}
