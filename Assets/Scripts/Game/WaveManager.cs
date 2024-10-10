using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;        // Префаб врага
    public GameObject crossPrefab;        // Префаб зеленого крестика
    public Transform[] spawnPoints;       // Точки появления врагов
    public float timeBetweenWaves = 5f;   // Время между волнами
    public float waveDuration = 30f;      // Время продолжительности волны
    private int waveNumber = 0;           // Номер текущей волны
    public int maxWaves = 50;             // Максимальное количество волн
    public int maxActiveEnemies = 50;     // Максимальное количество активных врагов одновременно

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
        // Ждем между волнами
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
        int enemiesToSpawn = Mathf.FloorToInt(100 + waveNumber * 1.5f);
        enemiesToSpawn = Mathf.Min(enemiesToSpawn, maxActiveEnemies); // Убедимся, что количество врагов не превышает лимит

        // Рассчитываем время между спавнами врагов
        float spawnInterval = waveDuration / enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // Проверяем количество активных врагов
            while (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxActiveEnemies)
            {
                yield return null; // Ждем, пока количество активных врагов уменьшится
            }

            // Случайная точка появления
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Спавним крестик перед мобом
            GameObject cross = Instantiate(crossPrefab, spawnPoint.position, Quaternion.identity);
            Destroy(cross, 1f); // Уничтожаем крестик через 1 секунду

            // Небольшая задержка перед спавном моба, чтобы крестик успел отобразиться
            yield return new WaitForSecondsRealtime(0.3f);

            // Спавн врага
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // Задержка перед следующим спавном врага
            yield return new WaitForSecondsRealtime(spawnInterval - 0.3f); // Учитываем время крестика
        }

        // Ждем время продолжительности волны, прежде чем удалять оставшихся врагов
        yield return new WaitForSecondsRealtime(waveDuration - (enemiesToSpawn * spawnInterval));

        // Удаление всех оставшихся врагов
        RemoveRemainingEnemies();

        spawningWave = false; // Позволяем запуск новой волны
    }

    void RemoveRemainingEnemies()
    {
        GameObject[] remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in remainingEnemies)
        {
            Destroy(enemy);
        }
    }

    public int GetWaveNumber()
    {
        return waveNumber; // Возвращаем текущий номер волны
    }

    public float GetTimeUntilNextWave()
    {
        if (spawningWave)
        {
            float timePassed = Time.time - timeStartedWave; // Время с начала волны
            float timeUntilNext = (waveDuration) - timePassed; // Осталось времени до конца волны

          

            if (timeUntilNext <= 0)
            {
                RemoveRemainingEnemies(); // Удаление всех оставшихся врагов
                StartCoroutine(StartNextWave()); // Запускаем новую волну после задержки
                return 0; // Возвращаем 0, если время до следующей волны закончилось
            }

            return timeUntilNext; // Возвращаем оставшееся время
        }
        return -1; // Если волна не спавнится, возвращаем -1
    }
}
