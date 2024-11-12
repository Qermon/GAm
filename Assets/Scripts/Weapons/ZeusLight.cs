using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZeusLight : Weapon
{
    public GameObject projectilePrefab;
    public int maxBounces = 5;
    public float bounceDamageReduction = 0.1f;

    public float splitChance = 0f; // ���� �� ���������� �������

    public override void Update()
    {
        base.Update(); // ���� ����� ������� ������������ �����
       
    
    // ��������� �������� ����� ������ ����, ���� ������ � ��������
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            Vector3 directionToProjectile = transform.position - Camera.main.transform.position;
            float angle = Vector3.SignedAngle(Vector3.right, directionToProjectile, Vector3.forward);
            float pan = Mathf.InverseLerp(-90f, 90f, angle);
            audioSource.panStereo = pan;
        }

        // ���� ������� ���������� �������
    }


    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchProjectileCoroutine());
    }

    private IEnumerator LaunchProjectileCoroutine()
    {
        while (true)
        {
            LaunchProjectile();
            yield return new WaitForSeconds(1f / attackSpeed);
        }
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon";
        projectile.AddComponent<ZeusProjectile>().Initialize(this, maxBounces, bounceDamageReduction, splitChance, damage);

        // �������� AudioSource �� ������� �������
        AudioSource audioSource = projectile.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.spatialBlend = 1f; // 3D ����
            audioSource.minDistance = 1f; // ����������� ���������� ��� �����
            audioSource.maxDistance = 15f; // ������������ ���������� ��� �����

            // ��������� ����������� �� ������ � �������
            Vector3 directionToProjectile = projectile.transform.position - transform.position;

            // ������� ���� ����� ������������ ������� � ������������ ������� ������
            float angle = Vector3.SignedAngle(Vector3.right, directionToProjectile, Vector3.forward);

            // ����������� ���� ��� �������� �� -1 �� 1
            float pan = Mathf.Clamp(Mathf.InverseLerp(-90f, 90f, angle), -1f, 1f); // ������������ ���� ��� �������� �� -1 �� 1

            // ������������� �������� �����
            audioSource.panStereo = pan;

            // ��������� ���������
            audioSource.volume = 1f;

            // ����������� ����
            audioSource.Play();
        }
    }

    public void zeusLightCountBounceBuff()
    {
        maxBounces += 2;
    }    

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly", "Boss"));
        return enemies.Length > 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void IncreaseProjectileSplitEffect(float percentage)
    {
        splitChance += percentage;

        if (splitChance > 0.5f)
        {
            splitChance = 0.5f;
        }
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
    private float maxLifetime = 10f; // ������������ ����� ����� �������
    private float lifetimeTimer; // ������ ��� ������������ ������� �����
    private float splitChance; // ���� ���������� �������

    public void Initialize(ZeusLight weapon, int maxBounces, float bounceDamageReduction, float splitChance, float initialDamage)
    {
        this.weapon = weapon;
        this.bouncesLeft = maxBounces; // ���������� �������
        this.currentDamage = initialDamage;  // ���� �������
        this.bounceDamageReduction = bounceDamageReduction;
        this.lifetimeTimer = maxLifetime;
        this.splitChance = splitChance;


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
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Mobs", "MobsFly", "Boss"));
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
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.GetMask("Mobs", "MobsFly", "Boss"));
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
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("Projectile collided with enemy: " + enemy.name);
                DealDamage(enemy);

                // �������� �� ���������� �������
                if (Random.value < splitChance)
                {
                    Debug.Log("Projectile split triggered!");

                    // ��������� ������ ��� �������� ���� ����� ��������
                    LaunchSplitProjectiles(enemy);
                }

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
    private void LaunchSplitProjectiles(Enemy initialTarget)
    {
       
        
            // ����� ���� ��� ����������� �������� � 75% �� ��������
            float splitDamage = currentDamage * 0.75f; // ���� ����� �� 25% ������

            // ������ ����� ������
            GameObject splitProjectile1 = Instantiate(weapon.projectilePrefab, transform.position, Quaternion.identity);
            ZeusProjectile splitProjectile1Script = splitProjectile1.AddComponent<ZeusProjectile>();
            splitProjectile1Script.Initialize(weapon, bouncesLeft, bounceDamageReduction, splitChance, splitDamage);

            // ������ ����� ������
            GameObject splitProjectile2 = Instantiate(weapon.projectilePrefab, transform.position, Quaternion.identity);
            ZeusProjectile splitProjectile2Script = splitProjectile2.AddComponent<ZeusProjectile>();
            splitProjectile2Script.Initialize(weapon, bouncesLeft, bounceDamageReduction, splitChance, splitDamage);

            // ������������� ����:
            splitProjectile1Script.target = initialTarget; // ������ ����� ������ ��������� ���� ����
            splitProjectile2Script.target = FindFarthestTarget(initialTarget);
        
    }

    private Enemy FindFarthestTarget(Enemy excludedEnemy)
    {
        // ������� ������ � ������� 2
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 2f, LayerMask.GetMask("Mobs", "MobsFly", "Boss"));
        float farthestDistance = 0f;
        Enemy farthestEnemy = null;

        foreach (Collider2D enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && enemy != excludedEnemy)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance > farthestDistance)
                {
                    farthestDistance = distance;
                    farthestEnemy = enemy;
                }
            }
        }

        return farthestEnemy;
    }

    private void OnBecameInvisible()
    {
        // ���������� ������, ���� �� ����� �� ������� ������
        Debug.Log("Projectile left the screen; destroying.");
        Destroy(gameObject);
    }
}
