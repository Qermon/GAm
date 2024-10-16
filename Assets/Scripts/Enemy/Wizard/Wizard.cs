using System.Collections;
using UnityEngine;

public class Wizard : Enemy
{
    public Animator animator; // ������ �� ��������� Animator
    public GameObject projectilePrefab; // ������ �������
    public Transform firePoint; // �����, ������ ����������� ������

    public float safeDistance = 5f; // ���������, �� ������� ��� �������� �� ������

    private float lastAttackTime = 0f; // ����� ��������� �����
    private bool isAttacking = false; // ���� ��� �������� ��������� �����

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>(); // �������� ��������� Animator
    }

    protected override void Update()
    {
        base.Update();

        if (isDead || player == null) return;

        // ��������� ��������� �� ������
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > safeDistance)
        {
            MoveAwayFromPlayer();
            animator.SetFloat("moveSpeed", enemyMoveSpeed); // ������ �������� ����
        }
        else
        {
            animator.SetFloat("moveSpeed", 0f); // ��������� �������� ��������

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

    // ��������� ��������
    private void StopMoving()
    {
        enemyMoveSpeed = 0f; // ������ ��������� ����
        animator.SetFloat("moveSpeed", 0f); // ��������� �������� ��������
    }

    // ����� ��� �������� � ������� �� ������
    protected void MoveAwayFromPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (transform.position - player.position).normalized; // ��������� �����������
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)direction, enemyMoveSpeed * Time.deltaTime);
            FlipSprite(direction); // ����� ��� �������� �������
        }
    }
}
