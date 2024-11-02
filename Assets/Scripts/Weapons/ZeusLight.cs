using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZeusLight : Weapon
{
    public GameObject projectilePrefab; // ������ �������
    public int maxBounces = 5; // ���������� ��������
    public float bounceDamageReduction = 0.1f; // ������� ���������� ����� �� ������ �������

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchProjectileCoroutine()); // ��������� �������� ��� ��������
    }

    private IEnumerator LaunchProjectileCoroutine()
    {
        while (true) // ����������� ���� ��� ����������� ������ ��������
        {
            LaunchProjectile(); // ������� ������
            yield return new WaitForSeconds(1f / attackSpeed); // ���� ��������, ������������ attackSpeed
        }
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon"; // ������������� ��� ��� �������
        projectile.AddComponent<ZeusProjectile>().Initialize(this, maxBounces, bounceDamageReduction); // �������������� ������
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        return enemies.Length > 0; // ���� ���� ���� �� ���� ���� � ������� �����, ���������� true
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange); // ������ ������ �����
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
            Destroy(gameObject); // ���������� ������, ���� ����� ���
        }
    }

    private void FindRandomTarget()
    {
        // ������� ������ � ������� 1.5
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Mobs", "MobsFly"));
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
        // ������� ������ � ������� 1.5
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Mobs", "MobsFly"));
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
        // ��������� ����������� ������������ �����
        bool isCriticalHit = Random.value < weapon.criticalChance;
        float damageToDeal = isCriticalHit ? currentDamage * (1 + weapon.criticalDamage / 100f) : currentDamage;

        // ������� ���� �����
        enemy.TakeDamage((int)damageToDeal, isCriticalHit);
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
