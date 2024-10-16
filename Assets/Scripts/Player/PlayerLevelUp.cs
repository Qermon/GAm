using UnityEngine;
using TMPro; // Для использования TMP_Text

public class PlayerLevelUp : MonoBehaviour
{
    public int currentLevel = 1;      // Текущий уровень игрока
    public int currentExperience = 0; // Текущий опыт игрока
    public int experienceToNextLevel = 5; // Опыт для перехода на следующий уровень

    public TMP_Text levelText; // Ссылка на UI Text элемент

    void Start()
    {
        UpdateLevel(); // Инициализируем уровень в начале
    }

    public void GainExperience(int amount)
    {
        currentExperience += amount; // Увеличиваем опыт
        Debug.Log("Получено опыта: " + amount);

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
        experienceToNextLevel = 5 + (currentLevel - 1) * 10; // Обновляем опыт для следующего уровня
        Debug.Log("Необходимо опыта для следующего уровня: " + experienceToNextLevel);

        // Обновляем текст уровня в UI
        if (levelText != null)
        {
            levelText.text = currentLevel.ToString();
        }
    }
}