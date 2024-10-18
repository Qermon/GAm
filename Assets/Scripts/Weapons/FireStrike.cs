using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireStrike : Weapon
{
    public GameObject projectilePrefab; // ������ �������
    public int maxTargets = 5; // ������������ ���������� �����, �� ������� ����� ������ ������
    public float burnDuration = 3f; // ����������������� �������
    public float projectileLifetime = 5f; // ����� ����� �������

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchFireStrike()); // ������ �������� �����
    }

    private IEnumerator LaunchFireStrike()
    {
        while (true) // ����������� ���� ��� ����������� ������� ��������
        {
            LaunchProjectile(); // ��������� ������
            yield return new WaitForSeconds(1f / attackSpeed); // ����, ������ �� �������� �����
        }
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon"; // ������������� ���
        projectile.AddComponent<FireProjectile>().Initialize(this, maxTargets, burnDuration, damage * 0.5f); // ������������� ���� �� �������
    }
}

public class FireProjectile : MonoBehaviour
{
    private FireStrike weapon; // ������ �� ������
    private Enemy target; // ������� ����
    private int targetsHit = 0; // ������� ���������� �����
    private int maxTargets; // ������������ ���������� �����
    private float burnDuration; // ����������������� �������
    private float burnDamagePerSecond; // ���� �� ������� � �������
    private List<Enemy> hitEnemies = new List<Enemy>(); // ������ ���������� ������
    private float projectileSpeed; // �������� �������
    private float projectileLifetime; // ����� ����� �������

    public void Initialize(FireStrike weapon, int maxTargets, float burnDuration, float burnDamagePerSecond)
    {
        this.weapon = weapon;
        this.maxTargets = maxTargets;
        this.burnDuration = burnDuration; // ����������� �������� ����������������� �������
        this.burnDamagePerSecond = burnDamagePerSecond; // ����������� �������� ����� �� ������� � �������
        this.projectileSpeed = weapon.projectileSpeed;
        this.projectileLifetime = weapon.projectileLifetime;

        FindNearestTarget(); // ������� ��������� ����

        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            StartCoroutine(MoveProjectile(direction)); // ��������� �������� ��� �������� �������
        }
        else
        {
            Destroy(gameObject); // ���������� ������, ���� ������ ���
            Debug.LogWarning("No available enemies; destroying projectile.");
        }
    }

    private void FindNearestTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f, LayerMask.GetMask("Mobs", "MobsFly")); // ���� ������
        float closestDistance = float.MaxValue;
        foreach (Collider2D enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && !hitEnemies.Contains(enemy)) // �� �������� ��� ���������� ������
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = enemy;
                }
            }
        }
    }

    private IEnumerator MoveProjectile(Vector3 direction)
    {
        float lifetime = projectileLifetime;

        while (lifetime > 0 && targetsHit < maxTargets) // ������ ����� �� ��������� ������� ��� ���� �� �������� ��� ����
        {
            if (target == null)
            {
                FindNearestTarget(); // ���� ������ �����, ���� ������� ��� ���������
                if (target == null)
                {
                    Destroy(gameObject); // ���������� ������, ���� ������ ��� �����
                    yield break;
                }
                direction = (target.transform.position - transform.position).normalized; // ��������� �����������
            }

            transform.position += direction * projectileSpeed * Time.deltaTime; // ������� ������

            // ������������ ������ � ������� ��������
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            yield return null; // ���� �� ���������� �����

            lifetime -= Time.deltaTime; // ��������� ����� ����� �������

            // ���� ���� ��� ���������, ������� ��� �� ������ �����
            if (target != null && !target.gameObject.activeInHierarchy)
            {
                target = null;
            }
        }

        Destroy(gameObject); // ���������� ������ �� ��������� ������� ��� ����� ������������� ���������� ���������
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && CanHitEnemy(enemy)) // ���������, ����� �� ������� �����
            {
                DealDamage(enemy); // ������� ����
                StartCoroutine(ApplyBurningEffect(enemy)); // ��������� ������ �������
                targetsHit++; // ����������� ������� ���������� �����

                if (targetsHit >= maxTargets)
                {
                    Destroy(gameObject); // ���������� ������, ���� �������� ������������ ���������� �����
                }
            }
        }
    }

    private bool CanHitEnemy(Enemy enemy)
    {
        return !hitEnemies.Contains(enemy); // ���������, �� ��� �� ���� ��� �������
    }

    private void DealDamage(Enemy enemy)
    {
        float finalDamage = weapon.CalculateDamage(); // ������������ ����
        Debug.Log($"Direct damage dealt to {enemy.name}: {finalDamage}");
        enemy.TakeDamage((int)finalDamage); // ������� ����
        hitEnemies.Add(enemy); // ��������� ����� � ������ ����������
    }

    private IEnumerator ApplyBurningEffect(Enemy enemy)
    {
        Debug.Log($"Enemy {enemy.name} is burning for {burnDuration} seconds with {burnDamagePerSecond} damage per second!");

        float elapsedTime = 0f;

        while (elapsedTime < burnDuration)
        {
            enemy.TakeDamage((int)burnDamagePerSecond); // ������� ���� �� �������
            Debug.Log($"Applying burn damage: {burnDamagePerSecond} to {enemy.name}. Total elapsed time: {elapsedTime} seconds.");

            elapsedTime += 1f; // ���� 1 ������� ����� ��������� ������� �����
            yield return new WaitForSeconds(1f);
        }

        Debug.Log($"Burn effect on {enemy.name} has ended after {burnDuration} seconds.");
    }
}
