using System.Collections;
using UnityEngine;

public class BoomerangController : MonoBehaviour
{
    public GameObject boomerangPrefab; // ������ ���������
    public float speed = 10f; // �������� ���������
    public float attackInterval = 1.0f; // �������� ����� �������
    public int boomerangDamage = 10; // ���� ���������
    public float returnSpeed = 5f; // �������� ����������� ���������
    public float maxDistance = 5f; // ������������ ���������� ������

    private Vector3 lastMovedVector = Vector3.right; // ��������� ������ �������� (�� ��������� ������)

    private void Start()
    {
        StartCoroutine(ShootBoomerangs());
    }

    private void Update()
    {
        // ��������� ��������� ������ �������� ������
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            lastMovedVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).normalized;
        }
    }

    private IEnumerator ShootBoomerangs()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval); // ���� ����� ��������� �������
            ShootBoomerang();
        }
    }

    private void ShootBoomerang()
    {
        if (lastMovedVector != Vector3.zero) // ���������, ���� �� ��������
        {
            GameObject spawnedBoomerang = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);
            BoomerangBehaviour boomerangBehaviour = spawnedBoomerang.AddComponent<BoomerangBehaviour>(); // ��������� ��������� ���������
            boomerangBehaviour.Initialize(lastMovedVector, speed, boomerangDamage, transform, returnSpeed, maxDistance); // ������������� ���������
        }
    }
}

public class BoomerangBehaviour : MonoBehaviour
{
    private Vector3 direction;
    private float speed; // �������� ���������
    private int damage; // ���� ���������
    private Transform player; // ������ �� ������
    private float returnSpeed; // �������� �����������
    private float maxDistance; // ������������ ���������� ������
    private bool returning; // ���� �����������

    private Vector3 startPosition; // ��������� ������� ���������
    private float distanceTraveled; // ���������� ����������

    private void Start()
    {
        startPosition = transform.position; // ������������� ��������� �������
        Destroy(gameObject, 5f); // ���������� �������� ����� 5 ������, ���� �� ��������
    }

    private void Update()
    {
        if (returning)
        {
            // ������������ � ������
            if (player != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, returnSpeed * Time.deltaTime);
            }
            // ���������, �������� �� ������
            if (Vector3.Distance(transform.position, player.position) < 0.1f)
            {
                Destroy(gameObject); // ���������� �������� ��� ���������� ������
            }
        }
        else
        {
            // ������� �������� � ����������� �����
            transform.position += direction * speed * Time.deltaTime;
            distanceTraveled += speed * Time.deltaTime;

            // ��������� �� ������������ � ������
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Enemy"));
            if (enemies.Length > 0)
            {
                // ������� ���������� �����
                Collider2D closestEnemy = enemies[0];
                float closestDistance = Vector3.Distance(transform.position, closestEnemy.transform.position);

                foreach (var enemy in enemies)
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance < closestDistance)
                    {
                        closestEnemy = enemy;
                        closestDistance = distance;
                    }
                }

                // ������� ���� �����
                closestEnemy.GetComponent<Enemy>().TakeDamage(damage);
                returning = true; // �������� �����������
            }

            // ���������, �� ��������� �� ����������
            if (distanceTraveled >= maxDistance)
            {
                returning = true; // �������� �����������, ���� �������� ������������� ����������
            }
        }
    }

    public void Initialize(Vector3 newDirection, float boomerangSpeed, int boomerangDamage, Transform playerTransform, float returnSpeed, float maxDistance)
    {
        direction = newDirection.normalized; // ����������� �����������
        speed = boomerangSpeed; // ������������� ��������
        damage = boomerangDamage; // ������������� ����
        player = playerTransform; // ��������� ������ �� ������
        this.returnSpeed = returnSpeed; // ������������� �������� �����������
        this.maxDistance = maxDistance; // ������������� ������������ ����������
    }
}
