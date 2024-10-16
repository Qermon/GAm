using System.Collections;
using UnityEngine;

public class ArcherEnemy : Enemy
{
    public GameObject arrowPrefab; // Префаб стрелы
    public Transform shootPoint; // Точка, откуда выпускается стрела
    public float shootCooldown = 5f; // Время между сериями выстрелов
    public int arrowsPerShot = 3; // Количество стрел за раз (при необходимости)

    private bool isShooting = false; // Флаг для проверки, стреляет ли лучник
    private float shootTimer = 0f; // Таймер для отсчета времени до следующей атаки
    private Vector2 randomDirection; // Направление для случайного движения
    private float moveDuration = 2f; // Время движения в одном направлении
    private float changeDirectionTime; // Таймер для смены направления
    protected Rigidbody2D rb; // Изменяем с private на protected

    private Animator animator; // Для анимаций
    private float originalMass; // Для хранения оригинальной массы

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>(); // Инициализируем Rigidbody2D
        animator = GetComponent<Animator>(); // Получаем компонент Animator
        randomDirection = GetRandomDirection(); // Генерация случайного направления
        originalMass = rb.mass; // Сохраняем оригинальную массу
        changeDirectionTime = Time.time + moveDuration; // Устанавливаем начальное время для смены направления
    }

    protected override void Update()
    {
        if (isDead) return;

        shootTimer -= Time.deltaTime;

        if (!isShooting && shootTimer <= 0f)
        {
            StartShooting();
            shootTimer = shootCooldown; // Сбрасываем таймер на следующее действие
        }

        if (!isShooting)
        {
            MoveInRandomDirection();
        }
    }

    private void StartShooting()
    {
        if (player == null) return; // Проверяем, что игрок существует

        isShooting = true;
        rb.velocity = Vector2.zero; // Останавливаем лучника при стрельбе

        // Изменяем массу на 100
        rb.mass = 100f;

        // Поворачиваем лучника к игроку
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        FlipSprite(directionToPlayer); // Поворот по оси X

        // Выбираем случайную анимацию атаки
        int randomAnimation = Random.Range(0, 2); // 0 или 1
        if (randomAnimation == 0)
        {
            animator.SetBool("Shot1", true); // Проигрываем первую анимацию
        }
        else
        {
            animator.SetBool("Shot2", true); // Проигрываем вторую анимацию
        }
    }

    // Этот метод вызывается через событие в анимации
    public void ShootArrow()
    {
        if (arrowPrefab != null && shootPoint != null)
        {
            Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
            Debug.Log($"{gameObject.name} выпустил стрелу.");
        }
    }

    // Этот метод вызывается в конце анимации для возврата к обычному движению
    public void EndShooting()
    {
        animator.SetBool("Shot1", false);
        animator.SetBool("Shot2", false);
        isShooting = false; // Заканчиваем стрельбу и продолжаем движение

        // Восстанавливаем оригинальную массу
        rb.mass = originalMass;
    }

    private void MoveInRandomDirection()
    {
        rb.velocity = randomDirection * enemyMoveSpeed;

        // Поворачиваем моба в сторону движения
        if (rb.velocity.magnitude > 0.1f) // Проверяем, движется ли моб
        {
            FlipSprite(randomDirection);
        }

        // Проверяем, пришло ли время смены направления
        if (Time.time >= changeDirectionTime)
        {
            randomDirection = GetRandomDirection();
            changeDirectionTime = Time.time + moveDuration; // Устанавливаем новое время для смены направления
        }
    }

    private Vector2 GetRandomDirection()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    // Метод для поворота спрайта в сторону движения
    protected override void FlipSprite(Vector2 direction)
    {
        if (direction.x > 0)
        {
            // Поворачиваем вправо
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < 0)
        {
            // Поворачиваем влево
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
