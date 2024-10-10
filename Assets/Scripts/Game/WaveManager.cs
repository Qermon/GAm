using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;        // ������ �����
    public Transform[] spawnPoints;       // ����� ��������� ������
    public float timeBetweenWaves = 5f;   // ����� ����� �������
    public float waveDuration = 30f;      // ����� ����������������� �����
    private int waveNumber = 0;           // ����� ������� �����
    public int maxWaves = 50;             // ������������ ���������� ����

    private bool spawningWave = false;    // ���� ��� ������������ ������ �����
    private float timeStartedWave;         // ����� ������ �����

    void Update()
    {
        // ��������� ��������� �����, ���� ������ ��� �����
        if (!spawningWave && GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && waveNumber < maxWaves)
        {
            StartCoroutine(StartNextWave());
        }
    }

    private IEnumerator StartNextWave()
    {
        // ���� 10 ������ ����� ���������� ���������� �����
        yield return new WaitForSeconds(timeBetweenWaves);

        StartWave(); // ��������� ����� �����
    }

    public void StartWave()
    {
        if (!spawningWave) // ���������, �� �������� �� �����
        {
            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        spawningWave = true;
        waveNumber++;

        // ��������� ������� ������ �����
        timeStartedWave = Time.time;

        // ����������� ���������� ������ � ����������� �� ������ �����
        int enemiesToSpawn = Mathf.FloorToInt(5 + waveNumber * 1.5f);

        // ����� ������
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f); // �������� ����� �������� ������
        }

        // ���� ����� ����������������� �����, ������ ��� ������� ���������� ������
        yield return new WaitForSeconds(waveDuration);

        // �������� ���� ���������� ������
        RemoveRemainingEnemies();

        spawningWave = false; // ��������� ������ ����� �����
    }

    // ������� ������ �����
    void SpawnEnemy()
    {
        // ��������� ����� ���������
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

    // ������ ������ WaveManager
    public int GetWaveNumber()
    {
        return waveNumber; // ���������� ������� ����� �����
    }

   
    public float GetTimeUntilNextWave()
    {
        if (spawningWave)
        {
            // ���������� ���������� ����� �� ����� �����
            float timePassed = Time.time - timeStartedWave; // ����� � ������ �����
            float timeUntilNext = (waveDuration) - timePassed; // �������� ������� �� ����� �����

            // ����������� ��� �������
            Debug.Log("Time until next wave: " + timeUntilNext);

            // ���� ����� �����������, ���������� 0
            if (timeUntilNext <= 0)
            {
                // �������� ���� ���������� ������
                RemoveRemainingEnemies();

                // ��������� ����� ����� ����� ��������
                StartCoroutine(StartNextWave());

                return 0; // ���������� 0, ���� ����� �� ��������� ����� �����������
            }

            return timeUntilNext; // ���������� ���������� �����
        }
        return -1; // ���� ����� �� ���������, ���������� -1
    }
}