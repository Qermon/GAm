using System.Collections;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    public GameObject fireBallPrefab; // ������ ��������� ����
    public float speed = 10f; // �������� ��������� ����
    public float attackInterval = 1.0f; // �������� ����� �������
    public int fireBallDamage = 10; // ���� ��������� ����

    private Vector3 lastMovedVector = Vector3.right; // ��������� ������ �������� (�� ��������� ������)

    private void Start()
    {
        StartCoroutine(ShootFireBalls());
    }

    private void Update()
    {
        // ��������� ��������� ������ �������� ������
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            lastMovedVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).normalized;
        }
    }

    private IEnumerator ShootFireBalls()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval); // ���� ����� ��������� ���������
            ShootFireBall();
        }
    }

    private void ShootFireBall()
    {
        if (lastMovedVector != Vector3.zero) // ���������, ���� �� ��������
        {
            GameObject spawnedFireBall = Instantiate(fireBallPrefab, transform.position, Quaternion.identity);
            FireBallBehaviour fireBallBehaviour = spawnedFireBall.AddComponent<FireBallBehaviour>(); // ��������� ��������� ��������� ����
            fireBallBehaviour.SetDirection(lastMovedVector, speed, fireBallDamage); // ������������� �����������, �������� � ����
        }
    }
}

public class FireBallBehaviour : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private int damage; // ���� ��������� ����

    private void Start()
    {
        Destroy(gameObject, 5f); // ���������� �������� ��� ����� 5 ������, ���� �� �� ���������� � ���-��
    }

    private void Update()
    {
        // ������� �������� ��� �� ����������� � ������ ��������
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 newDirection, float fireBallSpeed, int fireBallDamage)
    {
        direction = newDirection.normalized; // ����������� �����������
        speed = fireBallSpeed;
        damage = fireBallDamage; // ������������� ����

        // ������������ �������� ��� � ������� ��������
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // ��������� ����
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // ������������ �������� ���
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage(damage); // ������� ���� �����

            // ����� ����� �������� ������ ��� ����������� ������� ��� �����
            // ��������, �������� ������� ��� ������������� ���� �����

            // ���� �� ������, ����� �������� ��� �������� ������ �����, ������� ������ Destroy
            // �� ��� ���� ����� ���������, ��� � ������ ��� �����������, ������� ����� �� ������ �������� ������
        }

    }
}
