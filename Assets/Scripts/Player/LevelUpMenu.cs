using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public enum UpgradeType
{
    Damage,          // Урон
    CritDamage,      // Критический урон
    AttackSpeed,     // Скорость атаки
    CritChance,      // Шанс критического удара
    AttackRange,     // Дальность атаки
    MaxHealth,       // Здоровье
    Armor,           // Броня
    HealthRegen,     // Регенерация здоровья
    Lifesteal,       // Вампиризм
    Investment,      // Инвестиции
    PickupRadius,    // Радиус сбора
    MoveSpeed,       // Скорость бега
    Luck             // Удача
}

public enum UpgradeRarity
{
    Common,     // Обычная
    Uncommon,   // Необычная
    Rare        // Редкая
}

[System.Serializable]
public class UpgradeOption
{
    public string upgradeName;
    public Sprite upgradeSprite;
    public UpgradeType upgradeType;
    public UpgradeRarity upgradeRarity; // Добавим редкость
}

public class LevelUpMenu : MonoBehaviour
{
    public GameObject levelUpPanel;
    public Image[] upgradeIcons;
    public Button[] upgradeButtons;
    public TMP_Text[] upgradeTexts; // Тексты, которые будут отображать описание улучшений
    public List<UpgradeOption> upgradeOptions;

    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private List<Weapon> playerWeapons; // Список оружий

    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerWeapons = new List<Weapon>(FindObjectsOfType<Weapon>()); // Получаем все оружия в игре

        levelUpPanel.SetActive(false);
        foreach (Image icon in upgradeIcons)
        {
            icon.gameObject.SetActive(false);
        }
    }

    public void OpenLevelUpMenu()
    {
        if (upgradeOptions.Count < 3)
        {
            Debug.LogError("Недостаточно доступных улучшений.");
            return;
        }

        List<UpgradeOption> selectedUpgrades = GetRandomUpgrades(3);
        DisplayUpgradeOptions(selectedUpgrades);
    }



    private List<UpgradeOption> GetRandomUpgrades(int count)
    {
        List<UpgradeOption> availableOptions = new List<UpgradeOption>(upgradeOptions);
        List<UpgradeOption> selectedUpgrades = new List<UpgradeOption>();

        for (int i = 0; i < count; i++)
        {
            UpgradeOption selectedUpgrade = GetRandomUpgradeByRarity(availableOptions);
            selectedUpgrades.Add(selectedUpgrade);
            availableOptions.Remove(selectedUpgrade);
        }

        return selectedUpgrades;
    }

    // Метод для получения случайного улучшения с учетом редкости
    private UpgradeOption GetRandomUpgradeByRarity(List<UpgradeOption> availableOptions)
    {
        // Вероятности: 80% - Common, 19% - Uncommon, 1% - Rare
        float randomValue = Random.Range(0f, 100f);

        UpgradeRarity selectedRarity;
        if (randomValue < 80f) // 80% шанс на обычную редкость
        {
            selectedRarity = UpgradeRarity.Common;
        }
        else if (randomValue < 19f) // 19% шанс на необычную редкость
        {
            selectedRarity = UpgradeRarity.Uncommon;
        }
        else // 1% шанс на редкую редкость
        {
            selectedRarity = UpgradeRarity.Rare;
        }

        // Фильтруем список доступных опций по выбранной редкости
        List<UpgradeOption> optionsOfSelectedRarity = availableOptions.FindAll(option => option.upgradeRarity == selectedRarity);

        // Если не нашлось опций нужной редкости, возвращаем случайное улучшение
        if (optionsOfSelectedRarity.Count == 0)
        {
            return availableOptions[Random.Range(0, availableOptions.Count)];
        }

        // Возвращаем случайную опцию из отфильтрованного списка
        return optionsOfSelectedRarity[Random.Range(0, optionsOfSelectedRarity.Count)];
    }


    private void DisplayUpgradeOptions(List<UpgradeOption> upgrades)
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0;

        for (int i = 0; i < upgrades.Count; i++)
        {
            // Отображаем иконки и текст
            upgradeIcons[i].sprite = upgrades[i].upgradeSprite;
            upgradeIcons[i].gameObject.SetActive(true);

            string description = GetUpgradeDescription(upgrades[i]);
            upgradeTexts[i].text = description;
            upgradeTexts[i].gameObject.SetActive(true);

            int index = i;
            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() => ChooseUpgrade(upgrades[index]));
        }
    }

    private string GetUpgradeDescription(UpgradeOption upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case UpgradeType.Damage:
                return "Урон + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.10f, 0.15f, 0.20f) * 100) + "%";
            case UpgradeType.CritDamage:
                return "Критический урон + " + (GetUpgradePercentage(upgrade.upgradeRarity, 15f, 20f, 25f)) + "%";
            case UpgradeType.AttackSpeed:
                return "Скорость атаки + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f) * 100) + "%";
            case UpgradeType.CritChance:
                return "Шанс критического удара + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.05f, 0.10f, 0.15f) * 100) + "%";
            case UpgradeType.AttackRange:
                return "Дальность атаки + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f) * 100) + "%";
            case UpgradeType.MaxHealth:
                return "Здоровье + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.30f, 0.40f, 0.50f) * 100) + "%";
            case UpgradeType.Armor:
                return "Броня + " + GetUpgradeValue(upgrade.upgradeRarity, 10, 15, 20);
            case UpgradeType.HealthRegen:
                return "Регенерация здоровья + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f) * 100) + "%";
            case UpgradeType.Lifesteal:
                return "Вампиризм + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f) * 100) + "%";
            case UpgradeType.Investment:
                return "Инвестиции + " + GetUpgradeValue(upgrade.upgradeRarity, 30, 75, 120);
            case UpgradeType.PickupRadius:
                return "Радиус сбора + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.30f, 0.40f, 0.50f) * 100) + "%";
            case UpgradeType.MoveSpeed:
                return "Скорость бега + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.10f, 0.15f, 0.20f) * 100) + "%";
            case UpgradeType.Luck:
                return "Удача + " + GetUpgradeValue(upgrade.upgradeRarity, 10, 20, 30);
            default:
                return "";
        }
    }

    public void ChooseUpgrade(UpgradeOption upgrade)
    {
        ApplyUpgrade(upgrade);
        CloseLevelUpMenu();
    }

    public void CloseLevelUpMenu()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1;
    }

    // Вспомогательные методы для получения значений по редкости
    private float GetUpgradePercentage(UpgradeRarity rarity, float commonValue, float uncommonValue, float rareValue)
    {
        switch (rarity)
        {
            case UpgradeRarity.Uncommon:
                return uncommonValue;
            case UpgradeRarity.Rare:
                return rareValue;
            default:
                return commonValue;
        }
    }

    private int GetUpgradeValue(UpgradeRarity rarity, int commonValue, int uncommonValue, int rareValue)
    {
        switch (rarity)
        {
            case UpgradeRarity.Uncommon:
                return uncommonValue;
            case UpgradeRarity.Rare:
                return rareValue;
            default:
                return commonValue;
        }
    }

    // Применение улучшений
    public void ApplyUpgrade(UpgradeOption upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case UpgradeType.Damage:
                float damageIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.10f, 0.15f, 0.20f);
                foreach (var weapon in playerWeapons) // Применяем к каждому оружию
                {
                    weapon.IncreaseDamage(damageIncrease);
                }
                Debug.Log("Урон увеличен на: " + damageIncrease * 100 + "%");
                break;

            case UpgradeType.CritDamage:
                float critDamageIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 15f, 20f, 25f);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseCritDamage(critDamageIncrease);
                }
                Debug.Log("Критический урон увеличен на: " + critDamageIncrease * 100 + "%");
                break;

            case UpgradeType.AttackSpeed:
                float attackSpeedIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackSpeed(attackSpeedIncrease);
                }
                Debug.Log("Скорость атаки увеличена на: " + attackSpeedIncrease * 100 + "%");
                break;

            case UpgradeType.CritChance:
                // Шанс крита: 5% / 10% / 15%
                float critChanceIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.05f, 0.10f, 0.15f);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseCritChance(critChanceIncrease);
                }
                Debug.Log("Шанс критического удара увеличен на: " + critChanceIncrease * 100 + "%");
                break;

            case UpgradeType.AttackRange:
                // Дальность атаки: 15% / 20% / 25%
                float attackRangeIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackRange(attackRangeIncrease);
                }
                Debug.Log("Дальность атаки увеличена на: " + attackRangeIncrease * 100 + "%");
                break;

            case UpgradeType.MaxHealth:
                // Здоровье: 30% / 40% / 50%
                float maxHealthIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.30f, 0.40f, 0.50f);
                playerHealth.IncreaseMaxHealth(maxHealthIncrease);
                Debug.Log("Здоровье увеличено на: " + maxHealthIncrease * 100 + "%");
                break;

            case UpgradeType.Armor:
                // Броня: 10 / 15 / 20
                int armorIncrease = GetUpgradeValue(upgrade.upgradeRarity, 10, 15, 20);
                playerHealth.IncreaseArmor(armorIncrease);
                Debug.Log("Броня увеличена на: " + armorIncrease);
                break;

            case UpgradeType.HealthRegen:
                // Регенерация здоровья: 15% / 20% / 25%
                float regenIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f);
                playerHealth.IncreaseHealthRegen(regenIncrease);
                Debug.Log("Регенерация здоровья увеличена на: " + regenIncrease * 100 + "%");
                break;

            case UpgradeType.Lifesteal:
                // Вампиризм: 15% / 20% / 25%
                float lifestealIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f);
                playerHealth.IncreaseLifesteal(lifestealIncrease);
                Debug.Log("Вампиризм увеличен на: " + lifestealIncrease * 100 + "%");
                break;

            case UpgradeType.Investment:
                // Инвестиции: 30 / 75 / 120
                int investmentIncrease = GetUpgradeValue(upgrade.upgradeRarity, 30, 75, 120);
                playerHealth.IncreaseInvestment(investmentIncrease);
                Debug.Log("Инвестиции увеличены на: " + investmentIncrease);
                break;

            case UpgradeType.PickupRadius:
                // Радиус сбора: 30% / 40% / 50%
                float pickupRadiusIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.30f, 0.40f, 0.50f);
                playerHealth.IncreasePickupRadius(pickupRadiusIncrease);
                Debug.Log("Радиус сбора увеличен на: " + pickupRadiusIncrease * 100 + "%");
                break;

            case UpgradeType.MoveSpeed:
                // Скорость бега: 10% / 15% / 20%
                float moveSpeedIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.10f, 0.15f, 0.20f);
                playerMovement.IncreaseMoveSpeed(moveSpeedIncrease);
                Debug.Log("Скорость бега увеличена на: " + moveSpeedIncrease * 100 + "%");
                break;

            case UpgradeType.Luck:
                // Удача: 10 / 20 / 30
                float luckIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 10, 20, 30);
                playerHealth.IncreaseLuck((int)luckIncrease);
                Debug.Log("Удача увеличена на: " + luckIncrease);
                break;
        }
    }
}