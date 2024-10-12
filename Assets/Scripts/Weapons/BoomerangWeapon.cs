using System.Collections;
using UnityEngine;

public class BoomerangWeapon : MonoBehaviour
{
    public GameObject boomerangPrefab; // ������ ���������
    public float attackInterval = 1.0f; // �������� ����� �������
    public int boomerangDamage = 10; // ���� ���������
    public float returnSpeed = 5f; // �������� ����������� ���������
    public float maxDistance = 5f; // ������������ ���������� ������ ���������

    private void Start()
    {
        StartCoroutine(ThrowBoomerang()); // ��������� �������� ��� �����
    }

    private IEnumerator ThrowBoomerang()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval); // ���� ����� ��������� �������
            SpawnBoomerang();
        }
    }

    private void SpawnBoomerang()
    {
        GameObject spawnedBoomerang = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);
        BoomerangBehaviour boomerangBehaviour = spawnedBoomerang.AddComponent<BoomerangBehaviour>(); // ��������� ��������� ���������
        boomerangBehaviour.Initialize(boomerangDamage, transform.position, maxDistance, returnSpeed); // ������������� ���������
    }
}

public class BoomerangBehaviour : MonoBehaviour
{
    private int damage; // ���� ���������
    private Vector3 startPosition; // ��������� ������� ���������
    private float maxDistance; // ������������ ���������� ������
    private float returnSpeed; // �������� ����������� ���������
    private bool returning; // ���� ����������� ���������

    private void Update()
    {
        if (returning)
        {
            // ������������ � ������
            transform.position = Vector3.MoveTowards(transform.position, startPosition, returnSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPosition) < 0.1f)
            {
                Destroy(gameObject); // ���������� �������� ��� ���������� ������
            }
        }
        else
        {
            // ������� ���������� ����� � �������
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, maxDistance, LayerMask.GetMask("Enemy"));
            if (enemies.Length > 0)
            {
                Collider2D closestEnemy = enemies[0];
                float closestDistance = Vector3.Distance(transform.position, closestEnemy.transform.position);

                // ���� ���������� �����
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
        }
    }

    public void Initialize(int boomerangDamage, Vector3 playerPosition, float maxDistance, float returnSpeed)
    {
        damage = boomerangDamage;
        startPosition = playerPosition;
        this.maxDistance = maxDistance;
        this.returnSpeed = returnSpeed; // ������������� �������� �����������
    }
}
