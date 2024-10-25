using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage; // Ссылка на компонент Image для здоровья
    public Image barrierBarImage; // Ссылка на компонент Image для барьера
    private int maxHealth; // Максимальное здоровье
    private float maxBarrier; // Максимальный барьер

    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        healthBarImage.fillAmount = 1f; // Заполняем бар на 100%
    }

    public void SetHealth(int health)
    {
        healthBarImage.fillAmount = (float)health / maxHealth; // Заполняем бар в зависимости от текущего здоровья
    }

    public void SetMaxBarrier(int barrier)
    {
        maxBarrier = barrier;
        barrierBarImage.fillAmount = 1f; // Заполняем барьер на 100%
    }

    public void AddBarrier(float barrier)
    {
        maxBarrier += barrier; // Добавляем новый барьер
        // Обновляем UI барьера
        barrierBarImage.fillAmount = maxBarrier / maxHealth; // Или другой подход для нормализации
    }

    public void SetBarrier(int barrier)
    {
        barrierBarImage.fillAmount = (float)barrier / maxBarrier; // Заполняем барьер в зависимости от текущего значения барьера
    }
}
