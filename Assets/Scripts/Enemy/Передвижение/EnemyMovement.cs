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
        // ���������� ����������� �� ������
        moveDir = (player.position - transform.position).normalized;

        // ������� ���� � ������� ������
        FlipSprite(moveDir);

        // �������� ���� � ������� ������
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }


    void FlipSprite(Vector2 direction)
    {
        // �������� ������� �������
        Vector3 currentScale = transform.localScale;

        // ���� ����� ��������� ������, ���������� ���� ������ (�� ��� X), ���� ����� � �����
        if (direction.x > 0)
        {
            // ������������� ������� �� X � 1 (��� ��������� �������� �� Y � Z)
            transform.localScale = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
        }
        else if (direction.x < 0)
        {
            // ������������� ������� �� X � -1 (��� ��������� �������� �� Y � Z)
            transform.localScale = new Vector3(-Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
        }
    }
}

