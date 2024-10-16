using System.Collections;
using UnityEngine;

public class ArcherEnemy : Enemy
{
    public GameObject arrowPrefab; // ������ ������
    public Transform shootPoint; // �����, ������ ����������� ������
    public float shootCooldown = 5f; // ����� ����� ������� ���������
    public int arrowsPerShot = 3; // ���������� ����� �� ��� (��� �������������)

    private bool isShooting = false; // ���� ��� ��������, �������� �� ������
    private float shootTimer = 0f; // ������ ��� ������� ������� �� ��������� �����
    private Vector2 randomDirection; // ����������� ��� ���������� ��������
    private float moveDuration = 2f; // ����� �������� � ����� �����������
    private float changeDirectionTime; // ������ ��� ����� �����������
    protected Rigidbody2D rb; // �������� � private �� protected

    private Animator animator; // ��� ��������
    private float originalMass; // ��� �������� ������������ �����

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>(); // �������������� Rigidbody2D
        animator = GetComponent<Animator>(); // �������� ��������� Animator
        randomDirection = GetRandomDirection(); // ��������� ���������� �����������
        originalMass = rb.mass; // ��������� ������������ �����
        changeDirectionTime = Time.time + moveDuration; // ������������� ��������� ����� ��� ����� �����������
    }

    protected override void Update()
    {
        if (isDead) return;

        shootTimer -= Time.deltaTime;

        if (!isShooting && shootTimer <= 0f)
        {
            StartShooting();
            shootTimer = shootCooldown; // ���������� ������ �� ��������� ��������
        }

        if (!isShooting)
        {
            MoveInRandomDirection();
        }
    }

    private void StartShooting()
    {
        if (player == null) return; // ���������, ��� ����� ����������

        isShooting = true;
        rb.velocity = Vector2.zero; // ������������� ������� ��� ��������

        // �������� ����� �� 100
        rb.mass = 100f;

        // ������������ ������� � ������
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        FlipSprite(directionToPlayer); // ������� �� ��� X

        // �������� ��������� �������� �����
        int randomAnimation = Random.Range(0, 2); // 0 ��� 1
        if (randomAnimation == 0)
        {
            animator.SetBool("Shot1", true); // ����������� ������ ��������
        }
        else
        {
            animator.SetBool("Shot2", true); // ����������� ������ ��������
        }
    }

    // ���� ����� ���������� ����� ������� � ��������
    public void ShootArrow()
    {
        if (arrowPrefab != null && shootPoint != null)
        {
            Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
            Debug.Log($"{gameObject.name} �������� ������.");
        }
    }

    // ���� ����� ���������� � ����� �������� ��� �������� � �������� ��������
    public void EndShooting()
    {
        animator.SetBool("Shot1", false);
        animator.SetBool("Shot2", false);
        isShooting = false; // ����������� �������� � ���������� ��������

        // ��������������� ������������ �����
        rb.mass = originalMass;
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
