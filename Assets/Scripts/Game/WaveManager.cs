using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;        // Префаб врага
    public GameObject crossPrefab;        // Префаб зеленого крестика
    public Transform[] spawnPoints;       // Точки появления врагов
    public float timeBetweenWaves = 5f;   // Время между волнами
    public float waveDuration = 30f;      // Время продолжительности волны
    private int waveNumber = 0;           // Номер текущей волны
    public int maxWaves = 1000;             // Максимальное количество волн
    public int maxActiveEnemies = 1000;     // Максимальное количество активных врагов одновременно

    private bool spawningWave = false;    // Флаг для отслеживания спавна волны
    private float timeStartedWave;         // Время начала волны

    private bool isWaveEnded = false; // Флаг для отслеживания завершения волны

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

        // Устанавливаем время продолжительности волны
        float waveDuration = 30f; // Устанавливаем продолжительность волны в 30 секунд
        float spawnDuration = waveDuration - 5f; // Время, за которое нужно заспавнить всех врагов

        // Определение количества врагов в зависимости от номера волны
        int enemiesToSpawn = Mathf.FloorToInt(100 + waveNumber * 1.5f);
        enemiesToSpawn = Mathf.Min(enemiesToSpawn, maxActiveEnemies); // Убедимся, что количество врагов не превышает лимит

        // Рассчитываем время между спавном врагов
        float spawnInterval = spawnDuration / enemiesToSpawn; // Делим на количество врагов

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

            // Запускаем корутину для спавна врага
            StartCoroutine(SpawnEnemy(cross, spawnPoint));

            // Задержка перед следующим спавном врага
            yield return new WaitForSecondsRealtime(spawnInterval); // Используем фиксированный интервал
        }

        // Ждем оставшиеся 5 секунд после спавна всех врагов
        yield return new WaitForSecondsRealtime(5f);

        // Удаление всех оставшихся врагов
        RemoveRemainingEnemies();

        spawningWave = false; // Позволяем запуск новой волны
        StartCoroutine(StartNextWave()); // Запускаем следующую волну
    }

    IEnumerator SpawnEnemy(GameObject cross, Transform spawnPoint)
    {
        // Небольшая задержка, чтобы крестик успел отобразиться
        yield return new WaitForSecondsRealtime(0.5f); // Задержка 0.5 секунд

        // Проверяем, все ли еще в процессе спавна
        if (!spawningWave) // Если волна завершена, выходим
        {
            Destroy(cross);
            yield break; // Прерываем корутину
        }

        // Спавн врага
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Уничтожаем крестик сразу после спавна врага
        Destroy(cross);
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
            float timeUntilNext = waveDuration - timePassed; // Осталось времени до конца волны

            // Если время до конца волны <= 0
            if (timeUntilNext <= 0)
            {
                timeUntilNext = 0; // Устанавливаем время в 0, чтобы оно не уходило в отрицательные значения
                RemoveRemainingEnemies(); // Удаление всех оставшихся врагов

                // Устанавливаем флаг, что волна завершена
                isWaveEnded = true;

                // Запускаем новую волну, только если она еще не запущена
                if (!spawningWave)
                {
                    StartCoroutine(StartNextWave()); // Запускаем новую волну после задержки
                }
            }

            return timeUntilNext; // Возвращаем оставшееся время
        }

        // Если волна завершена, возвращаем -1
        return isWaveEnded ? -1 : -1;
    }




}