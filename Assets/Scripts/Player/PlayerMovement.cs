using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;            // ������� �������� ��������
    public float baseMoveSpeed;        // ������� ��������, ������� ����� �����������
    private Rigidbody2D rb;            // Rigidbody ��������� ��� ���������� ��������
    public Vector2 moveDir;
    [HideInInspector]
    public float lastHorizontalVector;


    private Vector2 startTouchPosition;  // ��������� ������� �������
    private Vector2 currentTouchPosition; // ������� ������� ������
    private Vector2 direction;          // ����������� ��������

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // �������� ��������� Rigidbody2D
        moveSpeed = baseMoveSpeed;         // ������������� ��������� ��������
    }

    void Update()
    {
        HandleTouchInput();  // ������������ ���� � ������
    }

    void FixedUpdate()
    {
        Move();  // ������� ���������
    }

    // ����� ��� ��������� �������
    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            // �������� ���������� � ������ �������
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // ��������� ��������� ������� �������
                startTouchPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                // ��������� ������� ������� ������
                currentTouchPosition = touch.position;

                // ��������� �����������, � ������� ����� ������������
                direction = currentTouchPosition - startTouchPosition;
                moveDir = direction.normalized;  // ����������� ����������� ��� ����������� ��������

                // ��������� ��������� �������������� ��������
                lastHorizontalVector = moveDir.x;
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                // ����� ����� �������, ���������� �����������
                moveDir = Vector2.zero;
            }
        }
    }

    // ����� ��� �������� ���������
    void Move()
    {
        rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);  // ��������� ��������
    }

    // ����� ��� ���������� ��������
    public void IncreaseMoveSpeed(float percentage)
    {
        // ����������� �������� �� ������������ ������� �� ������� ��������
        float increaseAmount = baseMoveSpeed * percentage;
        moveSpeed += increaseAmount;  // ����������� ��������
        Debug.Log($"�������� �������� ��������� �� {increaseAmount}. ����� ��������: {moveSpeed}");
    }

    // ����� ��� ���������� �������� (��� ������ ����)
    public void ResetMoveSpeed()
    {
        moveSpeed = baseMoveSpeed;  // ���������� �������� �� �������
        Debug.Log($"�������� �������� �� �������: {moveSpeed}");
    }
}
