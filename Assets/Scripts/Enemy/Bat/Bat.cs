using System.Collections;
using UnityEngine;

public class Bat : MonoBehaviour
{
    // ��������� ��������
    private int health = 100;
    public int currentHealth;

    // ��������� ������������ � �����
    public Transform player;  // ������ �� ������
    public float moveSpeed = 6f;  // �������� ����������� (�������)
    private float currentMoveSpeed; // ������� �������� ����������� (����� ����������)
    private Vector2 targetDirection;  // ����������� �� ������

    // ��������� �����
    public float attackRange = 1.5f;  // ��������� �����
    public int damage = 10;  // ���� ������
    public float attackSpeed = 1f;  // �������� ����� (������ � �������)
    private float attackCooldown = 0f;  // ������ �������� �����

    // ����� ����� � ����
    public GameObject experienceItemPrefab;  // ������ �������� �����
    public int experienceAmount = 20;  // ���������� �����

    // ������� �����
    public GameObject[] bloodPrefabs;  // ������� �������� ������
    private float bloodCooldown = 0.5f;  // ������ ��� ������ �����
    private float lastBloodTime = 0f;  // ��������� ���, ����� ��������� �����

    // �������� ����� ������
    public float spawnDelay = 1.3f;  // �������� � ��������

    void Start()
    {
        // �������������� ������� ��������
        currentHealth = health;

        // ������� ������ � �����
        player = FindObjectOfType<PlayerMovement>().transform;

        // ��������� ��������� �������� ����� ������
        StartCoroutine(SlowMovementAfterSpawn());
    }

    void Update()
    {
        if (player == null) return;  // ���� ����� �� ������, ������ �� ������

        // ���� ��� �� ����������, �� ���������� ��� � ������� ������
        transform.position += (Vector3)targetDirection * currentMoveSpeed * Time.deltaTime;

        // ���� ���� � ������� �����, ���������, ����� �� ���������
        if (Vector2.Distance(transform.position, player.position) < attackRange && attackCooldown <= 0)
        {
            AttackPlayer();
        }

        // ��������� ������ �������� �����
        attackCooldown -= Time.deltaTime;
    }

    // ��������� ����������� � ������
    private void UpdateTargetDirection()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        // ������������ ���� � ������� ������
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // ������������� ����������� ��� ��������
        targetDirection = directionToPlayer;
    }

    // ������� ��� ���������� �������� ����� ������
    IEnumerator SlowMovementAfterSpawn()
    {
        // ������������� ��������� ��������� ��������
        float slowSpeed = moveSpeed * 0.25f;
        currentMoveSpeed = slowSpeed;

        // ��������� �������� � ������ � ������� 1.3 ������
        float elapsedTime = 0f;
        while (elapsedTime < spawnDelay)
        {
            UpdateTargetDirection();  // ��������� ����������� �� ������ ������ ����
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ����� 1.3 ������ ��������� �� ���������� ��������
        currentMoveSpeed = moveSpeed;
    }

    // ������������ ������������ � ��������� (��������, ������)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            StartCoroutine(StopAndTurn());
        }
    }

    // ������� ��� ��������� ����, ��� ��������� � ���������� �������� � ������
    IEnumerator StopAndTurn()
    {
        // ������������� ����
        float previousSpeed = currentMoveSpeed; // ��������� ������� ��������
        currentMoveSpeed = 0f;

        // ��������� ����������� �� ������
        UpdateTargetDirection();

        // �������������� � ������
        float turnDuration = 0.5f; // ����� �� �������
        float timeElapsed = 0f;

        while (timeElapsed < turnDuration)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // ����� �������� ��������������� ��������
        currentMoveSpeed = previousSpeed;
    }

    // ����� ������
    public void AttackPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        // ������������� ������� �����
        attackCooldown = 1f / attackSpeed;
    }

    // ��������� ����� �����
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // ����� ����� ��� ��������� �����
        if (Time.time >= lastBloodTime + bloodCooldown)
        {
            SpawnBlood();
            lastBloodTime = Time.time;
        }

        // ���� �������� ����������� � ��� �������
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ����� ������� �����
    void SpawnBlood()
    {
        if (bloodPrefabs.Length == 0) return;

        GameObject randomBlood = bloodPrefabs[Random.Range(0, bloodPrefabs.Length)];
        GameObject blood = Instantiate(randomBlood, transform.position, Quaternion.identity);
        StartCoroutine(FadeAndDestroy(blood, 3f));
    }

    // ������� ������������ � ����������� ������� �����
    IEnumerator FadeAndDestroy(GameObject blood, float fadeDuration)
    {
        SpriteRenderer bloodRenderer = blood.GetComponent<SpriteRenderer>();
        if (bloodRenderer == null) yield break;

        Color originalColor = bloodRenderer.color;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timeElapsed / fadeDuration);
            bloodRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(blood);
    }

    // �������� ����
    void Die()
    {
        Debug.Log("Bat died!");

        // ����� �������� �����
        if (experienceItemPrefab != null)
        {
            Instantiate(experienceItemPrefab, transform.position, Quaternion.identity);
        }

        // ��������� ������
        gameObject.SetActive(false);
    }

    // ��������������� ����� ��� �������� ��� �� ���
    public bool IsAlive()
    {
        return currentHealth > 0;
    }

}
