using UnityEngine;

public class PlayerLevelUp : MonoBehaviour
{
    public int currentLevel = 1;      // Текущий уровень игрока
    public int currentExperience = 0; // Текущий опыт игрока
    public int experienceToNextLevel = 5; // Опыт для перехода на следующий уровень

    void Start()
    {
        UpdateLevel();
    }

    public void GainExperience(int amount)
    {
        currentExperience += amount;
        Debug.Log("Получено опыта: " + amount);

        while (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience -= experienceToNextLevel;

        Debug.Log("Поздравляем! Вы достигли уровня: " + currentLevel);

        FindObjectOfType<LevelUpMenu>().OpenLevelUpMenu(); // Открываем меню улучшений

        UpdateLevel();
    }

    private void UpdateLevel()
    {
        experienceToNextLevel = 5 + (currentLevel - 1) * 10;
        Debug.Log("Необходимо опыта для следующего уровня: " + experienceToNextLevel);
    }
}
