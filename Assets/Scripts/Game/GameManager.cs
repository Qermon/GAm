using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private int currentWave;
    private Player player;
    private List<Enemy> enemies;
    private bool isGameOver;

    public GameObject enemyPrefab; // ������ �����
    public Transform spawnPoint; // ����� ������

    public float waveDuration = 20f; // ������������ ������ ����� � ��������
    public int totalWaves = 1000;
    void Start()
    {
        currentWave = 1;
        enemies = new List<Enemy>();
        isGameOver = false;

        StartGame();
    }

    public void StartGame()
    {
        // �������� ������ ������ � �������� ��������� Player
        GameObject playerObject = new GameObject("Player");
        player = playerObject.AddComponent<Player>();
        // ������������� ������, ��������, ��������
        player.Initialize(); // ���������, ��� � ��� ���� ����� ������������� � ������ Player

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop() // Coroutine to handle waves and game updates
    {
        while (!isGameOver && currentWave <= totalWaves) // ��������� ������� ��� �������� ���������� ����
        {
            StartWave();
            yield return new WaitForSeconds(1); // Optional: wait a second before starting the next wave
        }

        if (currentWave > totalWaves)
        {
            Debug.Log("�� ���������! ��� ����� ��������.");
        }
    }


    private void StartWave()
    {
        int waveDuration = 20; // ������������ ����� � ��������
        Debug.Log($"Starting wave {currentWave}...");

        SpawnEnemies();
        StartCoroutine(WaveTimer(waveDuration));
    }

    private IEnumerator WaveTimer(int duration)
    {
        float startTime = Time.time; // ���������� ����� ������ �����
        while (Time.time - startTime < duration)
        {
            if (!player.IsAlive())
            {
                EndGame();
                yield break; // ������� �� ��������, ���� ����� �����
            }

            UpdateGameLogic();

            // ������������ ���������� �����
            float remainingTime = duration - (Time.time - startTime);
            Debug.Log($"���������� ����� �����: {remainingTime:F2} ������"); // ������� ���������� ����� � �������

            yield return null; // ���� �� ���������� �����
        }

        
        currentWave++; // ������� � ��������� �����
        Debug.Log($"����� {currentWave} ��������!"); // ������� ��������� � ���������� �����
    }


    private void SpawnEnemies()
    {
        int numEnemies = currentWave * 2; // ���������� ������ ������� �� �����
        for (int i = 0; i < numEnemies; i++)
        {
            Enemy enemy = Enemy.Spawn(enemyPrefab, spawnPoint);
            if (enemy != null)
            {
                enemies.Add(enemy); // �������� ����� � ������, ���� �� ������� ������
            }
        }
    }

    private void UpdateGameLogic()
    {
        // ������ ��� ���������� ��������� ����
        // ��������, �������� ������� ������ �� ������
        enemies.RemoveAll(enemy => !enemy.IsAlive());
    }

    private void EndGame()
    {
        isGameOver = true;
        Debug.Log("Game Over! You died.");
    }
}
