using System.Collections.Generic;
using UnityEngine;

public class FireBallController : Weapon
{
    public GameObject fireBallPrefab; // ������ ��������� ����
    public float projectileLifetime = 5f; // ����� ����� �������
    private new float attackTimer; // ������ ��� �����

    protected override void Start()
    {
        base.Start();
        ResetAttackTimer();


    }

    protected override void Update()
    {
        base.Update();

        // ��������� ������ �����
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f) // ���� ����� ��� ����� �������
        {
            SpawnFireBall();
            ResetAttackTimer();
        }
    }

    private void ResetAttackTimer()
    {
        attackTimer = 1f / attackSpeed; // ���������� ������ � ������ �������� �����
    }

    private void SpawnFireBall()
    {
        GameObject nearestEnemy = FindNearestEnemy(); // ���� ���������� �����
        if (nearestEnemy != null)
        {
            // ������� �������� ���
            GameObject fireBall = Instantiate(fireBallPrefab, transform.position, Quaternion.identity);
            fireBall.tag = "Weapon"; // ������������� ���
            FireBall fireBallScript = fireBall.AddComponent<FireBall>(); // ��������� ��������� ��� ������ �������
            fireBallScript.Initialize(nearestEnemy.transform.position, projectileSpeed, projectileLifetime, this); // �������� ���������
        }
    }

    private GameObject FindNearestEnemy()
    {
        // ������� ���� ������ � ������� attackRange �� ����� Mobs � MobsFly
        int enemyLayerMask = LayerMask.GetMask("Mobs", "MobsFly");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayerMask);

        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy.gameObject; // ��������� ���������� �����
            }
        }

        return nearestEnemy; // ���������� ���������� �����
    }
}

public class FireBall : MonoBehaviour
{
    private Vector3 direction; // ����������� ������ �������
    private float speed; // ��������
    private float lifetime; // ����� �����
    private Weapon weapon; // ������ �� ������

    // ������� ��� ������������ ������� ��������� ����� �� �����
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>();

    public void Initialize(Vector3 targetPosition, float projectileSpeed, float projectileLifetime, Weapon weaponInstance)
    {
        direction = (targetPosition - transform.position).normalized; // ��������� ����������� � �����
        speed = projectileSpeed;
        lifetime = projectileLifetime;
        weapon = weaponInstance; // ��������� ������ �� ������

        // ������������ ������ � ������� ����
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        Destroy(gameObject, lifetime); // ���������� ������ ����� lifetime ������
    }

    private void Update()
    {
        // ������� ������ �� �����������
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // ���� ������ �� �����
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && CanAttackEnemy(enemy.gameObject)) // ���������, ����� �� ��������� �����
            {
                float finalDamage = weapon.CalculateDamage(); // ������������ ����
                enemy.TakeDamage((int)finalDamage); // ������� ����
                Debug.Log("���� ��������� ���� ������: " + finalDamage);
                UpdateLastAttackTime(enemy.gameObject); // ��������� ����� ��������� �����
            }
        }
    }

    // ��������, ����� �� ��������� ����� (�������� ����� ��������� �����)
    private bool CanAttackEnemy(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            float timeSinceLastAttack = Time.time - lastAttackTimes[enemy];
        }
        return true; // ���� ���� ��� �� ��������, ����� ���������
    }

    // ���������� ������� ��������� �����
    private void UpdateLastAttackTime(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            lastAttackTimes[enemy] = Time.time; // ��������� ����� ��������� �����
        }
        else
        {
            lastAttackTimes.Add(enemy, Time.time); // ��������� ����� � �������
        }
    }
}
