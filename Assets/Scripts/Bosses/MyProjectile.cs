using System.Collections;
using UnityEngine;

public class MyProjectile : MonoBehaviour
{
    public float speed = 5f; // Скорость снаряда
    public Vector2 direction; // Направление движения

    // Метод для инициализации снаряда
    public void Initialize(float speed, Vector2 direction)
    {
        this.speed = speed;
        this.direction = direction;

        // Запускаем корутину для движения снаряда
        StartCoroutine(MoveProjectile());
    }

    private IEnumerator MoveProjectile()
    {
        while (true) // Условие для уничтожения снаряда
        {
            transform.position += (Vector3)(direction.normalized * speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Обработка столкновения с игроком
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10); // Наносим 10 урона (или другое значение)
            }
            Destroy(gameObject); // Уничтожаем снаряд
        }
        // Добавьте обработку столкновения с другими объектами, если нужно
    }
}
