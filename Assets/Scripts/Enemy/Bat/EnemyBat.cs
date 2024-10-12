using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBat : MonoBehaviour
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

    // Новый префаб предмета, который увеличивает опыт игрока
    public GameObject experienceItemPrefab;
    public int experienceAmount = 20; // Количество опыта, которое даст предмет

    public GameObject[] bloodPrefabs; // Массив для нескольких текстур крови
    private float bloodCooldown = 0.5f;
    private float lastBloodTime = 0f;

    


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

    public static EnemyBat Spawn()
    {
        return new EnemyBat(); // Повертаємо новий об'єкт Enemy
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

        if (Time.time >= lastBloodTime + bloodCooldown)
        {
            SpawnBlood();
            lastBloodTime = Time.time;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void SpawnBlood()
    {
        if (bloodPrefabs.Length == 0) return;

        // Выбираем случайную текстуру крови
        GameObject randomBlood = bloodPrefabs[Random.Range(0, bloodPrefabs.Length)];

        // Спавним текстуру крови
        GameObject blood = Instantiate(randomBlood, transform.position, Quaternion.identity);

        // Начать корутину для плавного исчезновения и удаления через 3 секунды
        StartCoroutine(FadeAndDestroy(blood, 3f));
    }


    IEnumerator FadeAndDestroy(GameObject blood, float fadeDuration)
    {
        // Получаем SpriteRenderer для объекта крови
        SpriteRenderer bloodRenderer = blood.GetComponent<SpriteRenderer>();

        // Если SpriteRenderer не найден, выводим сообщение и завершаем выполнение
        if (bloodRenderer == null)
        {
            Debug.LogError("SpriteRenderer не найден на объекте крови: " + blood.name);
            yield break;
        }

        // Оригинальный цвет крови
        Color originalColor = bloodRenderer.color;
        float timeElapsed = 0f;

        Debug.Log("Начало исчезновения крови для объекта: " + blood.name);

        // Плавное исчезновение
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timeElapsed / fadeDuration); // Линейная интерполяция для альфа-канала
            bloodRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Debug.Log("Объект крови будет уничтожен: " + blood.name);

        // Удаление объекта после полного исчезновения
        Destroy(blood);
    }





    void Die()
    {
        Debug.Log("Enemy died!");

        // Спавним предмет опыта после смерти врага
        if (experienceItemPrefab != null)
        {
            Instantiate(experienceItemPrefab, transform.position, Quaternion.identity);
        }

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

    public static EnemyBat Spawn(GameObject enemyPrefab, Transform spawnPoint)
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            GameObject enemyObject = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Ворог спавнено!");

            // Повертаємо компонент Enemy з нового об'єкта
            return enemyObject.GetComponent<EnemyBat>();
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