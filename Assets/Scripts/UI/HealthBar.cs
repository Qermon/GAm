using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage; // Ссылка на компонент Image

    public void SetMaxHealth(int health)
    {
        // Установить максимальное здоровье
        healthBarImage.fillAmount = 1; // Полная полоска здоровья
    }

    public void SetHealth(int health)
    {
        // Обновить значение полоски здоровья
        healthBarImage.fillAmount = (float)health / 100; // Предполагается, что maxHealth = 100
    }
}

