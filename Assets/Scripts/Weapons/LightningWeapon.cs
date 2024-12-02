using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningWeapon : Weapon
{
    public GameObject projectilePrefab; // ������ �������
    public float projectileLifetime = 3f; // ����� ����� �������
    public int numberOfProjectiles = 3; // ���������� ������

    private AudioSource audioSource; // �������� ����� (������ �� ��������� AudioSource �� �������)

    protected override void Start()
    {
        base.Start();

        // �������� ����� ������ � ��������� "Lighting" �� �����
        GameObject lightingObject = GameObject.Find("Lighting");
        if (lightingObject != null)
        {
            audioSource = lightingObject.GetComponent<AudioSource>();

            if (audioSource == null)
            {
                Debug.LogWarning("������ 'Lighting' ������, �� �� ����� ���������� AudioSource!");
            }
        }
        else
        {
            Debug.LogWarning("������ � ������ 'Lighting' �� ������ �� �����!");
        }

        StartCoroutine(LaunchLightning()); // ������ �������� �����
    }

    private IEnumerator LaunchLightning()
    {
        while (true)
        {
            if (IsEnemyInRange()) // �������� ������� ����� � �������
            {
                LaunchProjectiles();
            }
            yield return new WaitForSeconds(1f / attackSpeed); // �������� ����� �������
        }
    }

    public void LightingCountBuff()
    {
        numberOfProjectiles += 1;
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly", "Boss"));
        return enemies.Length > 0; // ���������, ���� �� ����� � ������� attackRange
    }

    private void LaunchProjectiles()
    {
        HashSet<Vector2> occupiedPositions = new HashSet<Vector2>(); // �������� ������� �������

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly", "Boss"));
        if (enemies.Length == 0) return; // ���� ��� ������, �������

        // ������������� ���� ������ ���� ���
        if (audioSource != null)
        {
            audioSource.Play(); // ����������� ���� � ������� ����������
        }

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            Vector2 spawnPosition = Vector2.zero; // �������������� ����������
            bool validPosition = false;
            int attempts = 0;

            while (!validPosition && attempts < 100) // ������� ����� �������������� �������
            {
                spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * attackRange; // ���������� attackRange

                // ���������, �� ������������ �� � ������������ � �� ������ �� �������
                if (!Physics2D.OverlapCircle(spawnPosition, 0.1f, LayerMask.GetMask("Wall")) && !occupiedPositions.Contains(spawnPosition))
                {
                    occupiedPositions.Add(spawnPosition); // ��������� ������� � �������
                    validPosition = true; // ������� �������
                }
                attempts++;
            }

            if (validPosition)
            {
                GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
                projectile.tag = "Weapon"; // ������������� ���

                // ��������� ������� �������
                AdjustProjectileSize(projectile);

                projectile.AddComponent<LightningProjectile>().Initialize(this, projectileLifetime, damage, criticalDamage);
            }
        }
    }

    // ����� ��� ��������� ������� �������
    private void AdjustProjectileSize(GameObject projectile)
    {
        if (projectile != null)
        {
            // �������� ������ ������� �� ������ ���������� projectileSize
            projectile.transform.localScale = new Vector3(projectileSize, projectileSize, 1);
        }
    }
}

public class LightningProjectile : MonoBehaviour
{
    private float projectileLifetime;
    private float initialDamage;
    private bool isCriticalHit;
    private Weapon weapon;

    public void Initialize(LightningWeapon weapon, float lifetime, float initialDamage, float criticalDamage)
    {
        this.projectileLifetime = lifetime;
        this.initialDamage = initialDamage;
        this.weapon = weapon;

        // ����������, �������� �� ���� �����������
        float randomValue = Random.value; // ��������� ���������� ����� �� 0 �� 1
        isCriticalHit = randomValue < weapon.criticalChance; // ������� ��� ������������ �����

        StartCoroutine(DestroyAfterLifetime());
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(projectileLifetime);
        Destroy(gameObject); // ���������� ������ ����� �������� �����
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                DealDamage(enemy);
                // ���������� ������ ����� �����
            }
        }
    }

    private void DealDamage(Enemy enemy)
    {
        // ������������ ���� � ������ ������������ �����
        float damageToDeal = isCriticalHit ? initialDamage * (1 + weapon.criticalDamage / 100f) : initialDamage;

        // ������� ���� �����
        enemy.TakeDamage((int)damageToDeal, isCriticalHit);
    }
}
