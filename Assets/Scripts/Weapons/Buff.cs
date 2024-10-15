using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public float lifetime = 2f; // Время жизни бафа
    public float damageMultiplier = 2f; // Множитель урона (100% увеличение)
    private HashSet<Enemy> affectedEnemies = new HashSet<Enemy>(); // Для хранения врагов в зоне действия

    private void Start()
    {
        // Запускаем корутину для удаления бафа
        StartCoroutine(DestroyAfterTime(lifetime));
    }

    private void ApplyBuffToEnemies()
    {
        foreach (var enemy in affectedEnemies)
        {
            // Запоминаем исходный урон для отладки
            float originalDamage = enemy.damage;
            // Увеличиваем урон врага
            enemy.SetDamage(originalDamage * damageMultiplier);
            Debug.Log($"Buff applied: {enemy.gameObject.name} damage increased from {originalDamage} to {enemy.damage}");

            // Запускаем корутину для удаления эффекта после времени жизни
            StartCoroutine(RemoveBuffAfterTime(enemy, lifetime, originalDamage));
        }
    }

    private IEnumerator RemoveBuffAfterTime(Enemy enemy, float time, float originalDamage)
    {
        yield return new WaitForSeconds(time); // Ждем заданное время

        // Сбрасываем урон врага
        enemy.SetDamage(originalDamage);
        Debug.Log($"Buff removed: {enemy.gameObject.name} damage reset to {enemy.damage}");
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
                Debug.Log($"{enemy.gameObject.name} entered buff zone.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Проверяем, если враг покинул зону действия
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                affectedEnemies.Remove(enemy); // Убираем врага из коллекции
                Debug.Log($"{enemy.gameObject.name} exited buff zone.");
            }
        }
    }

    public IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time); // Ждем заданное время
        ApplyBuffToEnemies(); // Применяем баф перед уничтожением
        Destroy(gameObject); // Удаляем объект бафа
    }
}
