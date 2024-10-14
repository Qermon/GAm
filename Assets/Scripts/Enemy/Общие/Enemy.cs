using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // �������� �������������� �����
    public int maxHealth = 100;
    protected int currentHealth;
    public float enemyMoveSpeed = 2f;
    public int damage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    protected bool isDead = false;

    protected Transform player; // ������ �� ������
    private float attackTimer = 0f; // ���������� ������ ��� �������� ����

    // ��������� ���� ��� ����� � �����
    public GameObject experienceItemPrefab;
    public int experienceAmount = 20;
    public GameObject[] bloodPrefabs; // ������ ������� �����

    // ������ �� BloodManager
    public BloodManager bloodManager;

    protected virtual void Start()
    {
        // ������������� �������� � ���������� ������
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        if (isDead || player == null) return;

        // �������� � ������
        MoveTowardsPlayer();

        // �������� �� ����������� �����
        attackTimer -= Time.deltaTime;
        if (Vector2.Distance(transform.position, player.position) <= attackRange && attackTimer <= 0f)
        {
            AttackPlayer();
        }
    }

    // ����� ��� �������� � ������
    protected virtual void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemyMoveSpeed * Time.deltaTime);
            FlipSprite(direction); // ����� ��� �������� �������
        }
    }

    // ����� ��� ����� ������
    protected virtual void AttackPlayer()
    {
        attackTimer = attackCooldown;
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    // ����� ��� ��������� �����
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    // ����� ��� ������
    protected virtual void Die()
    {
        isDead = true;
        SpawnExperience();
        SpawnBlood();
        Destroy(gameObject);
    }

    // ����� �������� �����
    protected virtual void SpawnExperience()
    {
        if (experienceItemPrefab != null)
        {
            Instantiate(experienceItemPrefab, transform.position, Quaternion.identity);
        }
    }

    // ����� �����
    protected virtual void SpawnBlood()
    {
        if (bloodPrefabs.Length > 0) // ��������, ���� �� �������� ����� � �������
        {
            // ��������� ����� �������� ����� �� �������
            int randomIndex = Random.Range(0, bloodPrefabs.Length);
            GameObject blood = Instantiate(bloodPrefabs[randomIndex], transform.position, Quaternion.identity);
            blood.tag = "Blood"; // ������������� ��� ��� ������� �����

            // ���� ������ �� BloodManager ������, ��������� �������� ��� �������� �����
            if (bloodManager != null)
            {
                StartCoroutine(bloodManager.RemoveBlood(blood)); // ��������� �������� ��� �������� �����
            }
            else
            {
                Debug.LogWarning("BloodManager �� ����� � Enemy.");
            }
        }
    }


    // ����� ��� �������� ������� ���� � ������� ������
    protected virtual void FlipSprite(Vector2 direction)
    {
        Vector3 localScale = transform.localScale;
        if (direction.x > 0)
        {
            localScale.x = Mathf.Abs(localScale.x); // ��������� ������
        }
        else if (direction.x < 0)
        {
            localScale.x = -Mathf.Abs(localScale.x); // ��������� �����
        }
        transform.localScale = localScale;
    }
}
