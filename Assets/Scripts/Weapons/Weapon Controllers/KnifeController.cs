using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : Weapon
{
    public GameObject knifePrefab; // ������ �������
    public float speed = 10f; // �������� �������
    public float maxDistance = 5f; // ������������ ���������� ������
    public float instantKillChance = 0.05f; // ���� ������������� �������� (5%

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

            // �������� ������ �������
            AdjustProjectileSize(spawnedKnife);

            KnifeBehaviour knifeBehaviour = spawnedKnife.AddComponent<KnifeBehaviour>(); // ��������� ��������� �������
            knifeBehaviour.Initialize(directionToEnemy, speed, (int)CalculateDamage(), transform, maxDistance, this, instantKillChance); // ������������� ���������

            // ��������� ����� ��� �������
            AudioSource audioSource = spawnedKnife.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.spatialBlend = 0f;
                audioSource.minDistance = 1f;
                audioSource.maxDistance = 15f;

                float angle = Vector3.SignedAngle(Vector3.right, directionToEnemy, Vector3.forward);
                float pan = angle >= -90 && angle <= 90 ? Mathf.InverseLerp(-90f, 90f, angle) : -Mathf.InverseLerp(90f, 270f, Mathf.Abs(angle));
                audioSource.panStereo = pan;
                audioSource.Play();
            }
        }
    }

    // ����� ��� ��������� ������� �������
    private void AdjustProjectileSize(GameObject knife)
    {
        if (knife != null)
        {
            knife.transform.localScale = new Vector3(projectileSize, projectileSize, 1); // ��������� �������� ������
        }
    }

    private GameObject FindEnemyWithMostHealth()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        GameObject strongestEnemy = null;
        float highestHealth = -1;

        foreach (Collider2D enemy in enemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null && enemyScript.currentHealth > highestHealth)
            {
                highestHealth = enemyScript.currentHealth;
                strongestEnemy = enemy.gameObject;
            }
        }
        return strongestEnemy;
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        return enemies.Length > 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void MomentKill(float percentage)
    {
        instantKillChance += percentage;
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
    private float instantKillChance; // ���� ������������� �������� (5%)


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
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    // �������� �� ������������ ��������
                    if (Random.value < instantKillChance)
                    {
                        // ������������ ��������, ���� �������� �����
                        enemyScript.TakeDamage((int)enemyScript.currentHealth + 1, true, true);

                    }

                    else
                    {
                        // ������������ ����
                        float damageDealt = CalculateDamage(); // ������������ ����
                        bool isCriticalHit = damageDealt > weapon.damage; // ���������, ��� �� ���� �����������
                        enemyScript.TakeDamage((int)damageDealt, isCriticalHit); // ������� ����
                    }
                }

                UpdateLastAttackTime(enemy.gameObject); // ��������� ����� ��������� �����
            }
        
        }

        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Vector3 newDirection, float knifeSpeed, int knifeDamage, Transform playerTransform, float maxDistance, Weapon weapon, float instantKillChance)
    {
        direction = newDirection.normalized; // ����������� �����������
        speed = knifeSpeed; // ������������� ��������
        baseDamage = knifeDamage; // ������������� ������� ����
        player = playerTransform; // ��������� ������ �� ������
        this.maxDistance = maxDistance; // ������������� ������������ ����������
        this.weapon = weapon; // ��������� ������ �� ������
        this.instantKillChance = instantKillChance; // ������������� ���� ������������� ��������
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
