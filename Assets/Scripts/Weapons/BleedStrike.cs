using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BleedStrike : Weapon
{
    public GameObject projectilePrefab; // ������ �������
    public float slowEffect = 0.15f; // ���������� �����
    public float bleedDuration = 3f; // ������������ ������������
    public float projectileLifetime = 3f; // ����� ����� �������

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchBleedStrike()); // ������ �������� �����
    }

    private IEnumerator LaunchBleedStrike()
    {
        while (true)
        {
            if (IsEnemyInRange()) // �������� ������� ����� � �������
            {
                LaunchProjectile();
            }
            yield return new WaitForSeconds(1f / attackSpeed); // �������� ����� �������
        }
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        return enemies.Length > 0;
    }

    private void LaunchProjectile()
    {
        Collider2D nearestEnemy = FindNearestEnemy();
        if (nearestEnemy == null) return;

        Vector3 targetPosition = nearestEnemy.transform.position;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon"; // ������������� ���
        projectile.AddComponent<BleedProjectile>().Initialize(this, targetPosition, projectileLifetime, bleedDuration, slowEffect, damage);
    }

    private Collider2D FindNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        float minDistance = float.MaxValue;
        Collider2D nearestEnemy = null;

        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
}

public class BleedProjectile : MonoBehaviour
{
    private Vector3 direction;
    private float projectileLifetime;
    private float bleedDuration;
    private float slowEffect;
    private float initialDamage;
    private List<Enemy> hitEnemies = new List<Enemy>();

    public void Initialize(BleedStrike weapon, Vector3 targetPosition, float lifetime, float bleedDuration, float slowEffect, float initialDamage)
    {
        this.projectileLifetime = lifetime;
        this.bleedDuration = bleedDuration;
        this.slowEffect = slowEffect;
        this.initialDamage = initialDamage;

        direction = (targetPosition - transform.position).normalized; // ����������� �� ����� � ������ ������
        StartCoroutine(DestroyAfterLifetime()); // ������ �������� ��� ����������� ����� 3 ���.
    }

    private void Update()
    {
        // ������� ������
        transform.position += direction * Time.deltaTime * 5f;

        // ������������ ������ � ����������� ��������
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(projectileLifetime);
        Destroy(gameObject); // ���������� ������ ����� �������� �����
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                ApplyEffects(enemy);
                hitEnemies.Add(enemy);
            }
        }
    }

    private void ApplyEffects(Enemy enemy)
    {
        // ���������� ����
        enemy.TakeDamage((int)initialDamage);

        // ����������, ���� ������� ���������� ������ 10%
        float currentSlowEffect = enemy.GetCurrentSlowEffect(); // ������������, ��� � ��� ���� ����� ��� ��������� �������� ����������
        if (currentSlowEffect < 0.1f) // ���������, ������ �� �������� ���������� 10%
        {
            enemy.ModifySpeed(1f - slowEffect, bleedDuration); // ��������� ����������
        }
        // ������ ������������
        StartCoroutine(ApplyBleedEffect(enemy));
    }

    private IEnumerator ApplyBleedEffect(Enemy enemy)
    {
        yield return new WaitForSeconds(1f);

        // ��������� �� null ������ ����� ��������
        if (enemy == null) yield break;

        float elapsed = 0f;
        float bleedDamage = initialDamage * 0.05f; // 5% ����� ������ �������

        while (elapsed < bleedDuration)
        {
            if (enemy != null) // ��������� �� null ����� ������� ������ TakeDamage
            {
                enemy.TakeDamage((int)bleedDamage);
            }

            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }

        if (enemy != null) // �������� ����� ������� ModifySpeed
        {
            // ���������� ����� ������������ �������� �� ���������� ������������
            enemy.ModifySpeed(1f / (1f - slowEffect), bleedDuration);
        }
    }

}
