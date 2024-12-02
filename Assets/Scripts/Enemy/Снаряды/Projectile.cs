using UnityEngine;

public class WitchsProjectile : MonoBehaviour
{
    public float speed = 10f; // �������� �������
    public float lifetime = 5f; // ����� ����� �������
    private Rigidbody2D rb; // ������ �� Rigidbody2D
    private Vector2 direction; // ����������� �������� �������
    public float damage = 3f;
    public float baseDamage = 3f;

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

    public void UpdateStats(float projectile)
    {
        damage += baseDamage / 100 * projectile;
    }

    public void RefreshStats()
    {
        damage = baseDamage;
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

            // ���������, ��� ��� �� CircleCollider2D (������ �����)
            if (collision is CircleCollider2D)
            {
                // ���������� ��������� ������� �����
                return;
            }

            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage); // ������� ����
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
