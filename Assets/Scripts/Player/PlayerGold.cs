using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Для использования TMP_Text

public class PlayerGold : MonoBehaviour
{
    public int currentGold = 0; // Текущее количество золота
    public TMP_Text goldText; // Ссылка на UI Text элемент для отображения золота
    private PlayerHealth playerHealth;
    private bool bonusGivenForWave = false; // Флаг, отслеживающий начисление бонуса за текущую волну

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>(); // Получаем ссылку на PlayerHealth
        UpdateGoldDisplay(); // Инициализируем отображение золота в начале
    }

    // Добавляем золото
    public void AddGold(int amount)
    {
        currentGold += amount; // Увеличиваем золото
        UpdateGoldDisplay(); // Обновляем отображение золота
    }

    // Начисление бонуса после закрытия магазина
    public void AddWaveInvestmentBonus()
    {
        // Проверяем, было ли золото уже начислено за эту волну
        if (!bonusGivenForWave)
        {
            if (playerHealth != null)
            {
                float bonusGold = playerHealth.CalculateInvestmentBonus(currentGold); // Рассчитываем бонусное золото
                AddGold(Mathf.FloorToInt(bonusGold)); // Добавляем его к текущему золоту
                Debug.Log($"Бонусное золото за инвестиции: {bonusGold}");
            }
            else
            {
                Debug.LogError("PlayerHealth не найден!");
            }

            // Устанавливаем флаг, чтобы не начислять золото снова
            bonusGivenForWave = true;
        }
    }

    // Метод для закрытия магазина
    public void OnShopClosed()
    {
        AddWaveInvestmentBonus(); // Начисляем бонус за инвестиции после закрытия магазина
    }

    // Метод для начала новой волны
    public void OnNewWaveStarted()
    {
        bonusGivenForWave = false; // Сбрасываем флаг для новой волны
    }

    // Метод для покупки в магазине
    public bool PurchaseItem(int price)
    {
        if (currentGold >= price)
        {
            currentGold -= price; // Уменьшаем золото
            UpdateGoldDisplay(); // Обновляем отображение золота
            Debug.Log($"Предмет куплен за {price} золота.");
            return true; // Покупка успешна
        }
        else
        {
            Debug.Log("Недостаточно золота для покупки.");
            return false; // Недостаточно золота
        }
    }

    private void UpdateGoldDisplay()
    {
        // Обновляем текст золота в UI только если ссылка на текст существует
        if (goldText != null)
        {
            goldText.text ="" + currentGold;
        }
        else
        {
            Debug.LogError("Ссылка на goldText отсутствует!");
        }
    }
}
