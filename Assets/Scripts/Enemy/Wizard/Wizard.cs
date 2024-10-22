using System.Collections;
using UnityEngine;

public class Wizard : Enemy
{
    public Animator animator; // ������ �� ��������� Animator
    public GameObject projectilePrefab; // ������ �������
    public Transform firePoint; // �����, ������ ����������� ������

    public float safeDistance = 5f; // ���������, �� ������� ��� �������� �� ������
    public float moveDuration = 2f; // ����� �������� � ����� �����������
    private float lastAttackTime = 0f; // ����� ��������� �����
    private bool isAttacking = false; // ���� ��� �������� ��������� �����
    private Vector2 randomDirection; // ����������� ��� ���������� ��������
    private float changeDirectionTime; // ������ ��� ����� �����������
    private Rigidbody2D rb; // Rigidbody ��� ���������� ���������
    private float originalMass; // �������� ����� ����

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>(); // �������� ��������� Animator
        rb = GetComponent<Rigidbody2D>(); // �������������� Rigidbody2D
        originalMass = rb.mass; // ��������� ������������ �����
        randomDirection = GetRandomDirection(); // ��������� ���������� �����������
        changeDirectionTime = Time.time + moveDuration; // ������������� ��������� ����� ��� ����� �����������
    }

    protected override void Update()
    {
        base.Update();

        if (isDead || player == null) return;

        // ��������� ��������� �� ������
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // ���� ����� ������, ��������� � ��������� �����������
        if (distanceToPlayer > safeDistance && !isAttacking)
        {
            MoveInRandomDirection();
            animator.SetFloat("moveSpeed", enemyMoveSpeed); // �������� ��������
        }
        else
        {
            // ������������� �������� � �������
            animator.SetFloat("moveSpeed", 0f); // ������������� �������� ��������

            // ������� ��� �����
            if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
            {
                StartAttack(); // ������ �����
            }
        }
    }

    private void StartAttack()
    {
        lastAttackTime = Time.time; // ��������� ����� ��������� �����
        isAttacking = true; // ������������� ���� �����
        animator.SetBool("isAttacking", true); // ������ �������� �����

        // ������������ ���� � ������ ����� ������
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        FlipSprite(directionToPlayer); // ������� ���� �� ����������� � ������
    }

    // �����, ���������� �������� �������� ��� ������� �������
    public void ShootProjectile()
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity); // ���������� firePoint
        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        Vector2 directionToPlayer = (player.transform.position - firePoint.position).normalized; // ���������� firePoint ��� �����������
        projectile.SetDirection(directionToPlayer); // ������������� ����������� �������
    }

    // �����, ���������� � ����� �������� �����
    public void FinishAttack()
    {
        isAttacking = false; // ���������� ���� ����� ����� ���������� ��������
        animator.SetBool("isAttacking", false); // ��������� �������� �����
    }

    private void MoveInRandomDirection()
    {
        rb.velocity = randomDirection * enemyMoveSpeed;

        // ������������ ���� � ������� ��������
        if (rb.velocity.magnitude > 0.1f) // ���������, �������� �� ���
        {
            FlipSprite(randomDirection);
        }

        // ���������, ������ �� ����� ����� �����������
        if (Time.time >= changeDirectionTime)
        {
            randomDirection = GetRandomDirection();
            changeDirectionTime = Time.time + moveDuration; // ������������� ����� ����� ��� ����� �����������
        }
    }

    private Vector2 GetRandomDirection()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    // ����� ��� �������� ������� � ������� ��������
    protected override void FlipSprite(Vector2 direction)
    {
        if (direction.x > 0)
        {
            // ������������ ������
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < 0)
        {
            // ������������ �����
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
