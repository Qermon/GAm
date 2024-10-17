using UnityEngine;
using System.Collections;

public class LightningWeapon : Weapon
{
    public GameObject lightningPrefab; // ������ ������
    public float attackInterval = 1.0f; // �������� ����� ������� ������
    public int lightningCount = 5; // ���������� ������ �� ���
    public float spawnRadius = 2f; // ������ ������ ������

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SpawnLightning()); // ��������� �������� ��� ������ ������
    }

    private IEnumerator SpawnLightning()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval); // ���� ����� ��������� �������
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
            randomPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
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

    private void Start()
    {
        Destroy(gameObject, 3f); // ���������� ������ ����� 2 �������
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage(damage); // ������� ���� �����
           
        }
    }

    public void SetDamage(int lightningDamage)
    {
        damage = lightningDamage; // ������������� ����
    }
}