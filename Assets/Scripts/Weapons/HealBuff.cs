using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBuff : MonoBehaviour
{
    public float lifetime = 2f; // Время жизни бафа
    public float healPercentage = 0.2f; // Процент восстановления (20%)
    private HashSet<Enemy> affectedEnemies = new HashSet<Enemy>(); // Для хранения врагов, попавших в триггер

    private void Start()
    {
        // Запускаем корутину для удаления бафа
        StartCoroutine(DestroyAfterTime(lifetime));
    }

    private void ApplyHealToEnemies()
    {
        // Применяем эффект бафа к врагам, находящимся в триггере
        foreach (var enemy in affectedEnemies)
        {
            float healAmount = enemy.maxHealth * healPercentage; // Рассчитываем количество восстановленного здоровья
            enemy.Heal(healAmount); // Восстанавливаем здоровье врага
            Debug.Log($"Buff applied: {enemy.gameObject.name} healed for {healAmount}. Current Health: {enemy.currentHealth}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, если столкновение с объектом является врагом
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                affectedEnemies.Add(enemy); // Добавляем врага в коллекцию
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Убираем врага из коллекции, когда он покидает триггер
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                affectedEnemies.Remove(enemy); // Убираем врага из коллекции
            }
        }
    }

    public IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time); // Ждем заданное время
        ApplyHealToEnemies(); // Применяем восстановление перед уничтожением
        Destroy(gameObject); // Удаляем объект бафа
    }
}