using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : Weapon
{
    public GameObject knifePrefab; // ������ �������
    public float speed = 10f; // �������� �������
    public float maxDistance = 5f; // ������������ ���������� ������

    private new void Start()
    {
        base.Start(); // ���������, ��� ���������� ������� ����� Start, ���� ���� ���-�� ������ � ������������ ������
        StartCoroutine(ShootKnives()); // ��������� �������� ��� ������ ��������
    }

    private IEnumerator ShootKnives()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / attackSpeed); // �������� ����� �������, ������� �� ���������
            if (IsEnemyInRange()) // ���������, ���� �� ����� � ������� �����
            {
                ShootKnife();
            }
        }
    }

    private void ShootKnife()
    {
        GameObject targetEnemy = FindEnemyWithMostHealth(); // ������� ����� � ���������� ���������
        if (targetEnemy != null) // ���������, ���� �� �����
        {
            Vector3 directionToEnemy = (targetEnemy.transform.position - transform.position).normalized; // �������� ����������� � �����
            GameObject spawnedKnife = Instantiate(knifePrefab, transform.position, Quaternion.identity);
            KnifeBehaviour knifeBehaviour = spawnedKnife.AddComponent<KnifeBehaviour>(); // ��������� ��������� �������
            knifeBehaviour.Initialize(directionToEnemy, speed, (int)CalculateDamage(), transform, maxDistance, this); // ������������� ���������
        }
    }

    private GameObject FindEnemyWithMostHealth()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly")); // ������� ���� ������ � ������� �����
        GameObject strongestEnemy = null;
        float highestHealth = -1;

        foreach (Collider2D enemy in enemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null && enemyScript.currentHealth > highestHealth) // ���������� currentHealth
            {
                highestHealth = enemyScript.currentHealth; // ���������� �������� �����
                strongestEnemy = enemy.gameObject; // ���������� ����� � ���������� ���������
            }
        }
        return strongestEnemy; // ���������� ����� � ���������� ���������
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        return enemies.Length > 0; // ���� ���� ���� �� ���� ���� � ������� �����, ���������� true
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // ������ ������ ����� � ���������
    }
}

public class KnifeBehaviour : MonoBehaviour
{
    private Vector3 direction; // ����������� �������� �������
    private float speed; // �������� �������
    private int baseDamage; // ������� ���� �������
    private Transform player; // ������ �� ������
    private float maxDistance; // ������������ ���������� ������
    private float distanceTraveled; // ���������� ����������
    private Weapon weapon; // ������ �� ������

    // ������� ��� ������������ ������� ��������� ����� �� ������� �����
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>();
    private float attackCooldown = 1f; // ����� ����� ������� �� ������ � ���� �� ����� (1 �������)

    private void Start()
    {
        Destroy(gameObject, 5f); // ���������� ������ ����� 5 ������, ���� �� ��������
    }

    private void Update()
    {
        // ������� ������ � �������� �����������
        transform.position += direction * speed * Time.deltaTime;
        distanceTraveled += speed * Time.deltaTime;

        // ������������ ������ � ������� ��������
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        // ��������� �� ������������ � ������
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Mobs", "MobsFly"));

        foreach (var enemy in enemies)
        {
            if (CanAttackEnemy(enemy.gameObject))
            {
                // ������������ ���� � ���������, ��� �� �� �����������
                float damageDealt = CalculateDamage(); // ������������ ����
                bool isCriticalHit = damageDealt > weapon.damage; // ���������, ��� �� ���� �����������
                enemy.GetComponent<Enemy>().TakeDamage((int)damageDealt, isCriticalHit); // ������� ����
                UpdateLastAttackTime(enemy.gameObject); // ��������� ����� ��������� �����
            }
        }

        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }


    public void Initialize(Vector3 newDirection, float knifeSpeed, int knifeDamage, Transform playerTransform, float maxDistance, Weapon weapon)
    {
        direction = newDirection.normalized; // ����������� �����������
        speed = knifeSpeed; // ������������� ��������
        baseDamage = knifeDamage; // ������������� ������� ����
        player = playerTransform; // ��������� ������ �� ������
        this.maxDistance = maxDistance; // ������������� ������������ ����������
        this.weapon = weapon; // ��������� ������ �� ������
    }

    private float CalculateDamage()
    {
        // ���������� ��������� ��������
        float randomValue = Random.value;

        // �������� �� ����������� ����
        if (randomValue < weapon.criticalChance)
        {
            // ���� ����������� ����, ������������ ����������� ����
            float critDamage = baseDamage + baseDamage * (weapon.criticalDamage / 100f);
            return critDamage; // ���������� ����������� ����
        }
        return baseDamage; // ���������� ������� ����
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