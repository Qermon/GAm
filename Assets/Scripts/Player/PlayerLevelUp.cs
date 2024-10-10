using UnityEngine;

public class PlayerLevelUp : MonoBehaviour
{
    public int currentLevel = 1;      // Текущий уровень игрока
    public int currentExperience = 0; // Текущий опыт игрока
    public int experienceToNextLevel = 5; // Опыт для перехода на следующий уровень

    void Start()
    {
        // Инициализация уровня и опыта
        UpdateLevel();
    }

    // Метод для получения опыта
    public void GainExperience(int amount)
    {
        currentExperience += amount;
        Debug.Log("Получено опыта: " + amount);

        // Проверка на повышение уровня
        while (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    // Повышение уровня
    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= experienceToNextLevel; // Убираем опыт, потраченный на уровень

        Debug.Log("Поздравляем! Вы достигли уровня: " + currentLevel);

        // Определяем новый опыт для следующего уровня
        UpdateLevel();
    }

    // Логика обновления опыта для следующего уровня
    private void UpdateLevel()
    {
        if (currentLevel < 20)
        {
            experienceToNextLevel = 5 + (currentLevel - 1) * 10;
        }
        else if (currentLevel < 40)
        {
            experienceToNextLevel += 13;
        }
        else
        {
            experienceToNextLevel += 16;
        }

        Debug.Log("Необходимо опыта для следующего уровня: " + experienceToNextLevel);
    }
}


