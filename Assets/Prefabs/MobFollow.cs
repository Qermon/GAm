using UnityEngine;

public class MobFollow : MonoBehaviour
{
    public Transform player; // Ссылка на объект игрока
    public float speed = 3f; // Скорость моба
    public float stoppingDistance = 1.5f; // Дистанция, на которой моб остановится

    void Update()
    {
        if (player != null)
        {
            // Вычисляем дистанцию между мобом и игроком
            float distance = Vector2.Distance(transform.position, player.position);

            // Если дистанция больше, чем stoppingDistance, моб движется к игроку
            if (distance > stoppingDistance)
            {

                
                // Направление к игроку
                Vector2 direction = (player.position - transform.position).normalized;

                Vector3 playerPosition = player.position;  // Позиция игрока
                transform.position = Vector2.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);

              
            }
        }
    }
}