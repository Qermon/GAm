using System.Collections;
using UnityEngine;

public class Bat : MonoBehaviour
{
    // Параметры здоровья
    private int health = 100;
    public int currentHealth;

    // Параметры передвижения и атаки
    public Transform player;  // Ссылка на игрока
    public float moveSpeed = 6f;  // Скорость перемещения
    private Vector2 targetDirection;  // Направление на игрока
    private bool isStopped = false;   // Флаг остановки

    // Параметры атаки
    public float attackRange = 1.5f;  // Дистанция атаки
    public int damage = 10;  // Урон игроку
    public float attackSpeed = 1f;  // Скорость атаки (ударов в секунду)
    private float attackCooldown = 0f;  // Таймер кулдауна атаки

    // Спавн врага и опыт
    public GameObject experienceItemPrefab;  // Префаб предмета опыта
    public int experienceAmount = 20;  // Количество опыта

    // Эффекты крови
    public GameObject[] bloodPrefabs;  // Префабы кровавых следов
    private float bloodCooldown = 0.5f;  // Таймер для спавна крови
    private float lastBloodTime = 0f;  // Последний раз, когда появилась кровь



    void Start()
    {
        // Инициализируем текущее здоровье
        currentHealth = health;

        // Находим игрока в сцене
        player = FindObjectOfType<PlayerMovement>().transform;

        // Устанавливаем направление на игрока
        if (player != null)
        {
            UpdateTargetDirection();
        }
    }

    void Update()
    {
        if (player == null) return;  // Если игрок не найден, ничего не делаем

        // Если моб не остановлен, то перемещаем его в сторону игрока
        if (!isStopped)
        {
            transform.position += (Vector3)targetDirection * moveSpeed * Time.deltaTime;

            // Если враг в радиусе атаки, проверяем, можно ли атаковать
            if (Vector2.Distance(transform.position, player.position) < attackRange && attackCooldown <= 0)
            {
                AttackPlayer();
            }
        }

        // Обновляем таймер кулдауна атаки
        attackCooldown -= Time.deltaTime;
    }

    // Обновляем направление к игроку
    private void UpdateTargetDirection()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        // Поворачиваем моба в сторону игрока
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Устанавливаем направление для движения
        targetDirection = directionToPlayer;
    }

    // Обрабатываем столкновение с триггером (например, стеной)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            StartCoroutine(StopAndTurn());
        }
    }

    // Корутин для остановки моба, его разворота и повторного движения к игроку
    IEnumerator StopAndTurn()
    {
        isStopped = true;

        // Обновляем направление на игрока
        UpdateTargetDirection();

        // Поворачиваемся к игроку
        float turnDuration = 0.5f; // Время на поворот
        float timeElapsed = 0f;

        while (timeElapsed < turnDuration)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Движение к игроку с замедленной скоростью
        float moveDuration = 2f; // Время движения к игроку
        float moveSpeedReduced = 0.75f; // Замедленная скорость
        timeElapsed = 0f;

        while (timeElapsed < moveDuration)
        {
            transform.position += (Vector3)targetDirection * moveSpeedReduced * Time.deltaTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // После движения к игроку, обновляем направление на игрока для продолжения преследования
        UpdateTargetDirection();
        isStopped = false;
    }

    // Атака игрока
    public void AttackPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        // Устанавливаем кулдаун атаки
        attackCooldown = 1f / attackSpeed;
    }

    // Получение урона мобом
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Спавн крови при получении урона
        if (Time.time >= lastBloodTime + bloodCooldown)
        {
            SpawnBlood();
            lastBloodTime = Time.time;
        }

        // Если здоровье закончилось — моб умирает
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Спавн эффекта крови
    void SpawnBlood()
    {
        if (bloodPrefabs.Length == 0) return;

        GameObject randomBlood = bloodPrefabs[Random.Range(0, bloodPrefabs.Length)];
        GameObject blood = Instantiate(randomBlood, transform.position, Quaternion.identity);
        StartCoroutine(FadeAndDestroy(blood, 3f));
    }

    // Плавное исчезновение и уничтожение объекта крови
    IEnumerator FadeAndDestroy(GameObject blood, float fadeDuration)
    {
        SpriteRenderer bloodRenderer = blood.GetComponent<SpriteRenderer>();
        if (bloodRenderer == null) yield break;

        Color originalColor = bloodRenderer.color;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timeElapsed / fadeDuration);
            bloodRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(blood);
    }

    // Умирание моба
    void Die()
    {
        Debug.Log("Bat died!");

        // Спавн предмета опыта
        if (experienceItemPrefab != null)
        {
            Instantiate(experienceItemPrefab, transform.position, Quaternion.identity);
        }

        // Отключаем объект
        gameObject.SetActive(false);
    }

    // Вспомогательный метод для проверки жив ли моб
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}
