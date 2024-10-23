using UnityEngine;
using System.Collections;

public class KillerFog : Weapon
{
    public GameObject projectilePrefab; // ������ �������
    public int projectileCount = 4; // ���������� �������� � �����
    public float projectileDelay = 0.2f; // �������� ����� ��������� � ����� �����

    protected override void Start()
    {
        base.Start();
        StartCoroutine(LaunchSalvos()); // ��������� �������� ��� ������� ������
    }

    private IEnumerator LaunchSalvos()
    {
        while (true) // ����������� ���� ��� ����������� ������� ������
        {
            if (IsEnemyInRange()) // ���������, ���� �� ����� � ������� �����
            {
                for (int i = 0; i < projectileCount; i++)
                {
                    LaunchProjectile(); // ��������� ������
                    yield return new WaitForSeconds(projectileDelay); // ���� �������� ����� ���������
                }
            }

            yield return new WaitForSeconds(attackSpeed); // ���� �������� ����� �������
        }
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Mobs", "MobsFly"));
        return enemies.Length > 0; // ���� ���� ���� �� ���� ���� � ������� �����, ���������� true
    }

    private void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = "Weapon"; // ������������� ��� ��� �������
        projectile.AddComponent<ProjectileFog>().Initialize(this); // ��������� ��������� ProjectileFog � �������� ������ �� ������� ������
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // ������ ������ ����� � ���������
    }
}

public class ProjectileFog : MonoBehaviour
{
    private KillerFog weapon; // ������ �� ������
    public float speed = 6f; // �������� �������
    private Enemy target; // ������� ����

    public void Initialize(KillerFog weapon)
    {
        this.weapon = weapon; // �������� ������ �� KillerFog
        FindRandomTarget(); // ������� ��������� ����
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            StartCoroutine(MoveProjectile(direction));
        }
        else
        {
            Destroy(gameObject); // ���������� ������, ���� ������ ���
            Debug.LogWarning("No available enemies; destroying projectile.");
        }
    }

    private void FindRandomTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, weapon.attackRange, LayerMask.GetMask("Mobs", "MobsFly")); // ���������� ������ ����� �� ������
        if (enemies.Length > 0)
        {
            target = enemies[Random.Range(0, enemies.Length)].GetComponent<Enemy>(); // �������� ���������� �����
        }
    }

    private IEnumerator MoveProjectile(Vector3 direction)
    {
        while (true) // ���������� ������� ������, ���� �� ����������
        {
            if (target == null)
            {
                FindRandomTarget(); // ������� ������ �����, ���� ���������� ���������
                if (target == null)
                {
                    Destroy(gameObject); // ���������� ������, ���� ������ ������ ���
                    yield break; // ������� �� ��������
                }
                direction = (target.transform.position - transform.position).normalized; // ��������� �����������
            }

            transform.position += direction * speed * Time.deltaTime; // ������� ������

            // ������������ ������ � ������� ��������
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // ������������ ���� ��������
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // ������������ ������

            yield return null; // ���� �� ���������� �����

            // ���������, ��� �� ����
            if (target != null && !target.gameObject.activeInHierarchy)
            {
                target = null; // ������� ������ �� ����, ���� ��� ����������
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                float finalDamage = weapon.CalculateDamage(); // ������������ ��������� ����
                enemy.TakeDamage((int)finalDamage); // ������� ���� �����
                Destroy(gameObject); // ���������� ������ ��� ���������
            }
        }
    }
}
