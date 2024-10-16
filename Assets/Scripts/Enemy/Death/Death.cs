using System.Collections;
using UnityEngine;

public class Death : Enemy // Убедитесь, что MobDeath наследует от Enemy
{
    public float moveSpeed = 0.8f; // Скорость передвижения
    public float spawnInterval = 7f; // Интервал между спавнами
    public int mobsToSpawn = 5; // Количество спавнимых мобов
    public GameObject miniMobPrefab; // Префаб мини-моба
    private bool isSpawning = false; // Флаг, указывающий на спавн
    private Animator animator; // Аниматор

    public GameObject enemyPrefab;
    public Transform[] summonPoints;

    protected override void Start() // Используем override
    {
        base.Start(); // Вызываем метод Start() базового класса
        animator = GetComponent<Animator>();

        // Запускаем корутину для спавна мобов
        StartCoroutine(SpawnMobRoutine());
    }

    protected override void Update() // Используем override
    {
        base.Update(); // Вызываем метод Update() базового класса
        if (!isSpawning) // Проверяем, не спавним ли мобов
        {
            MoveMob(); // Двигаем моба, если он не спавнит
        }
    }

    void MoveMob()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            FlipSprite(direction); // Метод для поворота спрайта
        }
    }

    IEnumerator SpawnMobRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval - 1f); // Ждем 6 секунд перед началом спавна
            isSpawning = true; // Устанавливаем флаг спавна
            animator.SetBool("IsChasing", false); // Выключаем анимацию преследования
            animator.SetBool("IsIdle", true); // Включаем анимацию Idle

            // Задержка на 1 секунду перед спавном, когда моб будет в Idle анимации
            yield return new WaitForSeconds(1f); // Ждем 1 секунду

            for (int i = 0; i < mobsToSpawn; i++)
            {
                Vector2 spawnPosition = GetRandomSpawnPosition(); // Получаем случайную позицию спавна
                SpawnMiniMob(spawnPosition); // Спавним мини-моба
                yield return new WaitForSeconds(0.5f); // Небольшая задержка между спавном мобов
            }

            // После завершения спавна возвращаем моба к нормальному состоянию
            animator.SetBool("IsIdle", false); // Выключаем Idle после спавна
            isSpawning = false; // Завершаем спавн
        }
    }


    Vector2 GetRandomSpawnPosition()
    {
        Vector2 randomDirection;
        Vector2 spawnPosition;

        // Ищем подходящее место для спавна
        do
        {
            randomDirection = Random.insideUnitCircle.normalized; // Случайное направление
            float distance = Random.Range(0f, 1.5f); // Случайное расстояние в радиусе 1.5
            spawnPosition = (Vector2)transform.position + randomDirection * distance; // Позиция спавна
        }
        while (Physics2D.OverlapCircle(spawnPosition, 0.1f, LayerMask.GetMask("Wall"))); // Проверяем, не попадаем ли в триггер "Wall"

        return spawnPosition; // Возвращаем допустимую позицию спавна
    }


    bool IsInsideTrigger(Vector2 position)
    {
        // Проверка, находится ли позиция в триггер-коллайдере с тегом Wall
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return true; // Позиция внутри триггера с тегом Wall
            }
        }
        return false; // Позиция не внутри триггера
    }

    void SpawnMiniMob(Vector2 spawnPosition)
    {
        // Спавн мини-моба в указанной позиции
        Instantiate(miniMobPrefab, spawnPosition, Quaternion.identity);
    }



    // Метод, который отвечает за призыв мобов
    public void SummonEnemies()
    {
        foreach (Transform summonPoint in summonPoints)
        {
            // Создаём моба
            GameObject summonedEnemy = Instantiate(enemyPrefab, summonPoint.position, summonPoint.rotation);

            // Добавляем моба в список активных врагов через WaveManager
            FindObjectOfType<WaveManager>().AddEnemy(summonedEnemy);
        }
    }
}