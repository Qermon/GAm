using System.Collections;
using UnityEngine;

public class Boom : Enemy
{
    private bool isPreparingToExplode = false;
    private bool isTriggeredToExplode = false;
    public float explosionPreparationTime = 1.5f;
    public float explosionRadius = 2f;

    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        if (isDead || player == null) return;

        if (!isPreparingToExplode)
        {
            MoveTowardsPlayer();

            // Проверка радиуса до игрока
            if (Vector2.Distance(transform.position, player.position) <= 1f)
            {
                StartExplosionPreparation();
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0 && !isPreparingToExplode)
        {
            StartExplosionPreparation();
        }
    }

    private void StartExplosionPreparation()
    {
        if (isPreparingToExplode) return;

        isPreparingToExplode = true;
        enemyMoveSpeed = 0; // Остановим движение моба

        // Включаем анимацию подготовки
        animator.SetBool("isPreparingToExplode", true);
        StartCoroutine(PrepareForExplosion());
    }

    private IEnumerator PrepareForExplosion()
    {
        yield return new WaitForSeconds(explosionPreparationTime);

        if (isDead && isTriggeredToExplode)
        {
            Explode();
        }
        else if (!isDead)
        {
            isTriggeredToExplode = true;
            Explode();
        }
    }

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)damage);
                }
            }
        }

        // Запускаем анимацию взрыва перед удалением объекта
        animator.SetTrigger("Explode");

        // Удаление объекта после завершения анимации
        Destroy(gameObject, 0.5f); // Время задержки соответствует длительности анимации взрыва
    }
    private void DestroySelf()
    {
        Destroy(gameObject);
    }


    protected override void Die()
    {
        isDead = true;
        animator.SetBool("isDead", true); // Устанавливаем флаг для анимации взрыва
        base.Die();  // Вызов базового метода Die для начисления золота, опыта и т.д.
    }

    protected override void MoveTowardsPlayer()
    {
        if (!isPreparingToExplode && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemyMoveSpeed * Time.deltaTime);
            FlipSprite(direction);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Отображение радиуса взрыва в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
