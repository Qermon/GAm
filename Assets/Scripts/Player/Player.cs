using UnityEngine;

public class Player : MonoBehaviour
{
    public int health;
    public int maxHealth = 100; // Теперь maxHealth может изменяться

    // Инициализация игрока
    public void Initialize()
    {
        health = maxHealth; // Устанавливаем текущее здоровье на максимальное
        Debug.Log("Игрок инициализирован! Здоровье: " + health);
    }

    // Метод для получения урона
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0; // Здоровье не может быть ниже нуля
        }
        Debug.Log($"Игрок получил {damage} урона. Текущее здоровье: {health}");
    }

    // Проверка на живучесть игрока
    public bool IsAlive()
    {
        return health > 0;
    }

    // Увеличение максимального здоровья
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        health = maxHealth; // Восстанавливаем текущее здоровье до нового максимума
        Debug.Log($"Максимальное здоровье увеличено до: {maxHealth}");
    }
}
