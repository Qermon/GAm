using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Основные характеристики мобов
    public int maxHealth = 100;
    protected int currentHealth;
    public float enemyMoveSpeed = 2f;
    public int damage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    protected bool isDead = false;

    protected Transform player; // Ссылка на игрока
    private float attackTimer = 0f; // Внутренний таймер для контроля атак

    // Публичные поля для крови и опыта
    public GameObject experienceItemPrefab;
    public int experienceAmount = 20;
    public GameObject[] bloodPrefabs; // Массив текстур крови

    // Ссылка на BloodManager
    public BloodManager bloodManager;

    protected virtual void Start()
    {
        // Инициализация здоровья и нахождение игрока
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        if (isDead || player == null) return;

        // Движение к игроку
        MoveTowardsPlayer();

        // Проверка на возможность атаки
        attackTimer -= Time.deltaTime;
        if (Vector2.Distance(transform.position, player.position) <= attackRange && attackTimer <= 0f)
        {
            AttackPlayer();
        }
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
            if (bloodManager != null)
            {
                StartCoroutine(bloodManager.RemoveBlood(blood)); // Запускаем корутину для удаления крови
            }
            else
            {
                Debug.LogWarning("BloodManager не задан в Enemy.");
            }
        }
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
}
