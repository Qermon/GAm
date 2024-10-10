using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;        // Префаб врага
    public Transform[] spawnPoints;       // Точки появления врагов
    public float timeBetweenWaves = 5f;   // Время между волнами
    public float waveDuration = 30f;      // Время продолжительности волны
    private int waveNumber = 0;           // Номер текущей волны
    public int maxWaves = 50;             // Максимальное количество волн

    private bool spawningWave = false;    // Флаг для отслеживания спавна волны
    private float timeStartedWave;         // Время начала волны

    void Update()
    {
        // Запускаем следующую волну, если прошли все враги
        if (!spawningWave && GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && waveNumber < maxWaves)
        {
            StartCoroutine(StartNextWave());
        }
    }

    private IEnumerator StartNextWave()
    {
        // Ждем 10 секунд после завершения предыдущей волны
        yield return new WaitForSeconds(timeBetweenWaves);

        StartWave(); // Запускаем новую волну
    }

    public void StartWave()
    {
        if (!spawningWave) // Проверяем, не запущена ли волна
        {
            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        spawningWave = true;
        waveNumber++;

        // Установка времени начала волны
        timeStartedWave = Time.time;

        // Определение количества врагов в зависимости от номера волны
        int enemiesToSpawn = Mathf.FloorToInt(5 + waveNumber * 1.5f);

        // Спавн врагов
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f); // Задержка между спавнами врагов
        }

        // Ждем время продолжительности волны, прежде чем удалять оставшихся врагов
        yield return new WaitForSeconds(waveDuration);

        // Удаление всех оставшихся врагов
        RemoveRemainingEnemies();

        spawningWave = false; // Позволяем запуск новой волны
    }

    // Функция спавна врага
    void SpawnEnemy()
    {
        // Случайная точка появления
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    void RemoveRemainingEnemies()
    {
        GameObject[] remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in remainingEnemies)
        {
            Destroy(enemy);
        }
    }

    // Внутри класса WaveManager
    public int GetWaveNumber()
    {
        return waveNumber; // Возвращаем текущий номер волны
    }

   
    public float GetTimeUntilNextWave()
    {
        if (spawningWave)
        {
            // Рассчитаем оставшееся время до конца волны
            float timePassed = Time.time - timeStartedWave; // Время с начала волны
            float timeUntilNext = (waveDuration) - timePassed; // Осталось времени до конца волны

            // Логирование для отладки
            Debug.Log("Time until next wave: " + timeUntilNext);

            // Если время закончилось, возвращаем 0
            if (timeUntilNext <= 0)
            {
                // Удаление всех оставшихся врагов
                RemoveRemainingEnemies();

                // Запускаем новую волну после задержки
                StartCoroutine(StartNextWave());

                return 0; // Возвращаем 0, если время до следующей волны закончилось
            }

            return timeUntilNext; // Возвращаем оставшееся время
        }
        return -1; // Если волна не спавнится, возвращаем -1
    }
}