using System.Collections;
using UnityEngine;

public class Wizard : Enemy
{
    public GameObject arrowPrefab; // Префаб стрелы
    public Transform shootPoint; // Точка, откуда выпускается стрела
    public float shootCooldown = 3f; // Время между сериями выстрелов

    private bool isShooting = false; // Флаг для проверки, стреляет ли лучник
    private float shootTimer = 1f; // Таймер для отсчета времени до следующей атаки
    private Vector2 randomDirection; // Направление для случайного движения
    private float moveDuration = 2f; // Время движения в одном направлении
    private float changeDirectionTime; // Таймер для смены направления
    protected Rigidbody2D rb; // Изменяем с private на protected

    private WaveManager waveManager;
    private Animator animator; // Для анимаций

    protected override void Start()
    {
        waveManager = GetComponent<WaveManager>();
        base.Start();
        rb = GetComponent<Rigidbody2D>(); // Инициализируем Rigidbody2D
        animator = GetComponent<Animator>(); // Получаем компонент Animator
        randomDirection = GetRandomDirection(); // Генерация случайного направления
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
        if (player == null || isShooting) return; // Проверяем, что игрок существует

        isShooting = true;
        rb.velocity = Vector2.zero; // Останавливаем лучника при стрельбе

        // Поворачиваем лучника к игроку
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        FlipSprite(directionToPlayer); // Поворот по оси X

        animator.SetBool($"Shot1", true); // Используем булевые параметры для управления анимацией

    }

    // Этот метод вызывается через событие в анимации
    public void ShootArrow()
    {
        enemyMoveSpeed = 0;
        // Создаем снаряд
        GameObject projectile = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        WitchsProjectile witchsProjectile = projectile.GetComponent<WitchsProjectile>();

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        FlipSprite(directionToPlayer); // Поворот по оси X

        if (witchsProjectile != null)
        {
            witchsProjectile.SetDirection(directionToPlayer); // Устанавливаем направление
        }

    }

    public void EndShooting()
    {
        animator.SetBool("Shot1", false); // Завершаем анимацию атаки
        isShooting = false; // Разрешаем движение моба

        enemyMoveSpeed = baseEnemyMoveSpeed; // Восстанавливаем скорость движения
        shootTimer = shootCooldown; // Сбрасываем таймер для следующей атаки
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