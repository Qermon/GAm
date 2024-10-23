using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveManager : MonoBehaviour
{


    public Shop shop;




    public Shop shop;

    public GameObject[] deathMobPrefabs;  // ������� ����� ������
    public GameObject[] deathPrefabs;
    public GameObject[] batPrefabs;       // ������� ����� ������� �����
    public GameObject[] wizardPrefabs;    // ������� ����� �����
    public GameObject[] skeletonPrefabs;  // ������� ��������
    public GameObject[] archerPrefabs;    // ������� ��������
    public GameObject[] boomPrefabs;      // ������� ����������������
    public GameObject[] healerPrefabs;    // ������� �������
    public GameObject[] buffMobPrefabs;   // ������� ��������
    public Transform[] spawnPoints;       // ����� ������
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
        InitializeWaves(); // ��������, ��� ������������� ���������� �� ������ ����

        if (waveConfigs == null || waveConfigs.Count == 0)
        {
            Debug.LogError("������ ������������� ����: waveConfigs �� ��������������� ��� ������.");
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
            Debug.LogError("WaveConfig �� ��������������� ��� ������.");
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
            waveDuration = currentWave.waveDuration;  // ����� �����
            timeStartedWave = Time.time;

            // ������� ������ ���� ����� ��� ������
            List<EnemySpawn> allEnemiesToSpawn = new List<EnemySpawn>(currentWave.enemiesToSpawn);

            // ������������ ����� ��� ������ ����� (���������� �� (����� ����� - 7 ������))
            float spawnTimeLimit = waveDuration - 7f;  // ���� ��������� �� (����� ����� - 7 ������)
            float totalEnemies = allEnemiesToSpawn.Sum(enemySpawn => enemySpawn.count);  // ��������� ���������� �����
            float spawnInterval = spawnTimeLimit / totalEnemies;  // �������� ������

            float currentTime = 0f;

            // ������� ����� ���������� � ������� ������� ������
            while (currentTime < spawnTimeLimit)
            {
                // �������� ���������� ���� ��� ������
                EnemySpawn randomEnemySpawn = GetRandomEnemySpawn(allEnemiesToSpawn);

                if (randomEnemySpawn != null && randomEnemySpawn.count > 0)
                {
                    Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                    GameObject enemyPrefab = randomEnemySpawn.enemyPrefab;

                    GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                    AddEnemy(enemy);

                    // ��������� ���������� ���������� ����� ����� ����
                    randomEnemySpawn.count--;

                    // ������� ��������� �����
                    yield return new WaitForSeconds(spawnInterval);

                    currentTime = Time.time - timeStartedWave;  // ��������� ������� �����
                }

                // ������� ����� � count = 0 �� ������ ��� �����������
                allEnemiesToSpawn.RemoveAll(enemySpawn => enemySpawn.count <= 0);
            }

            // ���������� ����� �����, �� ���������� ������ ������� �����
            while (Time.time - timeStartedWave < waveDuration)
            {
                yield return null;  // ���� ��������� �����
            }

            // ���������� ���������� ����� ����� ���������� �����
            RemoveRemainingEnemies();

            shop.OpenShop();
        }

        else
        {
            Debug.LogError("WaveConfig ��� waveNumber " + waveNumber + " �� ������ ��� waveConfigs �� ����������������.");
        }


        spawningWave = false;

        StartCoroutine(StartNextWave());
    }

    // ������� ��� ���������� ������ ����
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

        // ����� 1
        waveConfigs.Add(1, new WaveConfig(20f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(50 + 1 * 1.5f)) // 50 �������
    }));

        // ����� 2
        waveConfigs.Add(2, new WaveConfig(25f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(50 + 2 * 1.5f)), // 50 �������
        new EnemySpawn(batPrefabs[0], 10) // 10 ������� �����
    }));

        // ����� 3
        waveConfigs.Add(3, new WaveConfig(30f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(55 + 3 * 1.5f)), // 55 �������
        new EnemySpawn(batPrefabs[0], 20) // 20 ������� �����
    }));

        // ����� 4
        waveConfigs.Add(4, new WaveConfig(35f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(55 + 4 * 1.5f)), // 55 �������
        new EnemySpawn(batPrefabs[0], 20), // 20 ������� �����
        new EnemySpawn(wizardPrefabs[0], 5) // 5 �����
    }));

        // ����� 5
        waveConfigs.Add(5, new WaveConfig(40f, new List<EnemySpawn>
    {
        new EnemySpawn(batPrefabs[0], Mathf.FloorToInt(50 + 5 * 1.5f)) // 50 ������� �����
    }));

        // ����� 6
        waveConfigs.Add(6, new WaveConfig(45f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0], Mathf.FloorToInt(60 + 6 * 1.5f)), // 60 �������
        new EnemySpawn(deathPrefabs[0], 6), // 6 ����� ������
        new EnemySpawn(wizardPrefabs[0], 5), // 5 �����
        new EnemySpawn(batPrefabs[0], 20) // 20 ������� �����
    }));

        // ����� 7
        waveConfigs.Add(7, new WaveConfig(50f, new List<EnemySpawn>
    {
        new EnemySpawn(batPrefabs[0], Mathf.FloorToInt(50 + 7 * 1.5f)), // 50 ������� �����
        new EnemySpawn(archerPrefabs[0], 5), // 5 ��������
        new EnemySpawn(wizardPrefabs[0], 5), // 5 �����
        new EnemySpawn(boomPrefabs[0], 5) // 5 ����������������
    }));

        // ����� 8
        waveConfigs.Add(8, new WaveConfig(55f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(20 + 8 * 1.5f)), // 20 �������
        new EnemySpawn(skeletonPrefabs[0], 10), // 10 ��������
        new EnemySpawn(boomPrefabs[0], 5), // 5 ����������������
        new EnemySpawn(healerPrefabs[0], 2), // 2 ������
        new EnemySpawn(deathPrefabs[0], 15), // 5 ����� ������
        new EnemySpawn(wizardPrefabs[0], 5), // 5 �����
        new EnemySpawn(archerPrefabs[0], 2) // 2 �������
    }));

        // ����� 9
        waveConfigs.Add(9, new WaveConfig(60f, new List<EnemySpawn>
    {
        new EnemySpawn(deathMobPrefabs[0],  Mathf.FloorToInt(20 + 9 * 1.5f)), // 20 �������
        new EnemySpawn(skeletonPrefabs[0], 10), // 10 ��������
        new EnemySpawn(boomPrefabs[0], 5), // 5 ����������������
        new EnemySpawn(healerPrefabs[0], 2), // 2 ������
        new EnemySpawn(deathPrefabs[0], 15), // 5 ����� ������
        new EnemySpawn(wizardPrefabs[0], 5), // 5 �����
        new EnemySpawn(archerPrefabs[0], 2), // 2 �������
        new EnemySpawn(buffMobPrefabs[0], 2) // 2 �������
    }));

        // ����� 10
        waveConfigs.Add(10, new WaveConfig(60f, new List<EnemySpawn>
    {
        new EnemySpawn(batPrefabs[0], Mathf.FloorToInt(100 + 10 * 1.5f)), // 100 ������� �����
        new EnemySpawn(wizardPrefabs[0], 5), // 5 �����
        new EnemySpawn(boomPrefabs[0], 5) // 5 ����������������
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