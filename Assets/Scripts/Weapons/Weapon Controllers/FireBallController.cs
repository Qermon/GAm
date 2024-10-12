using System.Collections;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    public GameObject fireBallPrefab; // Префаб огненного шара
    public float speed = 10f; // Скорость огненного шара
    public float attackInterval = 1.0f; // Интервал между атаками
    public int fireBallDamage = 10; // Урон огненного шара

    private Vector3 lastMovedVector = Vector3.right; // Последний вектор движения (по умолчанию вправо)

    private void Start()
    {
        StartCoroutine(ShootFireBalls());
    }

    private void Update()
    {
        // Обновляем последний вектор движения игрока
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            lastMovedVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).normalized;
        }
    }

    private IEnumerator ShootFireBalls()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval); // Ждем перед следующим выстрелом
            ShootFireBall();
        }
    }

    private void ShootFireBall()
    {
        if (lastMovedVector != Vector3.zero) // Проверяем, есть ли движение
        {
            GameObject spawnedFireBall = Instantiate(fireBallPrefab, transform.position, Quaternion.identity);
            FireBallBehaviour fireBallBehaviour = spawnedFireBall.AddComponent<FireBallBehaviour>(); // Добавляем поведение огненного шара
            fireBallBehaviour.SetDirection(lastMovedVector, speed, fireBallDamage); // Устанавливаем направление, скорость и урон
        }
    }
}

public class FireBallBehaviour : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private int damage; // Урон огненного шара

    private void Start()
    {
        Destroy(gameObject, 5f); // Уничтожаем огненный шар через 5 секунд, если он не столкнулся с чем-то
    }

    private void Update()
    {
        // Двигаем огненный шар по направлению с учётом скорости
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 newDirection, float fireBallSpeed, int fireBallDamage)
    {
        direction = newDirection.normalized; // Нормализуем направление
        speed = fireBallSpeed;
        damage = fireBallDamage; // Устанавливаем урон

        // Поворачиваем огненный шар в сторону движения
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Вычисляем угол
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Поворачиваем огненный шар
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage(damage); // Наносим урон врагу

            // Здесь можно добавить логику для визуального эффекта или звука
            // Например, показать частицы или воспроизвести звук удара

            // Если вы хотите, чтобы огненный шар пролетел сквозь врага, уберите строку Destroy
            // Но при этом нужно убедиться, что у врагов нет коллайдеров, которые могли бы мешать движению шарика
        }

    }
}
