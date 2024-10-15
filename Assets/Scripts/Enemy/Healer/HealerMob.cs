using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerMob : Enemy
{
    public GameObject buffPrefab; // Префаб бафа, который будет спавниться
    public float attackAnimationDuration = 2f; // Длительность анимации атаки
    public float buffSpawnCooldown = 5f; // Время между атаками
    private Animator animator; // Компонент Animator

    private bool isCasting = false; // Переменная для контроля состояния кастинга

    protected override void Start()
    {
        base.Start(); // Вызов метода Start() из класса Enemy
        animator = GetComponent<Animator>(); // Получаем компонент Animator
        StartCoroutine(AttackRoutine()); // Запускаем корутину для атаки
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            // Бежать к игроку
            MoveTowardsPlayer();

            // Ждать перед атакой
            yield return new WaitForSeconds(buffSpawnCooldown);

            // Проигрывать анимацию атаки
            Attack();

            // Останавливаемся на время анимации атаки
            yield return new WaitForSeconds(attackAnimationDuration);

            // Спавнить баф только один раз после атаки
            SpawnBuff();
        }
    }

    private void Attack()
    {
        if (!isCasting) // Проверяем, что сейчас не идет каст
        {
            isCasting = true; // Устанавливаем флаг кастинга
            animator.SetBool("isCasting", true); // Установка булевой переменной для анимации
            StopMoving(); // Останавливаем движение

            // Восстановление состояния после завершения атаки
            StartCoroutine(ResetCastingState());
        }
    }

    private void StopMoving()
    {
        enemyMoveSpeed = 0; // Останавливаем движение
    }

    private IEnumerator ResetCastingState()
    {
        yield return new WaitForSeconds(attackAnimationDuration); // Ждем завершения анимации
        animator.SetBool("isCasting", false); // Сбрасываем булевую переменную
        isCasting = false; // Сбрасываем флаг кастинга
        enemyMoveSpeed = 1f; // Восстанавливаем скорость движения (замените на вашу стандартную скорость)
    }

    private void SpawnBuff()
    {
        // Спавн бафа
        GameObject buff = Instantiate(buffPrefab, transform.position, Quaternion.identity);
        Buff buffScript = buff.GetComponent<Buff>(); // Получаем компонент скрипта бафа
        if (buffScript != null)
        {
            StartCoroutine(buffScript.DestroyAfterTime(2f)); // Удаляем баф через 5 секунд
        }
    }

    // Метод для движения к игроку (наследуется от Enemy)
    protected override void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemyMoveSpeed * Time.deltaTime);
            FlipSprite(direction); // Метод для поворота спрайта
        }
    }
}
