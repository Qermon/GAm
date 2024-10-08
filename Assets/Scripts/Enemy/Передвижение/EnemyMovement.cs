using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Transform player; // Ссылка на объект игрока
    public float moveSpeed; // Скорость передвижения моба
    private Vector2 moveDir; // Направление движения моба
    private float lastHorizontalVector;

    // Start is called before the first frame update
    void Start()
    {
        // Найти объект игрока с компонентом PlayerMovement
        player = FindObjectOfType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Рассчитать направление на игрока
        moveDir = (player.position - transform.position).normalized;

        // Поворот моба в сторону игрока
        FlipSprite(moveDir);

        // Движение моба в сторону игрока
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }


    void FlipSprite(Vector2 direction)
    {
        // Получаем текущий масштаб
        Vector3 currentScale = transform.localScale;

        // Если игрок находится справа, развернуть моба вправо (по оси X), если слева — влево
        if (direction.x > 0)
        {
            // Устанавливаем масштаб по X в 1 (или сохраняем значение по Y и Z)
            transform.localScale = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
        }
        else if (direction.x < 0)
        {
            // Устанавливаем масштаб по X в -1 (или сохраняем значение по Y и Z)
            transform.localScale = new Vector3(-Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
        }
    }
}

