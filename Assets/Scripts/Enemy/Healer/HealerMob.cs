using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerMob : Enemy
{
    public GameObject healAnimationPrefab; // Префаб анимации хила
    public float healRadius = 3f; // Радиус для поиска врагов
    public float healAmountPercent = 0.2f; // 20% от максимального здоровья
    public float healInterval = 5f; // Интервал хила в секундах
    public float castTime = 2f; // Время кастования

    private Animator animator;
    private float originalMoveSpeed;
    private float lastHealTime = 0f; // Время последнего хила
    private bool isCasting = false; // Статус кастования

    protected override void Start()
    {
        base.Start(); // Вызов метода Start() из Enemy
        animator = GetComponent<Animator>();
        originalMoveSpeed = enemyMoveSpeed; // Инициализация исходной скорости
    }

    protected override void Update()
    {
        base.Update();

        if (Time.time >= lastHealTime + healInterval && !isCasting)
        {
            StartCoroutine(CastHeal()); // Запускаем корутину
        }
    }

    private IEnumerator CastHeal()
    {
        isCasting = true; // Моб начинает кастовать
        animator.SetBool("isCasting", true); // Установка параметра в Animator

        // Останавливаем моба
        enemyMoveSpeed = 0f; // Остановка движения моба

        // Ждем 0.5 секунды перед созданием анимации хила
        yield return new WaitForSeconds(0.5f);

        // Создание экземпляра префаба анимации хила
        GameObject healAnimation = null;
        if (healAnimationPrefab != null)
        {
            healAnimation = Instantiate(healAnimationPrefab, transform.position, Quaternion.identity);
        }

        // Ждем время каста
        yield return new WaitForSeconds(castTime - 0.5f); // Вычитаем 0.5 секунды из общего времени каста

        // Хилим врагов после завершения каста
        HealEnemies();

        // После завершения анимации и хила, восстанавливаем движение
        enemyMoveSpeed = originalMoveSpeed; // Восстанавливаем исходную скорость
        lastHealTime = Time.time; // Обновляем время последнего хила
        isCasting = false; // Завершаем каст
        animator.SetBool("isCasting", false); // Возвращаем параметр в Animator

        // Удаляем анимацию хила после завершения
        if (healAnimation != null)
        {
            Destroy(healAnimation); // Удаляем анимацию хила
        }
    }


    private void HealEnemies()
    {
        Collider2D[] enemiesToHeal = Physics2D.OverlapCircleAll(transform.position, healRadius);

        foreach (Collider2D enemyCollider in enemiesToHeal)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null && !enemy.IsDead) // Проверка на мертвого врага
                {
                    float healAmount = enemy.maxHealth * healAmountPercent; // Рассчитываем количество лечения

                    // Вывод информации о здоровье до хила
                    Debug.Log($"{enemy.gameObject.name} - HP до хила: {enemy.currentHealth}"); // HP до хила

                    // Выполняем лечение
                    enemy.Heal(healAmount); // Вызов метода хила

                    // Теперь, чтобы получить актуальное значение здоровья, нужно заново вызвать его из enemy
                    Debug.Log($"{enemy.gameObject.name} - Хил: {healAmount}, HP теперь: {enemy.currentHealth}"); // HP после хила
                }
            }
        }
    }


    // Метод для отрисовки радиуса в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healRadius);
    }
}
