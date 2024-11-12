using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 7f; // �������� ������ ������
    public float damage = 5f; // ����, ������� ������� ������
    public float baseDamage = 5f;
    private Transform target; // ���� (�����)

    void Start()
    {
        // ������� ������ �� ���� "Player"
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // ���������, ��� ���� �������
        if (target == null)
        {
            Debug.LogError("����� � ����� Player �� ������ �� �����.");
        }
    }

    void Update()
    {
        // ���� ���� ����������, ���������� ������ � ������� ������
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            // ������������ ������ � ������� ������
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    public void UpdateStats(float projectile)
    {
        damage += baseDamage / 100 * projectile;
    }

    public void RefreshStats()
    {
        damage = baseDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ��������� ������������ � ��������, ������� ��� "Wall" ��� "Weapon"
        if (collision.CompareTag("Wall") || collision.CompareTag("Weapon"))
        {
            Destroy(gameObject); // ���������� ������
            return;
        }

        // ��������� ������������ � �������
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            // ���������, ��� ��� �� CircleCollider2D (������ �����)
            if (collision is CircleCollider2D)
            {
                // ���������� ��������� ������� �����
                return;
            }

            // ������� ���� ������ ������ ���� ��� �� CircleCollider2D
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);
            }

            Destroy(gameObject); // ���������� ������ ����� �����
        }
    }

}
