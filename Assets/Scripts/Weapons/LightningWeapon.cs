using System.Collections;
using UnityEngine;

public class LightningWeapon : MonoBehaviour
{
    public GameObject lightningPrefab; // ������ ������
    public float attackInterval = 1.0f; // �������� ����� ������� ������
    public int lightningDamage = 10; // ���� ������
    public int lightningCount = 5; // ���������� ������ �� ���
    public float spawnRadius = 2f; // ����������� ������ ������ ������

    private void Start()
    {
        StartCoroutine(SpawnLightning()); // ��������� �������� ��� ������ ������
    }

    private IEnumerator SpawnLightning()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval); // ���� ����� ��������� �������
            for (int i = 0; i < lightningCount; i++)
            {
                SpawnLightningBolt();
            }
        }
    }

    private void SpawnLightningBolt()
    {
        // ���������� ��������� ������� � ������� �� ������
        Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
        GameObject spawnedLightning = Instantiate(lightningPrefab, randomPosition, Quaternion.identity);
        LightningBehaviour lightningBehaviour = spawnedLightning.AddComponent<LightningBehaviour>(); // ��������� ��������� ������
        lightningBehaviour.SetDamage(lightningDamage); // ������������� ���� ������
    }
}

public class LightningBehaviour : MonoBehaviour
{
    private int damage; // ���� ������

    private void Start()
    {
        Destroy(gameObject, 2f); // ���������� ������ ����� 2 �������
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage(damage); // ������� ���� �����
            Destroy(gameObject); // ���������� ������ ����� �����
        }
    }

    public void SetDamage(int lightningDamage)
    {
        damage = lightningDamage; // ������������� ����
    }
}
