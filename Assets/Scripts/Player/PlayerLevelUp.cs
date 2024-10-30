using UnityEngine;
using TMPro; // Для использования TMP_Text

public class PlayerLevelUp : MonoBehaviour
{
    public int currentLevel = 1;      // Текущий уровень игрока
    public int currentExperience = 0; // Текущий опыт игрока
    public int experienceToNextLevel = 100; // Опыт для перехода на следующий уровень

    public TMP_Text levelText; // Ссылка на UI Text элемент

    void Start()
    {
        UpdateLevel(); // Инициализируем уровень в начале
    }

    public void GainExperience(int amount)
    {
        currentExperience += amount; // Увеличиваем опыт

        while (currentExperience >= experienceToNextLevel)
        {
            LevelUp(); // Проверяем и проводим уровень вверх
        }
    }

    private void LevelUp()
    {
        currentLevel++; // Увеличиваем уровень
        currentExperience -= experienceToNextLevel; // Уменьшаем опыт на необходимое количество для следующего уровня

        Debug.Log("Поздравляем! Вы достигли уровня: " + currentLevel);
        FindObjectOfType<LevelUpMenu>().OpenLevelUpMenu(); // Открываем меню улучшений

        UpdateLevel(); // Обновляем текст уровня
    }

    private void UpdateLevel()
    {
        experienceToNextLevel = 100 + (currentLevel - 1) * 100; // Обновляем опыт для следующего уровня

        // Обновляем текст уровня в UI
        if (levelText != null)
        {
            levelText.text = currentLevel.ToString();
        }
    }
}