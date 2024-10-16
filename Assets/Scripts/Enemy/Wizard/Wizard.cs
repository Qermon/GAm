using System.Collections;
using UnityEngine;

public class Wizard : Enemy
{
    public Animator animator; // Ссылка на компонент Animator
    public GameObject projectilePrefab; // Префаб снаряда
    public Transform firePoint; // Точка, откуда выпускается снаряд

    public float safeDistance = 5f; // Дистанция, на которой маг держится от игрока

    private float lastAttackTime = 0f; // Время последней атаки
    private bool isAttacking = false; // Флаг для проверки состояния атаки

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>(); // Получаем компонент Animator
    }

    protected override void Update()
    {
        base.Update();

        if (isDead || player == null) return;

        // Проверяем дистанцию до игрока
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > safeDistance)
        {
            MoveAwayFromPlayer();
            animator.SetFloat("moveSpeed", enemyMoveSpeed); // Запуск анимации бега
        }
        else
        {
            animator.SetFloat("moveSpeed", 0f); // Остановка анимации движения

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

    // Остановка движения
    private void StopMoving()
    {
        enemyMoveSpeed = 0f; // Полная остановка мага
        animator.SetFloat("moveSpeed", 0f); // Остановка анимации движения
    }

    // Метод для движения в сторону от игрока
    protected void MoveAwayFromPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (transform.position - player.position).normalized; // Вычисляем направление
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)direction, enemyMoveSpeed * Time.deltaTime);
            FlipSprite(direction); // Метод для поворота спрайта
        }
    }
}
