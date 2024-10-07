using UnityEngine;

public class MobFollow : MonoBehaviour
{
    public Transform player; // ������ �� ������ ������
    public float speed = 3f; // �������� ����
    public float stoppingDistance = 1.5f; // ���������, �� ������� ��� �����������

    void Update()
    {
        if (player != null)
        {
            // ��������� ��������� ����� ����� � �������
            float distance = Vector2.Distance(transform.position, player.position);

            // ���� ��������� ������, ��� stoppingDistance, ��� �������� � ������
            if (distance > stoppingDistance)
            {

                
                // ����������� � ������
                Vector2 direction = (player.position - transform.position).normalized;

                Vector3 playerPosition = player.position;  // ������� ������
                transform.position = Vector2.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);

              
            }
        }
    }
}