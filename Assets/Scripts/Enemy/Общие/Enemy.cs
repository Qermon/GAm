using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Основные характеристики мобов
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

    // Ссылка на BloodManager

    public bool IsDead
    {
        get { return isDead; }
    }




    protected virtual void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        originalMass = rb.mass; // Сохраняем исходную массу


        // Инициализация здоровья и нахождение игрока
        currentHealth = maxHealth;
        // Получаем ссылку на объект игрока
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        // Проверяем, найден ли объект с тегом "Player"
        if (playerObject != null)
        {
            // Если найден, получаем Transform
            player = playerObject.transform;
        }
        else
        {
            // Если не найден, выводим ошибку в консоль
            Debug.LogError("Объект Player не найден в сцене!");
        }
    }

    protected virtual void Update()
    {
        if (isDead || player == null) return;



        // Проверка на возможность атаки
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

        // Здесь может быть логика для движения моба

        MoveTowardsPlayer();
    }

    public float GetCurrentHP()
    {
        return currentHealth;
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

    // Метод для атаки игрока
    protected virtual void AttackPlayer()
    {
        attackTimer = attackCooldown;
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    // Метод для получения урона
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    // Метод для смерти
    protected virtual void Die()
    {
        isDead = true;

        // Восстановление здоровья игрока при убийстве
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.HealOnKill(maxHealth); // Восстанавливаем здоровье на основе здоровья врага
        }

        SpawnExperience();
        SpawnBlood();
        Destroy(gameObject);
    }

    // Спавн предмета опыта
    protected virtual void SpawnExperience()
    {
        if (experienceItemPrefab != null)
        {
            Instantiate(experienceItemPrefab, transform.position, Quaternion.identity);
        }
    }

    // Спавн крови
    protected virtual void SpawnBlood()
    {
        if (bloodPrefabs.Length > 0) // Проверка, есть ли текстуры крови в массиве
        {
            // Случайный выбор текстуры крови из массива
            int randomIndex = Random.Range(0, bloodPrefabs.Length);
            GameObject blood = Instantiate(bloodPrefabs[randomIndex], transform.position, Quaternion.identity);
            blood.tag = "Blood"; // Устанавливаем тег для объекта крови

            // Если ссылка на BloodManager задана, запускаем корутину для удаления крови

        }
    }

    public void Heal(float amount)
    {
        if (isDead) return; // Если враг мертв, он не может быть вылечен

        currentHealth += (int)amount; // Приведение float к int

        // Убедимся, что текущее здоровье не превышает максимальное
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log(gameObject.name + " был вылечен на " + amount + " единиц.");
    }



    // Метод для поворота спрайта моба в сторону игрока
    protected virtual void FlipSprite(Vector2 direction)
    {
        Vector3 localScale = transform.localScale;
        if (direction.x > 0)
        {
            localScale.x = Mathf.Abs(localScale.x); // Повернуть вправо
        }
        else if (direction.x < 0)
        {
            localScale.x = -Mathf.Abs(localScale.x); // Повернуть влево
        }
        transform.localScale = localScale;
    }

    public void SetDamage(float newDamage)
    {
        damage = (int)newDamage; // Устанавливаем новый урон
    }

}