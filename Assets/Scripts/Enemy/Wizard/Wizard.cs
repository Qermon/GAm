using System.Collections;
using UnityEngine;

public class Wizard : Enemy
{
    public Animator animator; // Ссылка на компонент Animator
    public GameObject projectilePrefab; // Префаб снаряда
    public Transform firePoint; // Точка, откуда выпускается снаряд

    public float safeDistance = 5f; // Дистанция, на которой маг держится от игрока
    public float moveDuration = 2f; // Время движения в одном направлении
    private float lastAttackTime = 0f; // Время последней атаки
    private bool isAttacking = false; // Флаг для проверки состояния атаки
    private Vector2 randomDirection; // Направление для случайного движения
    private float changeDirectionTime; // Таймер для смены направления
    private Rigidbody2D rb; // Rigidbody для управления движением
    private float originalMass; // Исходная масса моба

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>(); // Получаем компонент Animator
        rb = GetComponent<Rigidbody2D>(); // Инициализируем Rigidbody2D
        originalMass = rb.mass; // Сохраняем оригинальную массу
        randomDirection = GetRandomDirection(); // Генерация случайного направления
        changeDirectionTime = Time.time + moveDuration; // Устанавливаем начальное время для смены направления
    }

    protected override void Update()
    {
        base.Update();

        if (isDead || player == null) return;

        // Проверяем дистанцию до игрока
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Если игрок далеко, двигаемся в случайном направлении
        if (distanceToPlayer > safeDistance && !isAttacking)
        {
            MoveInRandomDirection();
            animator.SetFloat("moveSpeed", enemyMoveSpeed); // Анимация движения
        }
        else
        {
            // Останавливаем движение и атакуем
            animator.SetFloat("moveSpeed", 0f); // Останавливаем анимацию движения

            // Условие для атаки
            if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
            {
                StartAttack(); // Запуск атаки
            }
        }
    }

    private void StartAttack()
    {
        lastAttackTime = Time.time; // Обновляем время последней атаки
        isAttacking = true; // Устанавливаем флаг атаки
        animator.SetBool("isAttacking", true); // Запуск анимации атаки

        // Поворачиваем мага к игроку перед атакой
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        FlipSprite(directionToPlayer); // Поворот мага по направлению к игроку
    }

    // Метод, вызываемый событием анимации для выпуска снаряда
    public void ShootProjectile()
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity); // Используем firePoint
        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        Vector2 directionToPlayer = (player.transform.position - firePoint.position).normalized; // Используем firePoint для направления
        projectile.SetDirection(directionToPlayer); // Устанавливаем направление снаряда
    }

    // Метод, вызываемый в конце анимации атаки
    public void FinishAttack()
    {
        isAttacking = false; // Сбрасываем флаг атаки после завершения анимации
        animator.SetBool("isAttacking", false); // Остановка анимации атаки
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
