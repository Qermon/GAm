using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : Weapon
{
    public int hitCount = 0; // Счетчик попаданий
    public int maxHits = 5;  // Максимум попаданий до уничтожения
    public float attackInterval = 1.0f; // Интервал между атаками

    private Dictionary<Collider2D, float> lastAttackTime = new Dictionary<Collider2D, float>(); // Для отслеживания времени атаки

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (CanAttack(collision)) // Проверяем, можем ли атаковать
            {
               
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage); // Наносим урон
                }

                lastAttackTime[collision] = Time.time; // Запоминаем время атаки

                if (hitCount >= maxHits) // Если достигли лимита
                {
                    DestroyShuriken();
                }
            }
        }
    }

    private bool CanAttack(Collider2D enemy)
    {
        if (!lastAttackTime.ContainsKey(enemy))
        {
            lastAttackTime[enemy] = Time.time;
            return true;
        }

        return Time.time >= lastAttackTime[enemy] + attackInterval; // Проверяем интервал
    }

    private void DestroyShuriken()
    {
        Destroy(gameObject); // Уничтожаем шурикен
    }
}
