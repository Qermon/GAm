using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage; // Ссылка на компонент Image для здоровья
    private int maxHealth; // Максимальное здоровье


    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        healthBarImage.fillAmount = 1f; // Заполняем бар на 100%
    }

    public void SetHealth(int health)
    {
        healthBarImage.fillAmount = (float)health / maxHealth; // Заполняем бар в зависимости от текущего здоровья
    }
}
