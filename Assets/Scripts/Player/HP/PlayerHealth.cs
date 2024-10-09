using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar; // Ссылка на компонент полоски здоровья 
    public Animator animator; // Ссылка на компонент Animator

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

        // Убедитесь, что ссылка на Animator задана
        if (animator == null)
        {
            Debug.LogError("Animator reference is missing on PlayerHealth!");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Ограничиваем текущее здоровье, чтобы оно не уходило ниже 0
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Die()); // Запускаем корутину для смерти
        }

        // Обновляем UI здоровья (если есть)
        UpdateHealthUI();
    }

    // Теперь метод Die возвращает IEnumerator
    private IEnumerator Die()
    {
        Debug.Log("Player died!");
        // Отключаем объект сразу после вызова метода
        gameObject.SetActive(false); // Или используйте Destroy(gameObject), если хотите удалить объект

        yield return null; // Убедитесь, что метод возвращает значение
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
