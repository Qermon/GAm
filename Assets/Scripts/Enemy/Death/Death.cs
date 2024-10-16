using System.Collections;
using UnityEngine;

public class Death : Enemy // ���������, ��� MobDeath ��������� �� Enemy
{
    public float moveSpeed = 0.8f; // �������� ������������
    public float spawnInterval = 7f; // �������� ����� ��������
    public int mobsToSpawn = 5; // ���������� ��������� �����
    public GameObject miniMobPrefab; // ������ ����-����
    private bool isSpawning = false; // ����, ����������� �� �����
    private Animator animator; // ��������

    public GameObject enemyPrefab;
    public Transform[] summonPoints;

    protected override void Start() // ���������� override
    {
        base.Start(); // �������� ����� Start() �������� ������
        animator = GetComponent<Animator>();

        // ��������� �������� ��� ������ �����
        StartCoroutine(SpawnMobRoutine());
    }

    protected override void Update() // ���������� override
    {
        base.Update(); // �������� ����� Update() �������� ������
        if (!isSpawning) // ���������, �� ������� �� �����
        {
            MoveMob(); // ������� ����, ���� �� �� �������
        }
    }

    void MoveMob()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            FlipSprite(direction); // ����� ��� �������� �������
        }
    }

    IEnumerator SpawnMobRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval - 1f); // ���� 6 ������ ����� ������� ������
            isSpawning = true; // ������������� ���� ������
            animator.SetBool("IsChasing", false); // ��������� �������� �������������
            animator.SetBool("IsIdle", true); // �������� �������� Idle

            // �������� �� 1 ������� ����� �������, ����� ��� ����� � Idle ��������
            yield return new WaitForSeconds(1f); // ���� 1 �������

            for (int i = 0; i < mobsToSpawn; i++)
            {
                Vector2 spawnPosition = GetRandomSpawnPosition(); // �������� ��������� ������� ������
                SpawnMiniMob(spawnPosition); // ������� ����-����
                yield return new WaitForSeconds(0.5f); // ��������� �������� ����� ������� �����
            }

            // ����� ���������� ������ ���������� ���� � ����������� ���������
            animator.SetBool("IsIdle", false); // ��������� Idle ����� ������
            isSpawning = false; // ��������� �����
        }
    }


    Vector2 GetRandomSpawnPosition()
    {
        Vector2 randomDirection;
        Vector2 spawnPosition;

        // ���� ���������� ����� ��� ������
        do
        {
            randomDirection = Random.insideUnitCircle.normalized; // ��������� �����������
            float distance = Random.Range(0f, 1.5f); // ��������� ���������� � ������� 1.5
            spawnPosition = (Vector2)transform.position + randomDirection * distance; // ������� ������
        }
        while (Physics2D.OverlapCircle(spawnPosition, 0.1f, LayerMask.GetMask("Wall"))); // ���������, �� �������� �� � ������� "Wall"

        return spawnPosition; // ���������� ���������� ������� ������
    }


    bool IsInsideTrigger(Vector2 position)
    {
        // ��������, ��������� �� ������� � �������-���������� � ����� Wall
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return true; // ������� ������ �������� � ����� Wall
            }
        }
        return false; // ������� �� ������ ��������
    }

    void SpawnMiniMob(Vector2 spawnPosition)
    {
        // ����� ����-���� � ��������� �������
        Instantiate(miniMobPrefab, spawnPosition, Quaternion.identity);
    }



    // �����, ������� �������� �� ������ �����
    public void SummonEnemies()
    {
        foreach (Transform summonPoint in summonPoints)
        {
            // ������ ����
            GameObject summonedEnemy = Instantiate(enemyPrefab, summonPoint.position, summonPoint.rotation);

            // ��������� ���� � ������ �������� ������ ����� WaveManager
            FindObjectOfType<WaveManager>().AddEnemy(summonedEnemy);
        }
    }
}