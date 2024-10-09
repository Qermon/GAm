using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
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

     public static Enemy Spawn()
    {
        return new Enemy(); // ��������� ����� ��'��� Enemy
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
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");
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
    public static Enemy Spawn(GameObject enemyPrefab, Transform spawnPoint)
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            GameObject enemyObject = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("����� ��������!");

            // ��������� ��������� Enemy � ������ ��'����
            return enemyObject.GetComponent<Enemy>();
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

