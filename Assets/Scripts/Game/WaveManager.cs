using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    public Shop shop;



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
        InitializeWaves(); // Убедимся, что инициализация происходит до начала игры

        if (waveConfigs == null || waveConfigs.Count == 0)
        {
            Debug.LogError("Ошибка инициализации волн: waveConfigs не инициализирован или пустой.");
            return;
        }

        StartWave();
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
        if (waveConfigs == null || waveConfigs.Count == 0)
        {
            Debug.LogError("WaveConfig не инициализирован или пустой.");
            return;
        }

        if (!spawningWave)
        {
            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        spawningWave = true;
        waveNumber++;

        if (waveConfigs != null && waveConfigs.ContainsKey(waveNumber))
        {
            WaveConfig currentWave = waveConfigs[waveNumber];
            waveDuration = currentWave.waveDuration;  // Время волны
            timeStartedWave = Time.time;

            // Создаем список всех мобов для спавна
            List<EnemySpawn> allEnemiesToSpawn = new List<EnemySpawn>(currentWave.enemiesToSpawn);

            // Рассчитываем время для спавна мобов (равномерно за (время волны - 7 секунд))
            float spawnTimeLimit = waveDuration - 7f;  // Мобы спавнятся за (время волны - 7 секунд)
            float totalEnemies = allEnemiesToSpawn.Sum(enemySpawn => enemySpawn.count);  // Суммарное количество мобов
            float spawnInterval = spawnTimeLimit / totalEnemies;  // Интервал спавна

            float currentTime = 0f;

            // Спавним мобов равномерно в течение времени спавна
            while (currentTime < spawnTimeLimit)
            {
                // Выбираем случайного моба для спавна
                EnemySpawn randomEnemySpawn = GetRandomEnemySpawn(allEnemiesToSpawn);

                if (randomEnemySpawn != null && randomEnemySpawn.count > 0)
                {
                    Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                    GameObject enemyPrefab = randomEnemySpawn.enemyPrefab;

                    GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                    AddEnemy(enemy);

                    // Уменьшаем количество оставшихся мобов этого типа
                    randomEnemySpawn.count--;

                    // Ожидаем следующий спавн
                    yield return new WaitForSeconds(spawnInterval);

                    currentTime = Time.time - timeStartedWave;  // Обновляем текущее время
                }

                // Удаляем мобов с count = 0 из списка для оптимизации
                allEnemiesToSpawn.RemoveAll(enemySpawn => enemySpawn.count <= 0);
            }

            // Прекращаем спавн мобов, но продолжаем отсчет времени волны
            while (Time.time - timeStartedWave < waveDuration)
            {
                yield return null;  // Ждем окончания волны
            }

            // Уничтожаем оставшихся мобов после завершения волны
            RemoveRemainingEnemies();

            shop.OpenShop();
        }

        else
        {
            Debug.LogError("WaveConfig для waveNumber " + waveNumber + " не найден или waveConfigs не инициализировано.");
        }


        spawningWave = false;

        StartCoroutine(StartNextWave());
    }

    // Функция для случайного выбора моба
    private EnemySpawn GetRandomEnemySpawn(List<EnemySpawn> enemiesToSpawn)
    {
        if (enemiesToSpawn.Count > 0)
        {
            int randomIndex = Random.Range(0, enemiesToSpawn.Count);
            return enemiesToSpawn[randomIndex];
        }

        return null;
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