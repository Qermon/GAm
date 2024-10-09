using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public int hitCount = 0; // Счетчик попаданий
    public int maxHits = 5; // Максимальное количество попаданий до уничтожения
    public int damage = 20; // Урон, который наносит шурикен
    public float attackInterval = 1.0f; // Интервал между атаками на одного врага

    private Dictionary<Collider2D, float> lastAttackTime = new Dictionary<Collider2D, float>(); // Словарь для отслеживания времени последней атаки

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (CanAttack(collision)) // Проверяем, может ли атаковать
            {
                hitCount++; // Увеличиваем счетчик попаданий только для этого шурикена
                Debug.Log($"Shuriken hit enemy: {hitCount}");
                Debug.Log($"Shuriken {this.GetInstanceID()} hit enemy: {hitCount}"); // ID шурикена

                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage); // Наносим урон
                }

                lastAttackTime[collision] = Time.time; // Запоминаем время атаки

                if (hitCount >= maxHits) // Проверяем, достиг ли шурикен максимума
                {
                    DestroyShuriken(); // Уничтожаем шурикен
                }
            }
        }
    }

    private bool CanAttack(Collider2D enemy)
    {
        if (!lastAttackTime.ContainsKey(enemy)) // Если это первый удар по врагу
        {
            lastAttackTime[enemy] = Time.time; // Запоминаем текущее время
            return true;
        }

        // Проверяем, прошло ли достаточно времени с последнего удара
        return Time.time >= lastAttackTime[enemy] + attackInterval;
    }

    private void DestroyShuriken()
    {
        // Уведомляем менеджер о разрушении шурикена
        ShurikenManager manager = FindObjectOfType<ShurikenManager>();
        if (manager != null)
        {
            manager.OnShurikenDestroyed(this); // Уведомляем менеджер о разрушении
        }

        // Уничтожаем шурикен только в этом методе
        Destroy(gameObject);
    }
}
