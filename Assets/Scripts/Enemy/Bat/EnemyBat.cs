using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBat : MonoBehaviour
{
    private int health = 100;
    public int currentHealth;
    public Transform player; // ������ �� ������ ������
    public float moveSpeed; // �������� ������������ ����
    public float attackRange = 1.5f;
    public int damage = 10;
    public float attackSpeed = 1f; // �������� ����� � ������ � �������
    private float attackCooldown; // �����, ����� ���� ����� ����� ���������

    public GameObject enemyPrefab; // ������ ������
    public Transform spawnPoint; // ����� ������

    // ����� ������ ��������, ������� ����������� ���� ������
    public GameObject experienceItemPrefab;
    public int experienceAmount = 20; // ���������� �����, ������� ���� �������

    public GameObject[] bloodPrefabs; // ������ ��� ���������� ������� �����
    private float bloodCooldown = 0.5f;
    private float lastBloodTime = 0f;

    


    void Start()
    {
        currentHealth = health;
        attackCooldown = 0f; // ���������� ���� ����� ���������
        player = FindObjectOfType<PlayerMovement>().transform; // ����� ������ ������
       
    }

    void Update()
    {
        if (player == null) return; // ���� ����� �� �����, �������

        // ���������� ����������� �� ������
        Vector2 moveDir = (player.position - transform.position).normalized;

        // ������� ���� � ������� ������
        FlipSprite(moveDir);

        // �������� ���� � ������� ������
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // ��������� ������ ��������
        attackCooldown -= Time.deltaTime;

        // ���������, ����� �� ���� ���������
        if (Vector2.Distance(transform.position, player.position) < attackRange && attackCooldown <= 0)
        {
            AttackPlayer();
        }
    }

    public static EnemyBat Spawn()
    {
        return new EnemyBat(); // ��������� ����� ��'��� Enemy
    }

    public void AttackPlayer()
    {
        // ������� ���� ������
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        // ������������� ����� �� �������
        attackCooldown = 1f / attackSpeed; // ��������, ���� �������� ����� 2, ������� ����� 0.5 �������
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (Time.time >= lastBloodTime + bloodCooldown)
        {
            SpawnBlood();
            lastBloodTime = Time.time;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void SpawnBlood()
    {
        if (bloodPrefabs.Length == 0) return;

        // �������� ��������� �������� �����
        GameObject randomBlood = bloodPrefabs[Random.Range(0, bloodPrefabs.Length)];

        // ������� �������� �����
        GameObject blood = Instantiate(randomBlood, transform.position, Quaternion.identity);

        // ������ �������� ��� �������� ������������ � �������� ����� 3 �������
        StartCoroutine(FadeAndDestroy(blood, 3f));
    }


    IEnumerator FadeAndDestroy(GameObject blood, float fadeDuration)
    {
        // �������� SpriteRenderer ��� ������� �����
        SpriteRenderer bloodRenderer = blood.GetComponent<SpriteRenderer>();

        // ���� SpriteRenderer �� ������, ������� ��������� � ��������� ����������
        if (bloodRenderer == null)
        {
            Debug.LogError("SpriteRenderer �� ������ �� ������� �����: " + blood.name);
            yield break;
        }

        // ������������ ���� �����
        Color originalColor = bloodRenderer.color;
        float timeElapsed = 0f;

        Debug.Log("������ ������������ ����� ��� �������: " + blood.name);

        // ������� ������������
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timeElapsed / fadeDuration); // �������� ������������ ��� �����-������
            bloodRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Debug.Log("������ ����� ����� ���������: " + blood.name);

        // �������� ������� ����� ������� ������������
        Destroy(blood);
    }





    void Die()
    {
        Debug.Log("Enemy died!");

        // ������� ������� ����� ����� ������ �����
        if (experienceItemPrefab != null)
        {
            Instantiate(experienceItemPrefab, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false); // ��� ���������� ������
    }

    void FlipSprite(Vector2 direction)
    {
        // �������� ������� �������
        Vector3 currentScale = transform.localScale;

        // ���� ����� ��������� ������, ���������� ���� ������ (�� ��� X), ���� ����� � �����
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
        }
    }

    public static EnemyBat Spawn(GameObject enemyPrefab, Transform spawnPoint)
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            GameObject enemyObject = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("����� ��������!");

            // ��������� ��������� Enemy � ������ ��'����
            return enemyObject.GetComponent<EnemyBat>();
        }
        else
        {
            Debug.LogError("������ ������ ��� ����� ������ �� ������!");
            return null; // ���� �� ������� �������� ������
        }
    }

    public bool IsAlive()
    {
        return currentHealth > 0; // ����� �����, ���� ������'� ����� 0
    }
  



}