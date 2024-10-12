using UnityEngine;

public class LevelUpMenu : MonoBehaviour
{
    public GameObject levelUpPanel; // Панель для уровня
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth; // Ссылка на класс PlayerHealth

    private void Start()
    {
        // Найдите компоненты PlayerMovement и PlayerHealth на объекте игрока
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerHealth = FindObjectOfType<PlayerHealth>();
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
                if (playerHealth != null)
                {
                    playerHealth.maxHealth += 20; // Увеличиваем максимальное здоровье на 20
                    playerHealth.currentHealth = playerHealth.maxHealth; // Восстанавливаем текущее здоровье до максимума
                    playerHealth.UpdateHealthUI(); // Обновляем UI полоски здоровья
                    Debug.Log("Максимальное здоровье увеличено до: " + playerHealth.maxHealth);
                }
                break;

            case 3: // Увеличение урона
                // Логика увеличения урона (необходимо добавить вашу логику здесь)
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