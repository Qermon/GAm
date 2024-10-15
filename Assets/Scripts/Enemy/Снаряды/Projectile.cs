using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; // �������� �������
    public float lifetime = 5f; // ����� ����� �������
    private Rigidbody2D rb; // ������ �� Rigidbody2D
    private Vector2 direction; // ����������� �������� �������

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime); // ���������� ������ ����� �������� �����

        // ���������, ����������� �� �����������
        if (direction != Vector2.zero)
        {
            rb.velocity = direction * speed; // ������������� �������� �������

            // ������������ ������ � ������� ��������
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // ��������� ���� ��������
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // ������������ ������
        }
        else
        {
            Debug.LogError("����������� ������� �� �����������!");
        }
    }

    // ����� ��� ��������� ����������� �������
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized; // ������������� ����� �����������
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("������ ���������� �: " + collision.gameObject.name); // ���������, � ��� ������������ ������

        if (collision.CompareTag("Player"))
        {
            Debug.Log("������ ����� � ������!");
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(100); // ������� ����
                Debug.Log("������ ����� ���� ������! ������� ��������: " + playerHealth.currentHealth);
            }
            else
            {
                Debug.LogError("PlayerHealth ��������� �� ������ ��: " + collision.gameObject.name);
            }
            Destroy(gameObject); // ���������� ������
        }
        else if (collision.CompareTag("Wall"))
        {
            Debug.Log("������ ���������� �� ������ � ��� ���������!");
            Destroy(gameObject);
        }
    }
}