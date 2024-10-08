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
        // Движение моба в сторону игрока
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // Рассчитать направление на игрока
        moveDir = (player.position - transform.position).normalized;

        // Поворот моба в сторону игрока
        FlipSprite(moveDir);
    }

    void FlipSprite(Vector2 direction)
    {
        // Если игрок находится справа, развернуть моба вправо (по оси X), если слева — влево
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Повернуть вправо
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Повернуть влево
        }
    }
}
