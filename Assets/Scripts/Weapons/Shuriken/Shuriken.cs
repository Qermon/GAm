using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : Weapon
{
    public GameObject shurikenPrefab; // ������ ��������
    public int shurikenCount = 5; // ���������� ���������

    private GameObject[] shurikens; // ������ ���������

    protected override void Start()
    {
        base.Start();
        CreateShurikens(); // ������� ��������
    }

    private void CreateShurikens()
    {
        if (shurikenPrefab == null) return;

        shurikens = new GameObject[shurikenCount];
        for (int i = 0; i < shurikenCount; i++)
        {
            shurikens[i] = Instantiate(shurikenPrefab, transform.position, Quaternion.identity);
            if (shurikens[i] == null)
            {
                Debug.LogError($"������� {i} �� ��� ������! ��������� ������.");
                return; // ���������� ����������, ���� �������� �� �������
            }
            shurikens[i].transform.parent = transform; // ������� ������ ���������
            shurikens[i].transform.localPosition = new Vector3(Mathf.Cos((360f / shurikenCount) * i * Mathf.Deg2Rad) * attackRange,
                                                                Mathf.Sin((360f / shurikenCount) * i * Mathf.Deg2Rad) * attackRange, 0);

            // ��������� ������� �������
            AdjustProjectileSize(shurikens[i]);

            Collider2D collider = shurikens[i].AddComponent<BoxCollider2D>();
            collider.isTrigger = true; // ������� ��������� ���������
            collider.tag = "Weapon"; // ���������� ��� ��� ��������

            // ��������� ��������� ��� ��������� ������������
            ShurikenCollision shurikenCollision = shurikens[i].AddComponent<ShurikenCollision>();
            shurikenCollision.weapon = this; // �������� ������ �� ������� ������
        }

        Debug.Log("��� �������� ������� �������.");
    }

    // ����� ��� ��������� ������� �������
    private void AdjustProjectileSize(GameObject shuriken)
    {
        if (shuriken != null)
        {
            // �������� ������ ������� ������� �� ������ ���������� projectileSize
            shuriken.transform.localScale = new Vector3(projectileSize, projectileSize, 1);
        }
    }

    public override void Update()
    {
        base.Update();

        for (int i = 0; i < shurikenCount; i++)
        {
            if (shurikens == null || shurikens[i] == null) // �������� �� null
            {
                continue;
            }

            // ���������� attackSpeed ��� ������� ���� ��������
            float angle = Time.time * attackSpeed * 100 + (360f / shurikenCount) * i; // �������� �� 100 ��� ��������� rotationSpeed
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * attackRange; // �������� �� attackRange
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * attackRange; // �������� �� attackRange

            shurikens[i].transform.localPosition = new Vector3(x, y, 0);
        }
    }

    protected override void PerformAttack()
    {
        // ����� ��� ���������� ������
        Debug.Log("����� �������� ��������� � ������: " + CalculateDamage());
    }
}


public class ShurikenCollision : MonoBehaviour
{
    public Weapon weapon; // ������ �� ������

    // ������� ��� ������������ ������� ��������� ����� �� ������� �����
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>();
    private float attackCooldown = 0.25f; // ����� ����� ������� �� ������ � ���� �� �����

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
                UpdateLastAttackTime(enemy.gameObject); // ��������� ����� ��������� �����
            }
        }
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
        lastAttackTimes[enemy] = Time.time; // ��������� ����� ��������� �����
    }
}