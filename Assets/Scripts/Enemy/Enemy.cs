using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth;
    public Transform player; // ������ �� ������ ������
    public float moveSpeed; // �������� ������������ ����
    public float attackRange = 1.5f;
    public int damage = 10;
    public float attackSpeed = 1f; // �������� ����� � ������ � �������
    private float attackCooldown; // �����, ����� ���� ����� ����� ���������

    void Start()
    {
        currentHealth = maxHealth;
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

    void AttackPlayer()
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
        gameObject.SetActive(false);
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
}
