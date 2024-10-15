using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffMob : Enemy
{
    public GameObject healAnimationPrefab; // Префаб анимации урона
    public float buffRadius = 3f; // Радиус для поиска врагов
    public float damageBonusPercent = 0.5f; // 50% бонус к урону
    public float buffInterval = 5f; // Интервал бафа в секундах
    public float castTime = 2f; // Время кастования
    public float buffDuration = 10f; // Длительность бафа в секундах

    private Animator animator;
    private float originalMoveSpeed;
    private float lastBuffTime = 0f; // Время последнего бафа
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

        if (Time.time >= lastBuffTime + buffInterval && !isCasting)
        {
            StartCoroutine(CastBuff()); // Запускаем корутину
        }
    }

    private IEnumerator CastBuff()
    {
        isCasting = true; // Моб начинает кастовать
        animator.SetBool("isCasting", true); // Установка параметра в Animator

        // Останавливаем моба
        enemyMoveSpeed = 0f; // Остановка движения моба

        // Ждем 0.5 секунды перед созданием анимации бафа
        yield return new WaitForSeconds(0.5f);

        

        GameObject healAnimation = null;
        if (healAnimationPrefab != null)
        {
            healAnimation = Instantiate(healAnimationPrefab, transform.position, Quaternion.identity);
        }

        // Ждем время каста
        yield return new WaitForSeconds(castTime - 0.5f); // Вычитаем 0.5 секунды из общего времени каста

        // Бафаем врагов после завершения каста
        BuffEnemies();

        // После завершения анимации и бафа, восстанавливаем движение
        enemyMoveSpeed = originalMoveSpeed; // Восстанавливаем исходную скорость
        lastBuffTime = Time.time; // Обновляем время последнего бафа
        isCasting = false; // Завершаем каст
        animator.SetBool("isCasting", false); // Возвращаем параметр в Animator

        // Удаляем анимацию хила после завершения
        if (healAnimation != null)
        {
            Destroy(healAnimation); // Удаляем анимацию хила
        }
        // Destroy(buffAnimation); // Uncomment if you have a buff animation prefab
    }

    private void BuffEnemies()
    {
        Collider2D[] alliesToBuff = Physics2D.OverlapCircleAll(transform.position, buffRadius);

        foreach (Collider2D allyCollider in alliesToBuff)
        {
            if (allyCollider.CompareTag("Enemy")) // Замените на ваш тег союзников
            {
                Enemy enemy = allyCollider.GetComponent<Enemy>();
                if (enemy != null && !enemy.IsDead) // Проверка на мертвого врага
                {
                    float originalDamage = enemy.damage; // Сохраняем оригинальный урон
                    float buffedDamage = originalDamage * (1 + damageBonusPercent); // Рассчитываем увеличенный урон

                    enemy.SetDamage(buffedDamage); // Устанавливаем новый урон (добавьте метод SetDamage в Enemy)

                    // Вывод информации о уроне
                    Debug.Log($"{enemy.gameObject.name} - Бонус к урону: {buffedDamage}, Оригинальный урон: {originalDamage}");

                    // Запускаем корутину для восстановления оригинального урона
                    StartCoroutine(RestoreOriginalDamage(enemy, originalDamage, buffDuration));
                }
            }
        }
    }

    private IEnumerator RestoreOriginalDamage(Enemy enemy, float originalDamage, float duration)
    {
        yield return new WaitForSeconds(duration); // Ждем 10 секунд

        enemy.SetDamage(originalDamage); // Восстанавливаем оригинальный урон
        Debug.Log($"{enemy.gameObject.name} - Урон восстановлен: {originalDamage}");
    }

    // Метод для отрисовки радиуса в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue; // Цвет радиуса
        Gizmos.DrawWireSphere(transform.position, buffRadius);
    }
}
