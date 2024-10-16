using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 7f; // �������� ������ ������
    public int damage = 20; // ����, ������� ������� ������
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ��������� ������������ � ��������, ������� ��� "Wall" ��� "Weapon"
        if (collision.CompareTag("Wall") || collision.CompareTag("Weapon"))
        {
            Destroy(gameObject); // ���������� ������
        }

        // ��������� ������������ � �������
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            // ������� ���� ������
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject); // ���������� ������ ����� �����
        }
    }
}
