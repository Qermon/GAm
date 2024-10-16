using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject crossPrefab;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 5f;
    public float waveDuration = 30f;
    private int waveNumber = 0;
    public int maxWaves = 1000;
    public int maxActiveEnemies = 100000;

    private bool spawningWave = false;
    private float timeStartedWave;

    private List<GameObject> activeEnemies = new List<GameObject>(); // Храним активных врагов

    public BloodManager bloodManager; // Переменная для ссылки на ваш BloodManager

    void Update()
    {
        // Запускаем следующую волну, если прошли все враги
        if (!spawningWave && activeEnemies.Count == 0 && waveNumber < maxWaves)
        {
            StartCoroutine(StartNextWave());
        }
    }

    private IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartWave();
    }

    public void StartWave()
    {
        if (!spawningWave)
        {
            StartCoroutine(SpawnWave());
        }

        RemoveBloodAfterWave();
    }

    public void RemoveBloodAfterWave()
    {
        if (bloodManager != null)
        {
            bloodManager.RemoveAllBlood();
        }
    }

    IEnumerator SpawnWave()
    {
        spawningWave = true;
        waveNumber++;
        timeStartedWave = Time.time;

        float spawnDuration = waveDuration - 5f;
        int enemiesToSpawn = Mathf.FloorToInt(50 + waveNumber * 1.5f);
        enemiesToSpawn = Mathf.Min(enemiesToSpawn, maxActiveEnemies);

        float spawnInterval = spawnDuration / enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // Проверка активных врагов
            while (activeEnemies.Count >= maxActiveEnemies)
            {
                yield return null;
            }

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject cross = Instantiate(crossPrefab, spawnPoint.position, Quaternion.identity);

            StartCoroutine(SpawnEnemy(cross, spawnPoint));

            yield return new WaitForSeconds(spawnInterval);
        }

        yield return new WaitForSeconds(5f);
        RemoveRemainingEnemies();
        spawningWave = false;
        StartCoroutine(StartNextWave());
    }

    IEnumerator SpawnEnemy(GameObject cross, Transform spawnPoint)
    {
        yield return new WaitForSeconds(0.5f);

        if (!spawningWave)
        {
            Destroy(cross);
            yield break;
        }

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        activeEnemies.Add(enemy);

        Destroy(cross);
    }

    void RemoveRemainingEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();
        // Удаляем всех оставшихся врагов с тегом "Enemy"
        RemoveAllEnemiesWithTag();

        // Очищаем список активных врагов
        activeEnemies.Clear();
    }

    public int GetWaveNumber()
    {
        return waveNumber;
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
            }

            return timeUntilNext; // Возвращаем оставшееся время
        }

        // Если волна завершена, возвращаем -1
        return -1;
    }

    public void AddEnemy(GameObject enemy)
    {
        activeEnemies.Add(enemy);

        if (enemy != null)
        {
            activeEnemies.Add(enemy);
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    public void RemoveAllEnemiesWithTag()
    {
        // Находим все объекты с тегом "Enemy"
        GameObject[] enemiesWithTag = GameObject.FindGameObjectsWithTag("Enemy");

        // Проходим по каждому объекту и удаляем его
        foreach (GameObject enemy in enemiesWithTag)
        {
            // Удаляем моба из списка активных врагов, если он там есть
            RemoveEnemy(enemy);

            // Уничтожаем объект моба
            Destroy(enemy);
        }
    }


}