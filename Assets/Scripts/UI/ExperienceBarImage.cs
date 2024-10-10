using UnityEngine;
using UnityEngine.UI;

public class ExperienceBarImage : MonoBehaviour
{
    public Image experienceBarImage;      // Ссылка на изображение полоски опыта
    public PlayerLevelUp playerLevelUp;   // Ссылка на скрипт игрока

    void Start()
    {
        // Инициализируем значение полоски
        UpdateExperienceBar();
    }

    void Update()
    {
        // Обновляем полоску каждый кадр
        UpdateExperienceBar();
    }

    // Метод для обновления состояния полоски
    private void UpdateExperienceBar()
    {
        // Вычисляем процент заполнения полоски
        float fillAmount = (float)playerLevelUp.currentExperience / playerLevelUp.experienceToNextLevel;
        experienceBarImage.fillAmount = fillAmount;
    }
}
