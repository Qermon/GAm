using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Controls all player movement
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float baseMoveSpeed;
    Rigidbody2D rb;

    [HideInInspector]
    public Vector2 moveDir;

    // Сохраняем состояния
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 lastMovedVector;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f); // Инициализация для корректной работы оружия
    }

    void Update()
    {
        InputManagement();
    }

    void FixedUpdate() // Физика в FixedUpdate
    {
        Move();
    }

    void InputManagement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized; // Ограничиваем скорость на диагоналях

        if (moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f);    // Последнее движение по X
        }

        if (moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector);  // Последнее движение по Y
        }

        if (moveDir.x != 0 && moveDir.y != 0)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);    // При движении
        }
    }

    // Метод для увеличения скорости движения
    public void IncreaseMoveSpeed(float percentage)
    {
        float increaseAmount = baseMoveSpeed * percentage;
        moveSpeed += increaseAmount;// Увеличиваем скорость движения
        Debug.Log($"Скорость движения увеличена на {increaseAmount}%. Новая скорость: {moveSpeed}");
    }

    void Move()
    {
        rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);    // Применяем скорость
    }
}