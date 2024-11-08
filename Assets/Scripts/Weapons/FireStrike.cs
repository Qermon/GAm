using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireStrike : Weapon
{
    public GameObject projectilePrefab; // ������ �������
    public float projectileLifetime = 3f; // ����� ����� �������

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchFireStrike()); // ������ �������� �����
    }

    private IEnumerator LaunchFireStrike()
    {
        while (true)
        {
            if (IsEnemyInRange())
            {
                LaunchProjectile();
            }
            yield return new WaitForSeconds(1f / attackSpeed); // ����, ������ �� �������� �����
        }
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        return enemies.Length > 0;
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon";

        // ������������� ������ �������
        AdjustProjectileSize(projectile);

        projectile.AddComponent<FireProjectile>().Initialize(this, damage); // �������� ���� � ������
    }

    // ����� ��� ��������� ������� �������
    private void AdjustProjectileSize(GameObject projectile)
    {
        if (projectile != null)
        {
            projectile.transform.localScale = new Vector3(projectileSize, projectileSize, 1); // ������������� ������
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

public class FireProjectile : MonoBehaviour
{
    private float initialDamage;
    private float burnDuration = 5f; // ������������ �������
    private float burnTickInterval = 1f; // �������� ����� �������
    private float burnDamageFactor = 0.25f; // ��������� ���� �� ������� (25% �� �����)
    private float burnDecayFactor = 0.05f; // ���������� ����� �� 5% ������ ���
    private float projectileSpeed = 3.5f; // �������� ������
    private List<Enemy> hitEnemies = new List<Enemy>(); // ������ ���������� ������
    private bool isCriticalHit;
    private Weapon weapon;

    private AudioSource audioSource; // �������� ����� ��� �������
    private Vector3 direction; // ����������� �������� �������

    public void Initialize(FireStrike weapon, float damage)
    {
        this.weapon = weapon; // ��������� ������ �� ������
        initialDamage = damage;

        // �������� ��������� AudioSource � ����������� ���
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play(); // ������������� ���� ��� ������� �������
        }

        FindNearestEnemyDirection();
        StartCoroutine(DestroyAfterLifetime(3f)); // ���������� ������ ����� 3 �������
    }

    private void FindNearestEnemyDirection()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Mobs", "MobsFly"));
        if (enemies.Length > 0)
        {
            Transform nearestEnemy = enemies[0].transform;
            direction = (nearestEnemy.position - transform.position).normalized;
            StartCoroutine(MoveProjectile(direction));
            RotateTowardsDirection(direction);
        }
        else
        {
            Destroy(gameObject); // ���������� ������, ���� ������ ���
        }
    }

    private IEnumerator MoveProjectile(Vector3 direction)
    {
        while (true)
        {
            transform.position += direction * projectileSpeed * Time.deltaTime;

            // ��������� �������� ����� � ����������� �� �����������
            if (audioSource != null)
            {
                float pan = Mathf.Clamp(direction.x, -0.4f, 0.4f); // ���� �����, �� -1, ���� ������, �� 1
                audioSource.panStereo = pan;
            }

            yield return null;
        }
    }

    private void RotateTowardsDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                ApplyDirectDamage(enemy);
                StartCoroutine(ApplyBurningEffect(enemy));
                hitEnemies.Add(enemy); // ��������� � ������ ���������� ������
            }
        }
    }

    private void ApplyDirectDamage(Enemy enemy)
    {
        // ��������� ����������� ������������ �����
        bool isCriticalHit = Random.value < weapon.criticalChance; // �����������, ��� criticalChance �������� � ���������
        float damageToDeal = isCriticalHit ? initialDamage * (1 + weapon.criticalDamage / 100f) : initialDamage; // ��������� ����������� ����

        enemy.TakeDamage((int)damageToDeal, isCriticalHit); // ������� ���� � ������ ������������ �����
    }

    private IEnumerator ApplyBurningEffect(Enemy enemy)
    {
        float remainingBurnDamage = initialDamage * burnDamageFactor;
        float elapsedTime = 0f;

        yield return new WaitForSeconds(burnTickInterval);

        while (elapsedTime < burnDuration)
        {
            if (enemy != null) // ��������� �� null ����� ���������� �����
            {
                enemy.TakeDamage((int)remainingBurnDamage, isCriticalHit); // ������� ���� �� ������� � ������ ������������ �����
                remainingBurnDamage *= (1 - burnDecayFactor);
            }
            elapsedTime += burnTickInterval;
            yield return new WaitForSeconds(burnTickInterval);
        }
    }

    private IEnumerator DestroyAfterLifetime(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
