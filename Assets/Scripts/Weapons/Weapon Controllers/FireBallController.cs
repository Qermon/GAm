using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : Weapon
{
    public GameObject fireBallPrefab; // ������ ��������� ����
    public float projectileLifetime = 5f; // ����� ����� �������
    private new float attackTimer; // ������ ��� �����
    public float stunDuration = 0f; // ������������ �����

    protected override void Start()
    {
        base.Start();
        ResetAttackTimer();
    }

    public override void Update()
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

            // �������� ������ ������� �� ������ ���������� projectileSize
            AdjustProjectileSize(fireBall);

            FireBall fireBallScript = fireBall.AddComponent<FireBall>(); // ��������� ��������� ��� ������ �������
            fireBallScript.Initialize(nearestEnemy.transform.position, projectileSpeed, projectileLifetime, stunDuration, this); // �������� ���������

            // ����������� ���� ��������� ����
            AudioSource audioSource = fireBall.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.spatialBlend = 0f; // 2D ����
                audioSource.minDistance = 1f; // ����������� ���������� ��� �����
                audioSource.maxDistance = 15f; // ������������ ���������� ��� �����

                // ��������� ���� ����� ������������ ������� � ������ �������� ������
                Vector3 directionToEnemy = nearestEnemy.transform.position - transform.position;
                float angle = Vector3.SignedAngle(Vector3.right, directionToEnemy, Vector3.forward);

                // ����������� ���� ��� �������� �� -1 �� 1
                float pan;
                if (angle >= -90 && angle <= 90)
                {
                    pan = Mathf.InverseLerp(-90f, 90f, angle); // ������ �� ������: 0 �� 1
                }
                else
                {
                    pan = -Mathf.InverseLerp(90f, 270f, Mathf.Abs(angle)); // ����� �� ������: 0 �� -1
                }

                audioSource.panStereo = pan; // ������������� �������� �����
                audioSource.Play(); // ����������� ����
            }
        }
    }

    // ����� ��� ��������� ������� �������
    private void AdjustProjectileSize(GameObject fireBall)
    {
        if (fireBall != null)
        {
            // �������� ������ ������� �� ������ ���������� projectileSize
            fireBall.transform.localScale = new Vector3(projectileSize, projectileSize, 1);
        }
    }

    private GameObject FindNearestEnemy()
    {
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
                nearestEnemy = enemy.gameObject;
            }
        }

        return nearestEnemy;
    }

    public void IncreaseProjectileStunEffect(float percentage)
    {
        stunDuration += percentage;
    }
}
    

public class FireBall : MonoBehaviour
{
    private Vector3 direction; // ����������� ������ �������
    private float speed; // ��������
    private float lifetime; // ����� �����
    private float stunDuration; // ������������ �����
    private Weapon weapon; // ������ �� ������
    private float initialDamage;

    // ������� ��� ������������ ������� ��������� ����� �� �����
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>();

    public void Initialize(Vector3 targetPosition, float projectileSpeed, float projectileLifetime, float stunDuration, Weapon weaponInstance)
    {
        direction = (targetPosition - transform.position).normalized; // ��������� ����������� � �����
        speed = projectileSpeed;
        lifetime = projectileLifetime;
        this.stunDuration = stunDuration; // ����������� stunDuration
        weapon = weaponInstance; // ��������� ������ �� ������

        // ������������� �������� �����
        initialDamage = weapon.damage; // �����������, ��� CalculateDamage() ���������� ������� ����

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
                float damageToDeal = weapon.CalculateDamage(); // �������� ���� �� ������
                bool isCriticalHit = damageToDeal > weapon.damage; // ���������, ��� �� ����������� ����

                // ������� ���� �����
                enemy.TakeDamage((int)damageToDeal, isCriticalHit); // ��������� ����������� ����

                enemy.Stun(stunDuration);
            }
        }
    }

    // ��������, ����� �� ��������� ����� (�������� ����� ��������� �����)
    private bool CanAttackEnemy(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            float timeSinceLastAttack = Time.time - lastAttackTimes[enemy];
            if (timeSinceLastAttack < 1f) // ����� ��������� �� ����, ��� ��� � �������
            {
                return false;
            }
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

