using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject[] deathMobPrefabs;  // Префабы мобов Смерти
    public GameObject[] deathPrefabs;
    public GameObject[] batPrefabs;       // Префабы мобов Летучих мышей
    public GameObject[] wizardPrefabs;    // Префабы мобов магов
    public GameObject[] skeletonPrefabs;  // Префабы скелетов
    public GameObject[] archerPrefabs;    // Префабы лучников
    public GameObject[] boomPrefabs;      // Префабы бомбардировщиков
    public GameObject[] healerPrefabs;    // Префабы лекарей
    public GameObject[] buffMobPrefabs;   // Префабы бафферов
    public Transform[] spawnPoints;       // Точки спауна
    public float timeBetweenWaves = 5f;

    private int waveNumber = 0;
    public int maxWaves = 10;
    private bool spawningWave = false;
    private float timeStartedWave;
    private float waveDuration;

    private List<GameObject> activeEnemies = new List<GameObject>();

    public BloodManager bloodManager;

    private Dictionary<int, WaveConfig> waveConfigs;

    void Start()
    {
        InitializeWaves();
    }

    void Update()
    {
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
    }

    IEnumerator SpawnWave()
    {
        spawningWave = true;
        waveNumber++;

        if (waveConfigs.ContainsKey(waveNumber))
        {
            WaveConfig currentWave = waveConfigs[waveNumber];
            waveDuration = currentWave.waveDuration;
            timeStartedWave = Time.time;

            // Создаем списки для мобов
            List<EnemySpawn> nonBatAndDeathMobs = new List<EnemySpawn>();
            List<EnemySpawn> batsAndDeathMobs = new List<EnemySpawn>();

            foreach (var enemySpawn in currentWave.enemiesToSpawn)
            {
                // Разделяем мобы на группы
                if (enemySpawn.enemyPrefab.name.Contains("Bat") || enemySpawn.enemyPrefab.name.Contains("Мобы смерти"))
                {
                    batsAndDeathMobs.Add(enemySpawn);
                }
                else
                {
                    nonBatAndDeathMobs.Add(enemySpawn);
                }
            }

            // Спавним мобы
            float timeElapsed = 0f;

            // Если есть мобы летучих мышей или мобы смерти, создаем отдельный поток для их спавна
            if (batsAndDeathMobs.Count > 0)
            {
                StartCoroutine(SpawnBatsAndDeaths(batsAndDeathMobs, currentWave.spawnInterval));
            }

            // Спавним обычные мобы в течение всей волны
            foreach (var enemySpawn in nonBatAndDeathMobs)
            {
                for (int j = 0; j < enemySpawn.count; j++)
                {
                    Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                    GameObject enemyPrefab = enemySpawn.enemyPrefab;

                    GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                    AddEnemy(enemy);
                    yield return new WaitForSeconds(currentWave.spawnInterval);
                }
            }

            // Ждем завершения всей волны
            while (timeElapsed < waveDuration)
            {
                timeElapsed += currentWave.spawnInterval;
                yield return new WaitForSeconds(currentWave.spawnInterval);
            }

            // Уничтожаем оставшихся мобов после завершения волны
            RemoveRemainingEnemies();
        }

        spawningWave = false;
        StartCoroutine(StartNextWave());
    }

    private IEnumerator SpawnBatsAndDeaths(List<EnemySpawn> batsAndDeathMobs, float spawnInterval)
    {
        while (spawningWave)
        {
            foreach (var enemySpawn in batsAndDeathMobs)
            {
                for (int j = 0; j < enemySpawn.count; j++)
                {
                    Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                    GameObject enemyPrefab = enemySpawn.enemyPrefab;

                    GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                    AddEnemy(enemy);
                    yield return new WaitForSeconds(spawnInterval);
                }
            }
        }
    }





    private void InitializeWaves()
    {
        waveConfigs = new Dictionary<int, WaveConfig>();

        // Волна 1
        waveConfigs.Add(1, new WaveConfig(20f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(50 + 1 * 1.5f)) // 50 Смертей
    }));

        // Волна 2
        waveConfigs.Add(2, new WaveConfig(25f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(50 + 2 * 1.5f)), // 50 Смертей
        new EnemySpawn(batPrefabs[0], 10) // 10 Летучих мышей
    }));

        // Волна 3
        waveConfigs.Add(3, new WaveConfig(30f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(55 + 3 * 1.5f)), // 55 Смертей
        new EnemySpawn(batPrefabs[0], 20) // 20 Летучих мышей
    }));

        // Волна 4
        waveConfigs.Add(4, new WaveConfig(35f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(55 + 4 * 1.5f)), // 55 Смертей
        new EnemySpawn(batPrefabs[0], 20), // 20 Летучих мышей
        new EnemySpawn(wizardPrefabs[0], 5) // 5 Магов
    }));

        // Волна 5
        waveConfigs.Add(5, new WaveConfig(40f, new List<EnemySpawn>
    {
        new EnemySpawn(batPrefabs[0], Mathf.FloorToInt(50 + 5 * 1.5f)) // 50 Летучих мышей
    }));

        // Волна 6
        waveConfigs.Add(6, new WaveConfig(45f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(60 + 6 * 1.5f)), // 60 Смертей
        new EnemySpawn(deathPrefabs[0], 6), // 6 Мобов Смерти
        new EnemySpawn(wizardPrefabs[0], 5), // 5 Магов
        new EnemySpawn(batPrefabs[0], 20) // 20 Летучих мышей
    }));

        // Волна 7
        waveConfigs.Add(7, new WaveConfig(50f, new List<EnemySpawn>
    {
        new EnemySpawn(batPrefabs[0], Mathf.FloorToInt(50 + 7 * 1.5f)), // 50 Летучих мышей
        new EnemySpawn(archerPrefabs[0], 5), // 5 Лучников
        new EnemySpawn(wizardPrefabs[0], 5), // 5 Магов
        new EnemySpawn(boomPrefabs[0], 5) // 5 Бомбардировщиков
    }));

        // Волна 8
        waveConfigs.Add(8, new WaveConfig(55f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(20 + 8 * 1.5f)), // 20 Смертей
        new EnemySpawn(skeletonPrefabs[0], 10), // 10 Скелетов
        new EnemySpawn(boomPrefabs[0], 5), // 5 Бомбардировщиков
        new EnemySpawn(healerPrefabs[0], 2), // 2 Лекаря
        new EnemySpawn(deathPrefabs[0], 15), // 5 Мобов Смерти
        new EnemySpawn(wizardPrefabs[0], 5), // 5 Магов
        new EnemySpawn(archerPrefabs[0], 2) // 2 Лучника
    }));

        // Волна 9
        waveConfigs.Add(9, new WaveConfig(60f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(20 + 9 * 1.5f)), // 20 Смертей
        new EnemySpawn(skeletonPrefabs[0], 10), // 10 Скелетов
        new EnemySpawn(boomPrefabs[0], 5), // 5 Бомбардировщиков
        new EnemySpawn(healerPrefabs[0], 2), // 2 Лекаря
        new EnemySpawn(deathPrefabs[0], 15), // 5 Мобов Смерти
        new EnemySpawn(wizardPrefabs[0], 5), // 5 Магов
        new EnemySpawn(archerPrefabs[0], 2), // 2 Лучника
        new EnemySpawn(buffMobPrefabs[0], 2) // 2 Баффера
    }));

        // Волна 10
        waveConfigs.Add(10, new WaveConfig(60f, new List<EnemySpawn>
    {
        new EnemySpawn(batPrefabs[0], Mathf.FloorToInt(100 + 10 * 1.5f)), // 100 Летучих мышей
        new EnemySpawn(wizardPrefabs[0], 5), // 5 Магов
        new EnemySpawn(boomPrefabs[0], 5) // 5 Бомбардировщиков
    }));
    }


    public int GetWaveNumber()
    {
        return waveNumber;
    }

    public void AddEnemy(GameObject enemy)
    {
        if (enemy != null && !activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    public float GetTimeUntilNextWave()
    {
        if (spawningWave)
        {
            float timePassed = Time.time - timeStartedWave;
            float timeUntilNext = waveDuration - timePassed;

            if (timeUntilNext <= 0)
            {
                timeUntilNext = 0;
                RemoveRemainingEnemies();
            }

            return timeUntilNext;
        }

        return -1;
    }

    void RemoveRemainingEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();
    }
}

public class WaveConfig
{
    public float waveDuration;
    public List<EnemySpawn> enemiesToSpawn;
    public float spawnInterval;

    public WaveConfig(float waveDuration, List<EnemySpawn> enemies)
    {
        this.waveDuration = waveDuration;
        this.enemiesToSpawn = enemies;
        this.spawnInterval = waveDuration / GetTotalEnemies();
    }

    private int GetTotalEnemies()
    {
        int total = 0;
        foreach (var enemySpawn in enemiesToSpawn)
        {
            total += enemySpawn.count;
        }
        return total;
    }
}

public class EnemySpawn
{
    public GameObject enemyPrefab;
    public int count;

    public EnemySpawn(GameObject prefab, int count)
    {
        this.enemyPrefab = prefab;
        this.count = count;
    }
}
