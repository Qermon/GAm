using UnityEngine;

public class LevelUpMenu : MonoBehaviour
{
    public GameObject levelUpPanel; // Панель для уровня
    private PlayerMovement playerMovement;

    private void Start()
    {
        // Найдите компонент PlayerMovement на объекте игрока
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    public void OpenLevelUpMenu()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true); // Активируем панель
            Debug.Log("Панель Level Up открыта.");
            Time.timeScale = 0; // Останавливаем время
        }
        else
        {
            Debug.LogError("Панель Level Up не назначена в инспекторе.");
        }
    }

    public void ChooseUpgrade(int choice)
    {
        switch (choice)
        {
            case 1: // Увеличение скорости
                if (playerMovement != null)
                {
                    playerMovement.moveSpeed += 2; // Например, увеличиваем скорость на 2
                    Debug.Log("Скорость увеличена до: " + playerMovement.moveSpeed);
                }
                break;
            case 2: // Увеличение здоровья
                // Логика увеличения здоровья
                break;
            case 3: // Увеличение урона
                // Логика увеличения урона
                break;
        }

        CloseLevelUpMenu(); // Закрываем меню после выбора
    }

    public void CloseLevelUpMenu()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false); // Деактивируем панель
            Debug.Log("Панель Level Up закрыта.");
            Time.timeScale = 1; // Возвращаем время
        }
    }
}
