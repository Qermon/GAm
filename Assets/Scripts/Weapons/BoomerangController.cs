using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangController : Weapon
{
    public GameObject boomerangPrefab; // ������ ���������
    public float speed = 10f; // �������� ���������
    public float returnSpeed = 5f; // �������� ����������� ���������
    public float maxDistance = 5f; // ������������ ���������� ������
    public float doubleDamageChance = 1f; // ���� �� ������� ���� (�� 0 �� 1)

    private new void Start()
    {
        base.Start();
        StartCoroutine(ShootBoomerangs()); // ��������� �������� ��� ������ ����������
    }

    private IEnumerator ShootBoomerangs()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / attackSpeed); // ���� ����� ��������� �������, ��������� �������� �����
            ShootBoomerang();
        }
    }

    private void ShootBoomerang()
    {
        GameObject closestEnemy = FindClosestEnemy(); // ������� ���������� �����
        if (closestEnemy != null) // ���������, ���� �� �����
        {
            Vector3 directionToEnemy = (closestEnemy.transform.position - transform.position).normalized;

            // ������������ ���� � ����������� ����
            float finalDamage = CalculateDamage();
            bool isCriticalHit = finalDamage > damage;

            // �������� �� ���� �������� �����
            bool isDoubleDamage = Random.value <= doubleDamageChance;

            // ���� �������� ���� �������� �����, ��������� ����
            if (isDoubleDamage)
            {
                finalDamage *= 2;
            }

            GameObject spawnedBoomerang = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);
            spawnedBoomerang.name = "Boomerang"; // ��������� ��� �������

            // �������� ������ ���������
            AdjustProjectileSize(spawnedBoomerang);

            BoomerangBehaviour boomerangBehaviour = spawnedBoomerang.AddComponent<BoomerangBehaviour>(); // ��������� ��������� ���������
            boomerangBehaviour.Initialize(directionToEnemy, speed, (int)finalDamage, isCriticalHit, isDoubleDamage, transform, returnSpeed, maxDistance); // �������� ��������
        }
    }

    // ����� ��� ��������� ������� ���������
    private void AdjustProjectileSize(GameObject boomerang)
    {
        if (boomerang != null)
        {
            // �������� ������ ��������� �� ������ ���������� projectileSize
            boomerang.transform.localScale = new Vector3(projectileSize, projectileSize, 1);
        }
    }

    private GameObject FindClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly", "Boss")); // ������� ���� ������ � ������� attackRange
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
    public void IncreaseProjectileDoubleDomageEffect(float percentage)
    {
        doubleDamageChance += percentage;
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
    private bool isCriticalHit; // ���� ������������ �����
    private bool isDoubleDamage; // ���� �������� �����

    private Vector3 startPosition; // ��������� ������� ���������
    private float distanceTraveled; // ���������� ����������

    private AudioSource audioSource; // �������� ����� ��� ���������

    // ������� ��� ������������ ������� ��������� ����� �� ������� �����
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>();
    private float attackCooldown = 0.3f; // ����� ����� ������� �� ������ � ���� �� ����� (0.3 �������)

    private void Start()
    {
        startPosition = transform.position; // ������������� ��������� �������
        Destroy(gameObject, 5f); // ���������� �������� ����� 5 ������, ���� �� ��������

        // �������� AudioSource �� ���������
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            // ��������� �������� ����� � ����������� �� �����������
            float pan = Mathf.Clamp(direction.x, -0.4f, 0.4f); // -1 ��� ������, 1 ��� �������
            audioSource.panStereo = pan; // ������������� �������� � AudioSource

            // ��������������� �����
            audioSource.Play();
        }
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
                return;
            }
        }
        else
        {
            // ������� �������� � �������� �����������
            transform.position += direction * speed * Time.deltaTime;
            distanceTraveled += speed * Time.deltaTime;

            // ���������, �� ��������� �� ����������
            if (distanceTraveled >= maxDistance)
            {
                returning = true; // �������� �����������, ���� �������� ������������� ����������
            }
        }

        // ��������� �� ������������ � ������ � ������� ��������� ��� �������� � ����� �����������
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 0.25f, LayerMask.GetMask("Mobs", "MobsFly", "Boss"));
        foreach (var enemy in enemies)
        {
            if (CanAttackEnemy(enemy.gameObject)) // ���������, ����� �� ��������� �����
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage, isCriticalHit, isDoubleDamage); // �������� ���� �������� �����


                UpdateLastAttackTime(enemy.gameObject); // ��������� ����� ��������� �����
            }
        }
    }

    public void Initialize(Vector3 newDirection, float boomerangSpeed, int boomerangDamage, bool criticalHit, bool doubleDamage, Transform playerTransform, float returnSpeed, float maxDistance)
    {
        direction = newDirection.normalized;
        speed = boomerangSpeed;
        damage = boomerangDamage;
        isCriticalHit = criticalHit; // ������������� ���� ������������ �����
        isDoubleDamage = doubleDamage; // ������������� ���� �������� �����
        player = playerTransform;
        this.returnSpeed = returnSpeed;
        this.maxDistance = maxDistance;

        // ��������� �������� ��� �����
        if (audioSource != null)
        {
            // ��������� �������� �� ������ ����������� ��������
            float pan = Mathf.Clamp(direction.x, -1f, 1f); // ���� �����, �� -1, ���� ������, �� 1
            audioSource.panStereo = pan;
        }
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
