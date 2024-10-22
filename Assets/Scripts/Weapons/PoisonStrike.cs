using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoisonStrike : Weapon
{
    public GameObject projectilePrefab; // ������ �������
    public int maxTargets = 5; // ������������ ���������� �����, �� ������� ����� ������ ������
    public float poisonDuration = 5f; // ����������������� ����������
    public float projectileLifetime = 5f; // ����� ����� �������
    public float activationRange = 10f; // ������, � ������� ������� �������� ����������

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchPoisonStrike()); // ������ �������� �����
    }

    private IEnumerator LaunchPoisonStrike()
    {
        while (true) // ����������� ���� ��� ����������� ������� ��������
        {
            if (IsEnemyInRange()) // ���������, ���� �� ���� � ������� ���������
            {
                LaunchProjectile(); // ��������� ������
            }
            yield return new WaitForSeconds(1f / attackSpeed); // ����, ������ �� �������� �����
        }
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, activationRange, LayerMask.GetMask("Mobs", "MobsFly"));
        return enemies.Length > 0; // ���� ���� ���� �� ���� ���� � ������� ���������, ���������� true
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon"; // ������������� ���
        projectile.AddComponent<PoisonProjectile>().Initialize(this, maxTargets, poisonDuration, damage * 0.5f); // ������������� ���� �� ���
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationRange); // ������ ������ ���������
    }
}

public class PoisonProjectile : MonoBehaviour
{
    private PoisonStrike weapon; // ������ �� ������
    private Enemy target; // ������� ����
    private int targetsHit = 0; // ������� ���������� �����
    private int maxTargets; // ������������ ���������� �����
    private float poisonDuration; // ����������������� ����������
    private float poisonDamagePerSecond; // ���� �� ��� � �������
    private List<Enemy> hitEnemies = new List<Enemy>(); // ������ ���������� ������
    private float projectileSpeed; // �������� �������
    private float projectileLifetime; // ����� ����� �������

    public void Initialize(PoisonStrike weapon, int maxTargets, float poisonDuration, float poisonDamagePerSecond)
    {
        this.weapon = weapon;
        this.maxTargets = maxTargets;
        this.poisonDuration = poisonDuration; // ����������� �������� ����������������� ����������
        this.poisonDamagePerSecond = poisonDamagePerSecond; // ����������� �������� ����� �� ��� � �������
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
                    // ���� ��� ���� � ������� 1.5, ���������� ������
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

    private void FindNearestTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Mobs", "MobsFly")); // ���� ������ � ������� 1.5
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


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && CanHitEnemy(enemy)) // ���������, ����� �� ������� �����
            {
                DealDamage(enemy); // ������� ���������� ����
                StartCoroutine(ApplyPoisonEffect(enemy)); // ��������� ������ ���
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
        float finalDamage = weapon.CalculateDamage(); // ������������ ���������� ����
        Debug.Log($"Direct damage dealt to {enemy.name}: {finalDamage}");
        enemy.TakeDamage((int)finalDamage); // ������� ���������� ����
        hitEnemies.Add(enemy); // ��������� ����� � ������ ����������
    }

    private IEnumerator ApplyPoisonEffect(Enemy enemy)
    {
        Debug.Log($"Enemy {enemy.name} is poisoned for {poisonDuration} seconds with {poisonDamagePerSecond} damage per second!");

        float elapsedTime = 0f;

        while (elapsedTime < poisonDuration)
        {
            enemy.TakeDamage((int)poisonDamagePerSecond); // ������� ���� �� ���
            Debug.Log($"Applying poison damage: {poisonDamagePerSecond} to {enemy.name}. Total elapsed time: {elapsedTime} seconds.");

            elapsedTime += 1f; // ���� 1 ������� ����� ��������� ������� �����
            yield return new WaitForSeconds(1f);
        }

        Debug.Log($"Poison effect on {enemy.name} has ended after {poisonDuration} seconds.");
    }
}
