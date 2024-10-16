using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeType
{
    Speed,
    MaxHealth,
    HealthRegen,
    WeaponBuff,
    Defense,
    Weapon, // Новый тип улучшения для оружия
    Lifesteal,

}

[System.Serializable]
public class UpgradeOption
{
    public string upgradeName; // Название улучшения (для удобства)
    public Sprite upgradeSprite; // Спрайт, который будет отображаться в меню
    public UpgradeType upgradeType; // Тип улучшения
    public Weapon[] weaponsToBuff; // Оружия, которые будут бафаться (если это WeaponBuff)
    public GameObject weaponPrefab; // Префаб оружия, которое будет предоставлено игроку
}

public class LevelUpMenu : MonoBehaviour
{
    public GameObject levelUpPanel; // Панель меню повышения уровня
    public Image[] upgradeIcons; // Иконки для отображения выбранных улучшений
    public Button[] upgradeButtons; // Кнопки для выбора улучшений
    public List<UpgradeOption> upgradeOptions; // Список всех возможных улучшений

    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerHealth = FindObjectOfType<PlayerHealth>();

        // Скрываем панель и кнопки
        levelUpPanel.SetActive(false);
        foreach (Image icon in upgradeIcons)
        {
            icon.gameObject.SetActive(false);
        }
    }

    public void OpenLevelUpMenu()
    {
        // Убедитесь, что есть достаточно вариантов для выбора
        if (upgradeOptions.Count < 3)
        {
            Debug.LogError("Недостаточно доступных улучшений.");
            return;
        }

        // Выбираем три случайных варианта
        List<UpgradeOption> selectedUpgrades = GetRandomUpgrades(3);

        // Отображаем их на панели
        DisplayUpgradeOptions(selectedUpgrades);
    }

    private List<UpgradeOption> GetRandomUpgrades(int count)
    {
        List<UpgradeOption> availableOptions = new List<UpgradeOption>(upgradeOptions);
        List<UpgradeOption> selectedUpgrades = new List<UpgradeOption>();

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availableOptions.Count);
            selectedUpgrades.Add(availableOptions[randomIndex]);
            availableOptions.RemoveAt(randomIndex); // Убираем выбранный вариант, чтобы не повторялся
        }

        return selectedUpgrades;
    }

    private void DisplayUpgradeOptions(List<UpgradeOption> upgrades)
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0; // Останавливаем время

        for (int i = 0; i < upgrades.Count; i++)
        {
            upgradeIcons[i].sprite = upgrades[i].upgradeSprite; // Присваиваем спрайт
            upgradeIcons[i].gameObject.SetActive(true); // Активируем иконку

            // Присваиваем кнопке соответствующую функцию при нажатии
            int index = i; // Нужно для правильной передачи индекса в лямбда-функцию
            upgradeButtons[i].onClick.RemoveAllListeners(); // Очищаем старые события
            upgradeButtons[i].onClick.AddListener(() => ChooseUpgrade(upgrades[index]));
        }
    }

    public void ChooseUpgrade(UpgradeOption upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case UpgradeType.Speed:
                playerMovement.moveSpeed += 2;
                Debug.Log("Скорость увеличена до: " + playerMovement.moveSpeed);
                break;

            case UpgradeType.MaxHealth:
                playerHealth.maxHealth += 20;
                playerHealth.UpdateHealthUI();
                Debug.Log("Максимальное здоровье увеличено до: " + playerHealth.maxHealth);
                break;

            case UpgradeType.HealthRegen:
                Debug.Log("Игрок выбрал регенерацию здоровья.");
                playerHealth.StartHealthRegen();
                Debug.Log("Пассивная регенерация активирована.");
                break;

            case UpgradeType.WeaponBuff:
                foreach (Weapon weapon in upgrade.weaponsToBuff)
                {
                    if (weapon != null)
                    {
                        weapon.damage += 10;
                        Debug.Log(weapon.name + " урон увеличен до: " + weapon.damage);
                    }
                }
                break;

            case UpgradeType.Defense:
                playerHealth.IncreaseDefense(10);
                Debug.Log("Защита игрока увеличена до: " + playerHealth.defense);
                break;

            case UpgradeType.Weapon:
                if (upgrade.weaponPrefab != null)
                {
                    GameObject weapon = Instantiate(upgrade.weaponPrefab, transform.position, Quaternion.identity);
                    weapon.SetActive(true);
                    Debug.Log(upgrade.weaponPrefab.name + " получено.");
                }
                break;
            case UpgradeType.Lifesteal: // Добавляем новый случай для вампиризма
                playerHealth.AddLifesteal(10); // Добавляем 10% вампиризма
                break;
        }

        CloseLevelUpMenu();
    }

    public void CloseLevelUpMenu()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1; // Возвращаем нормальное время
    }
}
