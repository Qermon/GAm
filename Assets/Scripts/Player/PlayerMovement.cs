using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;            // Текущая скорость движения
    public float baseMoveSpeed;        // Базовая скорость, которую можно увеличивать
    private Rigidbody2D rb;            // Rigidbody персонажа для применения скорости
    public Vector2 moveDir;
    [HideInInspector]
    public float lastHorizontalVector;


    private Vector2 startTouchPosition;  // Начальная позиция касания
    private Vector2 currentTouchPosition; // Текущая позиция пальца
    private Vector2 direction;          // Направление движения

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // Получаем компонент Rigidbody2D
        moveSpeed = baseMoveSpeed;         // Устанавливаем начальную скорость
    }

    void Update()
    {
        HandleTouchInput();  // Обрабатываем ввод с экрана
    }

    void FixedUpdate()
    {
        Move();  // Двигаем персонажа
    }

    // Метод для обработки касания
    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            // Получаем информацию о первом касании
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Сохраняем начальную позицию касания
                startTouchPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                // Обновляем текущую позицию пальца
                currentTouchPosition = touch.position;

                // Вычисляем направление, в котором палец перемещается
                direction = currentTouchPosition - startTouchPosition;
                moveDir = direction.normalized;  // Нормализуем направление для равномерной скорости

                // Сохраняем последнее горизонтальное движение
                lastHorizontalVector = moveDir.x;
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                // Когда палец отпущен, сбрасываем направление
                moveDir = Vector2.zero;
            }
        }
    }

    // Метод для движения персонажа
    void Move()
    {
        rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);  // Применяем скорость
    }

    // Метод для увеличения скорости
    public void IncreaseMoveSpeed(float percentage)
    {
        // Увеличиваем скорость на определенный процент от базовой скорости
        float increaseAmount = baseMoveSpeed * percentage;
        moveSpeed += increaseAmount;  // Увеличиваем скорость
        Debug.Log($"Скорость движения увеличена на {increaseAmount}. Новая скорость: {moveSpeed}");
    }

    // Метод для уменьшения скорости (для сброса бафа)
    public void ResetMoveSpeed()
    {
        moveSpeed = baseMoveSpeed;  // Сбрасываем скорость на базовую
        Debug.Log($"Скорость сброшена на базовую: {moveSpeed}");
    }
}
