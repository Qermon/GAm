using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningWeapon : Weapon
{
    public GameObject lightningPrefab; // ������ ������
    public int lightningCount = 5; // ���������� ������ �� ���

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SpawnLightning()); // ��������� �������� ��� ������ ������
    }

    private IEnumerator SpawnLightning()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / attackSpeed); // ���� ����� ��������� �������
            if (attackTimer <= 0f) // ���������, ����� �� ���������
            {
                for (int i = 0; i < lightningCount; i++)
                {
                    SpawnLightningBolt();
                }
                attackTimer = 1f / attackSpeed; // ������������� ������ �����
            }
        }
    }

    private void SpawnLightningBolt()
    {
        Vector2 randomPosition;

        // ���� ���������� �������, ���� �� ������ ��
        do
        {
            randomPosition = (Vector2)transform.position + Random.insideUnitCircle * attackRange; // �������� spawnRadius �� attackRange
        } while (IsPositionBlocked(randomPosition)); // ���������, ������������� �� �������

        GameObject spawnedLightning = Instantiate(lightningPrefab, randomPosition, Quaternion.identity);
        LightningBehaviour lightningBehaviour = spawnedLightning.AddComponent<LightningBehaviour>(); // ��������� ��������� ������
        lightningBehaviour.SetDamage((int)CalculateDamage()); // ������������� ���� ������
    }

    private bool IsPositionBlocked(Vector2 position)
    {
        // ���������, ���� �� ������� � ����� "Wall" � ������� 0.1 �� �������
        Collider2D hit = Physics2D.OverlapCircle(position, 0.1f, LayerMask.GetMask("Wall"));
        return hit != null; // ���� hit �� null, ������ �� ������� ���� ������ � ����� Wall
    }

    protected override void PerformAttack()
    {
        // ����� ����� ��������� ����� ��������, ������� ������ �� ������ �����
    }
}

public class LightningBehaviour : MonoBehaviour
{
    private int damage; // ���� ������
    private static Dictionary<GameObject, float> lastAttackTimes = new Dictionary<GameObject, float>(); // ������� ��� ������������ ������� ��������� �����
    private float attackCooldown = 0.5f; // ����� ����� ������� �� ������ ����� (1 �������)

    private void Start()
    {
        Destroy(gameObject, 3f); // ���������� ������ ����� 3 �������
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            GameObject enemy = collision.gameObject;
            if (CanAttackEnemy(enemy)) // ���������, ����� �� ��������� �����
            {
                collision.GetComponent<Enemy>().TakeDamage(damage); // ������� ���� �����
                UpdateLastAttackTime(enemy); // ��������� ����� ��������� �����
            }
        }
    }

    public void SetDamage(int lightningDamage)
    {
        damage = lightningDamage; // ������������� ����
    }

    // ��������, ����� �� ��������� ����� (�������� ����� ��������� �����)
    private bool CanAttackEnemy(GameObject enemy)
    {
        if (lastAttackTimes.ContainsKey(enemy))
        {
            float timeSinceLastAttack = Time.time - lastAttackTimes[enemy];
            return timeSinceLastAttack >= attackCooldown; // ���������, ������ �� ���������� �������
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
