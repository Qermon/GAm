using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private int currentWave;
    private Player player;
    private List<Enemy> enemies;
    private bool isGameOver;

    public GameObject enemyPrefab; // Префаб врага
    public Transform spawnPoint; // Точка спавна

    public float waveDuration = 20f; // Длительность каждой волны в секундах
    public int totalWaves = 1000;
    void Start()
    {
        currentWave = 1;
        enemies = new List<Enemy>();
        isGameOver = false;

        StartGame();
    }

    public void StartGame()
    {
        // Создайте объект игрока и добавьте компонент Player
        GameObject playerObject = new GameObject("Player");
        player = playerObject.AddComponent<Player>();
        // Инициализация игрока, например, здоровье
        player.Initialize(); // Убедитесь, что у вас есть метод инициализации в классе Player

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop() // Coroutine to handle waves and game updates
    {
        while (!isGameOver && currentWave <= totalWaves) // Добавляем условие для проверки количества волн
        {
            StartWave();
            yield return new WaitForSeconds(1); // Optional: wait a second before starting the next wave
        }

        if (currentWave > totalWaves)
        {
            Debug.Log("Всё завершено! Все волны пройдены.");
        }
    }


    private void StartWave()
    {
        int waveDuration = 20; // Длительность волны в секундах
        Debug.Log($"Starting wave {currentWave}...");

        SpawnEnemies();
        StartCoroutine(WaveTimer(waveDuration));
    }

    private IEnumerator WaveTimer(int duration)
    {
        float startTime = Time.time; // Запоминаем время начала волны
        while (Time.time - startTime < duration)
        {
            if (!player.IsAlive())
            {
                EndGame();
                yield break; // Выходим из корутины, если игрок мертв
            }

            UpdateGameLogic();

            // Рассчитываем оставшееся время
            float remainingTime = duration - (Time.time - startTime);
            Debug.Log($"Оставшееся время волны: {remainingTime:F2} секунд"); // Выводим оставшееся время в консоль

            yield return null; // Ждем до следующего кадра
        }

        
        currentWave++; // Переход к следующей волне
        Debug.Log($"Хвиля {currentWave} закінчена!"); // Выводим сообщение о завершении волны
    }


    private void SpawnEnemies()
    {
        int numEnemies = currentWave * 2; // Количество врагов зависит от волны
        for (int i = 0; i < numEnemies; i++)
        {
            Enemy enemy = Enemy.Spawn(enemyPrefab, spawnPoint);
            if (enemy != null)
            {
                enemies.Add(enemy); // Добавьте врага в список, если он успешно создан
            }
        }
    }

    private void UpdateGameLogic()
    {
        // Логика для обновления состояния игры
        // Например, удаление мертвых врагов из списка
        enemies.RemoveAll(enemy => !enemy.IsAlive());
    }

    private void EndGame()
    {
        isGameOver = true;
        Debug.Log("Game Over! You died.");
    }
}
