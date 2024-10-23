using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Для использования TMP_Text

public class PlayerGold : MonoBehaviour
{
    public int currentGold = 0; // Текущее количество золота
    public TMP_Text goldText; // Ссылка на UI Text элемент для отображения золота

    void Start()
    {
        UpdateGoldDisplay(); // Инициализируем отображение золота в начале
    }

    public void AddGold(int amount)
    {
        currentGold += amount; // Увеличиваем золото
        Debug.Log("Получено золота: " + amount);
        UpdateGoldDisplay(); // Обновляем отображение золота
    }

    private void UpdateGoldDisplay()
    {
        // Обновляем текст золота в UI
        if (goldText != null)
        {
            goldText.text = "Золото: " + currentGold; // Пример текста
        }
    }
}
