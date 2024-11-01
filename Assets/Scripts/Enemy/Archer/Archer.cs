using System.Collections;
using UnityEngine;

public class ArcherEnemy : Enemy
{
    public GameObject arrowPrefab; // ������ ������
    public Transform shootPoint; // �����, ������ ����������� ������
    public float shootCooldown = 3f; // ����� ����� ������� ���������

    private bool isShooting = false; // ���� ��� ��������, �������� �� ������
    private float shootTimer = 1f; // ������ ��� ������� ������� �� ��������� �����
    private Vector2 randomDirection; // ����������� ��� ���������� ��������
    private float moveDuration = 2f; // ����� �������� � ����� �����������
    private float changeDirectionTime; // ������ ��� ����� �����������
    protected Rigidbody2D rb; // �������� � private �� protected

    private WaveManager waveManager;
    private Animator animator; // ��� ��������

    protected override void Start()
    {
        waveManager = GetComponent<WaveManager>();
        base.Start();
        rb = GetComponent<Rigidbody2D>(); // �������������� Rigidbody2D
        animator = GetComponent<Animator>(); // �������� ��������� Animator
        randomDirection = GetRandomDirection(); // ��������� ���������� �����������
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
        if (player == null || isShooting) return; // ���������, ��� ����� ����������

        isShooting = true;
        rb.velocity = Vector2.zero; // ������������� ������� ��� ��������

        // ������������ ������� � ������
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        FlipSprite(directionToPlayer); // ������� �� ��� X

        // �������� ��������� �������� �����
        int randomAnimation = Random.Range(0, 2); // 0 ��� 1

        animator.SetBool($"Shot{randomAnimation + 1}", true); // ���������� ������� ��������� ��� ���������� ���������
    
    }

    // ���� ����� ���������� ����� ������� � ��������
    public void ShootArrow()
    {
        enemyMoveSpeed = 0;
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

        enemyMoveSpeed = baseEnemyMoveSpeed; // ��������������� ������� ��������
        shootTimer = shootCooldown; // ���������� ������ �� ��������� ��������

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
