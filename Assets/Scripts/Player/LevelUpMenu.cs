using UnityEngine;

public class LevelUpMenu : MonoBehaviour
{
    public GameObject levelUpPanel; // Панель с выбором улучшений

    private Player player;
    private PlayerMovement playerMovement;

    void Start()
    {
        // Находим игрока и необходимые компоненты
        player = FindObjectOfType<Player>();
        playerMovement = player.GetComponent<PlayerMovement>();

        // Скрываем панель с улучшениями при старте
        levelUpPanel.SetActive(false);
    }

    // Открываем меню выбора улучшений
    public void OpenLevelUpMenu()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true);  // Активируем панель
            Debug.Log("Панель Level Up открыта.");
            Time.timeScale = 0;  // Останавливаем время
        }
        else
        {
            Debug.LogError("Панель Level Up не назначена в инспекторе.");
        }
    }


    // Закрываем меню
    public void CloseLevelUpMenu()
    {
        Time.timeScale = 1f; // Возвращаем игру в нормальное состояние
        levelUpPanel.SetActive(false);
    }

    // Выбираем улучшение
    public void ChooseUpgrade(int choice)
    {
        switch (choice)
        {
            case 1:
                playerMovement.moveSpeed += 1f;
                Debug.Log("Скорость игрока увеличена до: " + playerMovement.moveSpeed);
                break;
            case 2:
                player.IncreaseMaxHealth(20); // Увеличиваем здоровье
                break;
            case 3:
                player.IncreaseWeaponDamage(5); // Увеличиваем урон оружий
                break;
            default:
                Debug.Log("Неверный выбор!");
                break;
        }

        CloseLevelUpMenu(); // Закрываем меню
    }
}
