using System.Collections;
using UnityEngine;

public class MyProjectile : MonoBehaviour
{
    public float speed = 5f; // �������� �������
    public Vector2 direction; // ����������� ��������

    // ����� ��� ������������� �������
    public void Initialize(float speed, Vector2 direction)
    {
        this.speed = speed;
        this.direction = direction;

        // ��������� �������� ��� �������� �������
        StartCoroutine(MoveProjectile());
    }

    private IEnumerator MoveProjectile()
    {
        while (true) // ������� ��� ����������� �������
        {
            transform.position += (Vector3)(direction.normalized * speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ��������� ������������ � �������
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10); // ������� 10 ����� (��� ������ ��������)
            }
            Destroy(gameObject); // ���������� ������
        }
        // �������� ��������� ������������ � ������� ���������, ���� �����
    }
}
