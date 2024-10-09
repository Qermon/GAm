using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int health;
    private const int maxHealth = 100;

    public void Initialize()
    {
        health = maxHealth; // Установите здоровье на максимальное значение
        Debug.Log("Игрок инициализирован! Здоровье: " + health);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0; // Здоров'я не може бути менше нуля
        }
        Console.WriteLine($"Гравець отримав {damage} ушкоджень. Здоров'я: {health}");
    }

    public bool IsAlive()   
    {
        return health > 0; // Перевірка, чи живий гравець
    }
}
