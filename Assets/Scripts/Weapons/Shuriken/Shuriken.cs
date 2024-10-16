using UnityEngine;

public class Shuriken : Weapon
{
    public GameObject shurikenPrefab; // ������ ��������
    public int shurikenCount = 5; // ���������� ���������
    public float rotationRadius = 5f; // ������ �������� ������ ������

    private GameObject[] shurikens; // ������ ���������

    protected override void Start()
    {
        base.Start();

        // ������� ��������
        shurikens = new GameObject[shurikenCount];
        for (int i = 0; i < shurikenCount; i++)
        {
            shurikens[i] = Instantiate(shurikenPrefab, transform.position, Quaternion.identity);
            shurikens[i].transform.parent = transform; // ������� ������ ���������
            shurikens[i].transform.localPosition = new Vector3(Mathf.Cos((360f / shurikenCount) * i * Mathf.Deg2Rad) * rotationRadius,
                                                                Mathf.Sin((360f / shurikenCount) * i * Mathf.Deg2Rad) * rotationRadius, 0);
            Collider2D collider = shurikens[i].AddComponent<BoxCollider2D>();
            collider.isTrigger = true; // ������� ��������� ���������
            collider.tag = "Weapon"; // ���������� ��� ��� ��������

            // ��������� ��������� ��� ��������� ������������
            ShurikenCollision shurikenCollision = shurikens[i].AddComponent<ShurikenCollision>();
            shurikenCollision.weapon = this; // �������� ������ �� ������� ������
        }
    }

    protected override void Update() // ��������� �������� ����� override
    {
        base.Update(); // ����� ������ Update() �� �������� ������

        // ������� �������� ������ ������
        for (int i = 0; i < shurikenCount; i++)
        {
            float angle = Time.time * rotationSpeed + (360f / shurikenCount) * i; // ��������� ����� � ������
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * rotationRadius;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * rotationRadius;

            // ��������� ������� ��������
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                float finalDamage = weapon.CalculateDamage(); // ������������ ��������� ����
                enemy.TakeDamage((int)finalDamage); // ������� ���� �����
                Debug.Log("���� ������: " + finalDamage);
               
            }
        }
    }
}
