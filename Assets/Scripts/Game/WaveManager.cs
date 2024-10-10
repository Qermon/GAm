using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;        // ������ �����
    public GameObject crossPrefab;        // ������ �������� ��������
    public Transform[] spawnPoints;       // ����� ��������� ������
    public float timeBetweenWaves = 5f;   // ����� ����� �������
    public float waveDuration = 30f;      // ����� ����������������� �����
    private int waveNumber = 0;           // ����� ������� �����
    public int maxWaves = 50;             // ������������ ���������� ����
    public int maxActiveEnemies = 50;     // ������������ ���������� �������� ������ ������������

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
        // ���� ����� �������
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
        int enemiesToSpawn = Mathf.FloorToInt(100 + waveNumber * 1.5f);
        enemiesToSpawn = Mathf.Min(enemiesToSpawn, maxActiveEnemies); // ��������, ��� ���������� ������ �� ��������� �����

        // ������������ ����� ����� �������� ������
        float spawnInterval = waveDuration / enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // ��������� ���������� �������� ������
            while (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxActiveEnemies)
            {
                yield return null; // ����, ���� ���������� �������� ������ ����������
            }

            // ��������� ����� ���������
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // ������� ������� ����� �����
            GameObject cross = Instantiate(crossPrefab, spawnPoint.position, Quaternion.identity);
            Destroy(cross, 1f); // ���������� ������� ����� 1 �������

            // ��������� �������� ����� ������� ����, ����� ������� ����� ������������
            yield return new WaitForSecondsRealtime(0.3f);

            // ����� �����
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // �������� ����� ��������� ������� �����
            yield return new WaitForSecondsRealtime(spawnInterval - 0.3f); // ��������� ����� ��������
        }

        // ���� ����� ����������������� �����, ������ ��� ������� ���������� ������
        yield return new WaitForSecondsRealtime(waveDuration - (enemiesToSpawn * spawnInterval));

        // �������� ���� ���������� ������
        RemoveRemainingEnemies();

        spawningWave = false; // ��������� ������ ����� �����
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
        return waveNumber; // ���������� ������� ����� �����
    }

    public float GetTimeUntilNextWave()
    {
        if (spawningWave)
        {
            float timePassed = Time.time - timeStartedWave; // ����� � ������ �����
            float timeUntilNext = (waveDuration) - timePassed; // �������� ������� �� ����� �����

          

            if (timeUntilNext <= 0)
            {
                RemoveRemainingEnemies(); // �������� ���� ���������� ������
                StartCoroutine(StartNextWave()); // ��������� ����� ����� ����� ��������
                return 0; // ���������� 0, ���� ����� �� ��������� ����� �����������
            }

            return timeUntilNext; // ���������� ���������� �����
        }
        return -1; // ���� ����� �� ���������, ���������� -1
    }
}
