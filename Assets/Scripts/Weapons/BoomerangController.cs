using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangController : Weapon
{
    public GameObject boomerangPrefab; // ������ ���������
    public float attackInterval = 1.0f; // �������� ����� �������
    public float speed = 10f; // �������� ���������
    public float returnSpeed = 5f; // �������� ����������� ���������
    public float maxDistance = 5f; // ������������ ���������� ������
    public float activationRange = 10f; // ������ ��������� ��� ������ ������

    private new void Start()
    {
        StartCoroutine(ShootBoomerangs()); // ��������� �������� ��� ������ ����������
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
        GameObject closestEnemy = FindClosestEnemy(); // ������� ���������� �����
        if (closestEnemy != null) // ���������, ���� �� �����
        {
            Vector3 directionToEnemy = (closestEnemy.transform.position - transform.position).normalized; // �������� ����������� � �����
            GameObject spawnedBoomerang = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);
            BoomerangBehaviour boomerangBehaviour = spawnedBoomerang.AddComponent<BoomerangBehaviour>(); // ��������� ��������� ���������
            boomerangBehaviour.Initialize(directionToEnemy, speed, (int)CalculateDamage(), transform, returnSpeed, maxDistance); // ������������� ���������
        }
    }

    private GameObject FindClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, activationRange, LayerMask.GetMask("Mobs", "MobsFly")); // ������� ���� ������ � ������� activationRange
        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.gameObject; // ���������� ���������� �����
            }
        }
        return closestEnemy; // ���������� ���������� �����
    }
}

public class BoomerangBehaviour : MonoBehaviour
{
    private Vector3 direction; // ����������� �������� ���������
    private float speed; // �������� ���������
    private int damage; // ���� ���������
    private Transform player; // ������ �� ������
    private float returnSpeed; // �������� �����������
    private float maxDistance; // ������������ ���������� ������
    private bool returning; // ���� �����������

    private Vector3 startPosition; // ��������� ������� ���������
    private float distanceTraveled; // ���������� ����������

    // ������� ��� ������������ ������� ��������� ����� �� ������� �����
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>();
    private float attackCooldown = 0.3f; // ����� ����� ������� �� ������ � ���� �� ����� (1 �������)

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
            // ������� �������� � �������� �����������
            transform.position += direction * speed * Time.deltaTime;
            distanceTraveled += speed * Time.deltaTime;

            // ��������� �� ������������ � ������
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Mobs", "MobsFly")); // ������� ���� ������ � ������� 0.5f

            foreach (var enemy in enemies)
            {
                if (CanAttackEnemy(enemy.gameObject)) // ���������, ����� �� ��������� �����
                {
                    // ������� ���� �����
                    enemy.GetComponent<Enemy>().TakeDamage(damage);
                    UpdateLastAttackTime(enemy.gameObject); // ��������� ����� ��������� �����
                }
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

    // ����� ��� ��������, ����� �� �� ��������� ����� (�� ������ ������� ��������� �����)
    private bool CanAttackEnemy(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            float timeSinceLastAttack = Time.time - lastAttackTimes[enemy];
            return timeSinceLastAttack >= attackCooldown; // ���������, ������ �� ������ attackCooldown ������
        }
        return true; // ���� ����� �� ����� ����� ��� �� ����, ����� ���������
    }

    // ����� ��� ���������� ������� ��������� �����
    private void UpdateLastAttackTime(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            lastAttackTimes[enemy] = Time.time; // ��������� ����� ��������� �����
        }
        else
        {
            lastAttackTimes.Add(enemy, Time.time); // ��������� ������ � ������� �����, ���� ����� ��� ��� � �������
        }
    }
}
