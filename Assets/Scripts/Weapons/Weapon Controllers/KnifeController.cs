using System.Collections;
using UnityEngine;

public class KnifeController : Weapon
{
    public GameObject knifePrefab; // ������ �������
    public float attackInterval = 1.0f; // �������� ����� �������
    public float speed = 10f; // �������� �������
    public float maxDistance = 5f; // ������������ ���������� ������

    private new void Start()
    {
        StartCoroutine(ShootKnives()); // ��������� �������� ��� ������ ��������
    }

    private IEnumerator ShootKnives()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval); // ���� ����� ��������� �������
            ShootKnife();
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
            knifeBehaviour.Initialize(directionToEnemy, speed, (int)CalculateDamage(), transform, maxDistance); // ������������� ���������
        }
    }

    private GameObject FindEnemyWithMostHealth()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f, LayerMask.GetMask("Mobs", "MobsFly")); // ������� ���� ������ � ������� 10f
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
}

public class KnifeBehaviour : MonoBehaviour
{
    private Vector3 direction; // ����������� �������� �������
    private float speed; // �������� �������
    private int damage; // ���� �������
    private Transform player; // ������ �� ������
    private float maxDistance; // ������������ ���������� ������
    private float distanceTraveled; // ���������� ����������

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
        if (direction != Vector3.zero) // ���������, �� ������� �� ������
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // �������� ����
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // ������������ ������
        }

        // ��������� �� ������������ � ������
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Mobs", "MobsFly")); // ������� ���� ������ � ������� 0.5f

        foreach (var enemy in enemies)
        {
            // ������� ���� �����
            enemy.GetComponent<Enemy>().TakeDamage(damage);
        }

        // ���������, �� ��������� �� ����������
        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject); // ���������� ������, ���� �������� ������������� ����������
        }
    }

    public void Initialize(Vector3 newDirection, float knifeSpeed, int knifeDamage, Transform playerTransform, float maxDistance)
    {
        direction = newDirection.normalized; // ����������� �����������
        speed = knifeSpeed; // ������������� ��������
        damage = knifeDamage; // ������������� ����
        player = playerTransform; // ��������� ������ �� ������
        this.maxDistance = maxDistance; // ������������� ������������ ����������
    }
}
