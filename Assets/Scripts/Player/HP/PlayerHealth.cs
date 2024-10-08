using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar; // Ссылка на компонент полоски здоровья 

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth); // Установить максимальное здоровье на полоску
        }
        else
        {
            Debug.LogError("HealthBar reference is missing on PlayerHealth!");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Ограничиваем текущее здоровье, чтобы оно не уходило ниже 0
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        // Обновляем UI здоровья (если есть)
        healthBar.SetHealth(currentHealth);
    }

    void Die()
    {
        Debug.Log("Player died!");
        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Если столкнулись с врагом, наносим урон
            TakeDamage(20); // Урон 20, например
        }
    }

    void UpdateHealthUI()
    {
        healthBar.SetHealth(currentHealth);
    }
}

