using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Transform player; // ������ �� ������ ������
    public float moveSpeed; // �������� ������������ ����
    private Vector2 moveDir; // ����������� �������� ����
    private float lastHorizontalVector;

    // Start is called before the first frame update
    void Start()
    {
        // ����� ������ ������ � ����������� PlayerMovement
        player = FindObjectOfType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        // �������� ���� � ������� ������
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // ���������� ����������� �� ������
        moveDir = (player.position - transform.position).normalized;

        // ������� ���� � ������� ������
        FlipSprite(moveDir);
    }

    void FlipSprite(Vector2 direction)
    {
        // ���� ����� ��������� ������, ���������� ���� ������ (�� ��� X), ���� ����� � �����
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // ��������� ������
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // ��������� �����
        }
    }
}
