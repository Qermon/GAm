using UnityEngine;

public class LevelUpMenu : MonoBehaviour
{
    public GameObject levelUpPanel;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;


    // Массив оружий, которые будут бафаться
    public Weapon[] weaponsToBuff;


    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerHealth = FindObjectOfType<PlayerHealth>();

    }

    public void OpenLevelUpMenu()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true);
            Debug.Log("Панель Level Up открыта.");
            Time.timeScale = 0;
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
                    playerMovement.moveSpeed += 2;
                    Debug.Log("Скорость увеличена до: " + playerMovement.moveSpeed);
                }
                break;

            case 2: // Увеличение максимального здоровья
                if (playerHealth != null)
                {
                    playerHealth.maxHealth += 20; // Увеличиваем максимальное здоровье на 20
                    playerHealth.UpdateHealthUI();
                    Debug.Log("Максимальное здоровье увеличено до: " + playerHealth.maxHealth);
                }
                break;

            case 3: // Пассивная регенерация здоровья
                if (playerHealth != null)
                {
                    playerHealth.StartHealthRegen(); // Активируем регенерацию
                    Debug.Log("Пассивная регенерация активирована.");
                }
                break;

            case 4: // Баф всех оружий
                foreach (Weapon weapon in weaponsToBuff)
                {
                    if (weapon != null)
                    {
                        weapon.damage += 10; // Увеличиваем урон на 10
                        Debug.Log(weapon.name + " урон увеличен до: " + weapon.damage);
                    }
                }
                break;

            case 5: // Увеличение защиты
                if (playerHealth != null)
                {
                    playerHealth.IncreaseDefense(10); // Увеличиваем защиту на 10
                    Debug.Log("Защита игрока увеличена до: " + playerHealth.defense);
                }
                break;

        }

        CloseLevelUpMenu();
    }

    public void CloseLevelUpMenu()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
            Debug.Log("Панель Level Up закрыта.");
            Time.timeScale = 1;
        }
    }
}
