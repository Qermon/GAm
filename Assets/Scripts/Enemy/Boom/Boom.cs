using System.Collections;
using UnityEngine;

public class Boom : Enemy
{
    public float explosionRadius = 0.5f;   // Радиус отталкивания врагов
    public float detectionRadius = 2f;     // Радиус, в котором моб остановится перед взрывом
    public float explosionDelay = 2f;      // Время ожидания перед взрывом
    public int explosionDamage = 50;       // Урон от взрыва

    private bool isExploding = false;      // Проверка, начался ли взрыв
    private Animator animator;             // Ссылка на Animator

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();  // Получаем компонент Animator
    }

    protected override void Update()
    {
        if (isDead || isExploding || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Если моб находится в радиусе взрыва, начинается зарядка взрыва
        if (distanceToPlayer <= detectionRadius)
        {
            StartCoroutine(Explode());
        }
        else
        {
            // Движение к игроку, если он вне радиуса взрыва
            animator.SetBool("isMoving", true);  // Активируем анимацию движения
            base.MoveTowardsPlayer();
        }
    }

    // Метод для взрыва
    private IEnumerator Explode()
    {
        isExploding = true;

        // Останавливаем движение и включаем анимацию Idle
        animator.SetBool("isMoving", false);
        animator.SetTrigger("Idle");

        // Ожидание перед взрывом
        yield return new WaitForSeconds(explosionDelay);

        // Активируем анимацию взрыва
        animator.SetTrigger("ExplodeTrigger");

        // Ожидаем, пока анимация взрыва завершится (пример: 0.5 сек, зависит от длины анимации)
        yield return new WaitForSeconds(0.5f); // Подкорректируй длительность ожидания под свою анимацию

        // Нанесение урона и отталкивание врагов
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D obj in hitObjects)
        {
            if (obj.CompareTag("Enemy") || obj.CompareTag("Player"))
            {
                // Нанесение урона
                obj.GetComponent<Enemy>()?.TakeDamage(explosionDamage);
                obj.GetComponent<PlayerHealth>()?.TakeDamage(explosionDamage);

                // Отталкивание объекта
                Vector2 direction = (obj.transform.position - transform.position).normalized;
                obj.GetComponent<Rigidbody2D>()?.AddForce(direction * 100f); // Отталкивание
            }
        }

        // Смерть моба после взрыва
        Die();
    }

    // Метод для визуализации радиуса взрыва в редакторе Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius); // Радиус взрыва
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // Радиус детекции
    }
}
