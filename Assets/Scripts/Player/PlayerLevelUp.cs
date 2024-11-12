using UnityEngine;
using TMPro; // Для использования TMP_Text

public class PlayerLevelUp : MonoBehaviour
{
    public int currentLevel = 1;      // Текущий уровень игрока
    public int currentExperience = 0; // Текущий опыт игрока
    public int experienceToNextLevel = 100; // Опыт для перехода на следующий уровень

    public TMP_Text levelText; // Ссылка на UI Text элемент

    public AudioSource lvlUpSound;

    void Start()
    {

        GameObject lvlUpSoundObject = GameObject.Find("LvlUpSound");

        if (lvlUpSoundObject != null)
        {
            lvlUpSound = lvlUpSoundObject.GetComponent<AudioSource>();
        }

        // Поиск объекта с именем "levelText" и получение TMP_Text компонента
        GameObject levelTextObject = GameObject.Find("levelText");
        if (levelTextObject != null)
        {
            levelText = levelTextObject.GetComponent<TMP_Text>();
        }
        else
        {
            Debug.LogError("Объект с именем 'levelText' не найден на сцене! Убедитесь, что он существует.");
        }

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

        if (lvlUpSound != null)
        {
            lvlUpSound.Play();
        }

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
