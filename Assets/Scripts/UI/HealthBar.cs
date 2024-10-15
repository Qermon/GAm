using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage; // Ссылка на компонент Image
    private int maxHealth; // Максимальное здоровье

    // Метод для установки максимального здоровья
    public void SetMaxHealth(int health)
    {
        maxHealth = health; // Сохраняем максимальное здоровье
        healthBarImage.fillAmount = 1; // Полная полоска здоровья
    }

    // Метод для обновления текущего здоровья
    public void SetHealth(int health)
    {
        healthBarImage.fillAmount = (float)health / maxHealth; // Обновляем полоску здоровья
    }
}
