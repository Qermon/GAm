using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;        // ������ �����
    public GameObject crossPrefab;        // ������ �������� ��������
    public Transform[] spawnPoints;       // ����� ��������� ������
    public float timeBetweenWaves = 5f;   // ����� ����� �������
    public float waveDuration = 30f;      // ����� ����������������� �����
    private int waveNumber = 0;           // ����� ������� �����
    public int maxWaves = 1000;             // ������������ ���������� ����
    public int maxActiveEnemies = 1000;     // ������������ ���������� �������� ������ ������������

    private bool spawningWave = false;    // ���� ��� ������������ ������ �����
    private float timeStartedWave;         // ����� ������ �����

    private bool isWaveEnded = false; // ���� ��� ������������ ���������� �����

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

        // ������������� ����� ����������������� �����
        float waveDuration = 30f; // ������������� ����������������� ����� � 30 ������
        float spawnDuration = waveDuration - 5f; // �����, �� ������� ����� ���������� ���� ������

        // ����������� ���������� ������ � ����������� �� ������ �����
        int enemiesToSpawn = Mathf.FloorToInt(100 + waveNumber * 1.5f);
        enemiesToSpawn = Mathf.Min(enemiesToSpawn, maxActiveEnemies); // ��������, ��� ���������� ������ �� ��������� �����

        // ������������ ����� ����� ������� ������
        float spawnInterval = spawnDuration / enemiesToSpawn; // ����� �� ���������� ������

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

            // ��������� �������� ��� ������ �����
            StartCoroutine(SpawnEnemy(cross, spawnPoint));

            // �������� ����� ��������� ������� �����
            yield return new WaitForSecondsRealtime(spawnInterval); // ���������� ������������� ��������
        }

        // ���� ���������� 5 ������ ����� ������ ���� ������
        yield return new WaitForSecondsRealtime(5f);

        // �������� ���� ���������� ������
        RemoveRemainingEnemies();

        spawningWave = false; // ��������� ������ ����� �����
        StartCoroutine(StartNextWave()); // ��������� ��������� �����
    }

    IEnumerator SpawnEnemy(GameObject cross, Transform spawnPoint)
    {
        // ��������� ��������, ����� ������� ����� ������������
        yield return new WaitForSecondsRealtime(0.5f); // �������� 0.5 ������

        // ���������, ��� �� ��� � �������� ������
        if (!spawningWave) // ���� ����� ���������, �������
        {
            Destroy(cross);
            yield break; // ��������� ��������
        }

        // ����� �����
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // ���������� ������� ����� ����� ������ �����
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
        return waveNumber; // ���������� ������� ����� �����
    }

    public float GetTimeUntilNextWave()
    {
        if (spawningWave)
        {
            float timePassed = Time.time - timeStartedWave; // ����� � ������ �����
            float timeUntilNext = waveDuration - timePassed; // �������� ������� �� ����� �����

            // ���� ����� �� ����� ����� <= 0
            if (timeUntilNext <= 0)
            {
                timeUntilNext = 0; // ������������� ����� � 0, ����� ��� �� ������� � ������������� ��������
                RemoveRemainingEnemies(); // �������� ���� ���������� ������

                // ������������� ����, ��� ����� ���������
                isWaveEnded = true;

                // ��������� ����� �����, ������ ���� ��� ��� �� ��������
                if (!spawningWave)
                {
                    StartCoroutine(StartNextWave()); // ��������� ����� ����� ����� ��������
                }
            }

            return timeUntilNext; // ���������� ���������� �����
        }

        // ���� ����� ���������, ���������� -1
        return isWaveEnded ? -1 : -1;
    }




}