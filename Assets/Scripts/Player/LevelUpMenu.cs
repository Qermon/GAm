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
    Common = 0,    // 1 звезда
    Uncommon = 1,  // 2 звезды
    Rare = 2       // 3 звезды
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
    public Sprite[] raritySprites; // Массив спрайтов для редкостей
    public Image[] rarityImages; // Массив для отображения звездочек
    public List<UpgradeOption> upgradeOptions;

    public WaveManager waveManager;

    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private List<Weapon> playerWeapons; // Список оружий
    private int waveNumber; // Переменная для хранения текущей волны

    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerWeapons = new List<Weapon>(FindObjectsOfType<Weapon>()); // Получаем все оружия в игре
        waveManager = FindObjectOfType<WaveManager>(); // Ищем WaveManager в сцене


        levelUpPanel.SetActive(false);
        foreach (Image icon in upgradeIcons)
        {
            icon.gameObject.SetActive(false);
        }
    }

    public void OpenLevelUpMenu()
    {
        waveNumber = waveManager.GetWaveNumber(); // Обновляем значение waveNumber

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
        HashSet<UpgradeType> selectedTypes = new HashSet<UpgradeType>(); // Для отслеживания выбранных типов улучшений

        while (selectedUpgrades.Count < count && availableOptions.Count > 0)
        {
            UpgradeOption selectedUpgrade = GetRandomUpgradeByRarity(availableOptions);

            // Проверяем, не был ли уже выбран тип этого улучшения
            if (!selectedTypes.Contains(selectedUpgrade.upgradeType))
            {
                selectedUpgrades.Add(selectedUpgrade);
                selectedTypes.Add(selectedUpgrade.upgradeType); // Добавляем тип в список выбранных типов
                availableOptions.Remove(selectedUpgrade); // Удаляем улучшение из доступных
            }
        }

        return selectedUpgrades;
    }


    // Метод для получения случайного улучшения с учетом редкости
    private UpgradeOption GetRandomUpgradeByRarity(List<UpgradeOption> availableOptions)
    {
        int commonChance;
        int uncommonChance;
        int rareChance;

        if (waveNumber < 11)
        {
            // Шансы для волн до 11-й
            commonChance = Mathf.Clamp(80 - (waveNumber - 1), 0, 100); // Снижаем шанс обычного улучшения
            uncommonChance = Mathf.Clamp(19 + (waveNumber - 1), 0, 100 - commonChance); // Увеличиваем шанс необычного улучшения
            rareChance = 100 - commonChance - uncommonChance; // Вычисляем шанс редкого улучшения
        }
        else if (waveNumber <= 21)
        {
            // Шансы для волн с 11-й по 21-ю
            commonChance = Mathf.Clamp(68 - (waveNumber - 11) * 2, 0, 100); // Обычные бафы: 68% -2% за волну
            uncommonChance = Mathf.Clamp(27 + (waveNumber - 11), 0, 100 - commonChance); // Необычные бафы: 27% +1% за волну
            rareChance = 100 - commonChance - uncommonChance; // Редкие бафы: 5% +1% за волну
        }
        else
        {
            // Шансы для волн после 21-й
            if (48 - (waveNumber - 21) * 2 > 10)
            {
                commonChance = 48 - (waveNumber - 21) * 2; // Уменьшаем common на 2% за волну
                uncommonChance = 37 + (waveNumber - 21); // Увеличиваем uncommon на 1% за волну
                rareChance = 15 + (waveNumber - 21); // Увеличиваем rare на 1% за волну
            }
            else
            {
                // Когда commonChance достиг 10%, фиксируем его на 10%
                commonChance = 10;

                // Оставшиеся 90% распределяются на ancommon и rare
                uncommonChance = 37 + 19; // uncommon получает 37% + 19%
                rareChance = 15 + 19; // rare получает 15% + 19%
            }
        }



        // Выводим шансы на дроп в консоль
        Debug.Log($"Текущая волна: {waveNumber}");
        Debug.Log($"Шанс на обычное улучшение: {commonChance}%");
        Debug.Log($"Шанс на необычное улучшение: {uncommonChance}%");
        Debug.Log($"Шанс на редкое улучшение: {rareChance}%");

        // Генерация случайного значения
        float randomValue = Random.Range(0f, 100f);

        UpgradeRarity selectedRarity;
        if (randomValue < commonChance) // Шанс на обычное улучшение
        {
            selectedRarity = UpgradeRarity.Common;
        }
        else if (randomValue < commonChance + uncommonChance) // Шанс на необычное улучшение
        {
            selectedRarity = UpgradeRarity.Uncommon;
        }
        else // Шанс на редкое улучшение
        {
            selectedRarity = UpgradeRarity.Rare;
        }

        // Выводим выбранную редкость
        Debug.Log($"Выбранная редкость: {selectedRarity}");

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
            // Отображаем иконки и текст для каждого улучшения
            upgradeIcons[i].sprite = upgrades[i].upgradeSprite;
            upgradeIcons[i].gameObject.SetActive(true);

            string description = GetUpgradeDescription(upgrades[i]);
            upgradeTexts[i].text = description;
            upgradeTexts[i].gameObject.SetActive(true);

            // Отображаем звездочки редкости для каждого улучшения
            int rarityIndex = (int)upgrades[i].upgradeRarity; // Преобразуем редкость в индекс
            rarityImages[i].sprite = raritySprites[rarityIndex]; // Устанавливаем спрайт звезды на основе редкости
            rarityImages[i].gameObject.SetActive(true); // Активируем объект, если он скрыт

            // Устанавливаем действие при нажатии на кнопку
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