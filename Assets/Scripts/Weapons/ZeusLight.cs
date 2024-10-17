using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZeusLight : Weapon
{
    public GameObject projectilePrefab; // ������ �������
    public int maxBounces = 5; // ���������� ��������
    public float bounceDamageReduction = 0.1f; // ������� ���������� ����� �� ������ �������
    public float targetRadius = 7f; // ������ ������ �����
    public float spawnCooldown = 2f; // ������� ����� ������� ��������

    private bool canSpawnProjectile = true; // ����, ����� �� �������� ����� ������

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchProjectileWithCooldown()); // ��������� �������� ��� �������� � ���������
    }

    private IEnumerator LaunchProjectileWithCooldown()
    {
        while (true) // ����������� ���� ��� ����������� ������ ��������
        {
            if (canSpawnProjectile)
            {
                LaunchProjectile(); // ������� ������
                canSpawnProjectile = false; // ������ ����, ��� ����� �������� ����������
                yield return new WaitForSeconds(spawnCooldown); // ���� �������
                canSpawnProjectile = true; // ��������� ����� ���������� �������
            }

            yield return null; // ���� �� ���������� �����
        }
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon"; // ������������� ��� ��� �������
        projectile.AddComponent<ZeusProjectile>().Initialize(this, maxBounces, bounceDamageReduction); // �������������� ������
    }
}

public class ZeusProjectile : MonoBehaviour
{
    private ZeusLight weapon; // ������ �� ������
    private Enemy target; // ������� ����
    private int bouncesLeft; // ������� ���������� ��������
    private float currentDamage; // ������� ����
    private float bounceDamageReduction; // ������� ���������� �����
    private bool isMoving = false; // ���� ��� ������������ ��������
    private float maxLifetime = 5f; // ������������ ����� ����� �������
    private float lifetimeTimer; // ������ ��� ������������ ������� �����

    public void Initialize(ZeusLight weapon, int maxBounces, float bounceDamageReduction)
    {
        this.weapon = weapon;
        this.bouncesLeft = maxBounces;
        this.currentDamage = weapon.damage;
        this.bounceDamageReduction = bounceDamageReduction;
        this.lifetimeTimer = maxLifetime;

        FindRandomTarget(); // ������� ������ ����
        if (target != null)
        {
            Debug.Log("Target found: " + target.name); // ���������� ���������
            isMoving = true;
        }
        else
        {
            Debug.LogWarning("No available enemies; destroying projectile.");
            Destroy(gameObject); // ���������� ������, ���� ����� ���
        }
    }

    private void FindRandomTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, weapon.targetRadius, LayerMask.GetMask("Mobs", "MobsFly")); // ������� ������ � �������
        if (enemies.Length > 0)
        {
            target = enemies[Random.Range(0, enemies.Length)].GetComponent<Enemy>(); // �������� ���������� �����
            Debug.Log("Random target selected: " + target.name);
        }
        else
        {
            Debug.Log("No enemies found in range.");
            target = null; // ���� ������ ���, ���������� ����
        }
    }

    private void FindNearestTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, weapon.targetRadius, LayerMask.GetMask("Mobs", "MobsFly"));
        float closestDistance = float.MaxValue;
        Enemy closestEnemy = null;

        foreach (Collider2D enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && enemy != target)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        target = closestEnemy;
        if (target != null)
        {
            Debug.Log("Nearest target found: " + target.name);
        }
        else
        {
            Debug.Log("No nearest target found.");
        }
    }

    private void Update()
    {
        lifetimeTimer -= Time.deltaTime; // ��������� ����� �����
        if (lifetimeTimer <= 0)
        {
            Debug.Log("Projectile lifetime expired; destroying.");
            Destroy(gameObject); // ���������� ������, ���� ����� ����� �����������
        }

        if (isMoving && target != null)
        {
            MoveTowardsTarget(); // ��������� ������� �������
        }
        else if (target == null)
        {
            Debug.LogWarning("No target; destroying projectile.");
            Destroy(gameObject); // ���������� ������, ���� ��� ����
        }
    }

    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * weapon.projectileSpeed * Time.deltaTime;

            // ������������ ������ � ������� ��������
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    private void DealDamage(Enemy enemy)
    {
        Debug.Log("Dealing damage: " + currentDamage + " to " + enemy.name);
        enemy.TakeDamage((int)currentDamage); // ������� ���� �����
        currentDamage -= currentDamage * bounceDamageReduction; // ��������� ����
        bouncesLeft--; // ��������� ���������� ��������
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("Projectile collided with enemy: " + enemy.name);
                DealDamage(enemy);

                if (bouncesLeft > 0)
                {
                    FindNearestTarget(); // ���� ��������� ����
                    if (target == null)
                    {
                        Debug.Log("No more targets; destroying projectile.");
                        Destroy(gameObject); // ���������� ������, ���� ����� ������ ���
                    }
                }
                else
                {
                    Debug.Log("Max bounces reached; destroying projectile.");
                    Destroy(gameObject); // ���������� ������, ���� �������� ������ ���
                }
            }
        }
    }

    private void OnBecameInvisible()
    {
        // ���������� ������, ���� �� ����� �� ������� ������
        Debug.Log("Projectile left the screen; destroying.");
        Destroy(gameObject);
    }
}
