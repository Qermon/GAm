using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int health;
    private const int maxHealth = 100;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        health = maxHealth; // Устанавливаем здоровье на максимальное значение
        Debug.Log("Игрок инициализирован! Здоровье: " + health);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0; // Здоровье не может быть меньше нуля
        }
        Debug.Log($"Игрок получил {damage} урона. Текущее здоровье: {health}");
    }

    public bool IsAlive()
    {
        return health > 0; // Проверяем, жив ли игрок
    }

    // Увеличение максимального здоровья
    public void IncreaseMaxHealth(int amount)
    {
        health += amount;
        Debug.Log("Макс. здоровье увеличено на " + amount + ". Текущее здоровье: " + health);
    }

    // Увеличение урона всех оружий игрока
    public void IncreaseWeaponDamage(int amount)
    {
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            weapon.IncreaseDamage(amount);
        }
        Debug.Log("Урон всех оружий увеличен на " + amount);
    }
}
