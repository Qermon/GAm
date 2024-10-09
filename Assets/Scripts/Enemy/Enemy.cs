using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int health = 100;
    public int currentHealth;
    public Transform player; // Ссылка на объект игрока
    public float moveSpeed; // Скорость передвижения моба
    public float attackRange = 1.5f;
    public int damage = 10;
    public float attackSpeed = 1f; // Скорость атаки в ударах в секунду
    private float attackCooldown; // Время, когда враг может снова атаковать

    public GameObject enemyPrefab; // Префаб ворога
    public Transform spawnPoint; // Точка спавну
    void Start()
    {
        currentHealth = health;
        attackCooldown = 0f; // Изначально враг может атаковать
        player = FindObjectOfType<PlayerMovement>().transform; // Найти объект игрока
    }

    void Update()
    {
        if (player == null) return; // Если игрок не задан, выходим

        // Рассчитать направление на игрока
        Vector2 moveDir = (player.position - transform.position).normalized;

        // Поворот моба в сторону игрока
        FlipSprite(moveDir);

        // Движение моба в сторону игрока
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // Уменьшаем таймер кулдауна
        attackCooldown -= Time.deltaTime;

        // Проверяем, может ли враг атаковать
        if (Vector2.Distance(transform.position, player.position) < attackRange && attackCooldown <= 0)
        {
            AttackPlayer();
        }
    }

     public static Enemy Spawn()
    {
        return new Enemy(); // Повертаємо новий об'єкт Enemy
    }

    public void AttackPlayer()
    {
        // Наносим урон игроку
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        // Устанавливаем время на кулдаун
        attackCooldown = 1f / attackSpeed; // Например, если скорость атаки 2, кулдаун будет 0.5 секунды
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        gameObject.SetActive(false); // Или уничтожьте объект
    }


    void FlipSprite(Vector2 direction)
    {
        // Получаем текущий масштаб
        Vector3 currentScale = transform.localScale;

        // Если игрок находится справа, развернуть моба вправо (по оси X), если слева — влево
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
        }
    }
    public static Enemy Spawn(GameObject enemyPrefab, Transform spawnPoint)
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            GameObject enemyObject = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Ворог спавнено!");

            // Повертаємо компонент Enemy з нового об'єкта
            return enemyObject.GetComponent<Enemy>();
        }
        else
        {
            Debug.LogError("Префаб ворога або точка спавну не вказані!");
            return null; // Якщо не вдалося спавнити ворога
        }
    }

    public bool IsAlive()
    {
        return currentHealth > 0; // Ворог живий, якщо здоров'я більше 0
    }
}

