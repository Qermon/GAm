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

        // Поиск объекта с именем "GoldPlayer" и получение TMP_Text компонента
        GameObject goldPlayerObject = GameObject.Find("GoldPlayer");
        if (goldPlayerObject != null)
        {
            goldText = goldPlayerObject.GetComponent<TMP_Text>();
        }
        else
        {
            Debug.LogError("Объект с именем 'GoldPlayer' не найден на сцене! Убедитесь, что он существует.");
        }

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
        if (!bonusGivenForWave)
        {
            if (playerHealth != null)
            {
                float bonusGold = playerHealth.CalculateInvestmentBonus(currentGold);
                AddGold(Mathf.FloorToInt(bonusGold));
                Debug.Log($"Бонусное золото за инвестиции: {bonusGold}");
            }
            else
            {
                Debug.LogError("PlayerHealth не найден!");
            }

            bonusGivenForWave = true;
        }
    }

    // Метод для закрытия магазина
    public void OnShopClosed()
    {
        AddWaveInvestmentBonus();
    }

    // Метод для начала новой волны
    public void OnNewWaveStarted()
    {
        bonusGivenForWave = false;
    }

    // Метод для покупки в магазине
    public bool PurchaseItem(int price)
    {
        if (currentGold >= price)
        {
            currentGold -= price;
            UpdateGoldDisplay();
            Debug.Log($"Предмет куплен за {price} золота.");
            return true;
        }
        else
        {
            Debug.Log("Недостаточно золота для покупки.");
            return false;
        }
    }

    public void UpdateGoldDisplay()
    {
        if (goldText != null)
        {
            goldText.text = "" + currentGold;
        }
        else
        {
            Debug.LogError("Ссылка на goldText отсутствует!");
        }
    }
}
