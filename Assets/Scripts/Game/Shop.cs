using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public Button closeButton;
    public TextMeshProUGUI playerStatsText;

    public Button[] buffButtons; // Массив кнопок для выбора баффов
    public TextMeshProUGUI[] buffCostTexts; // Тексты для отображения стоимости баффов
    public TextMeshProUGUI[] buffDescriptionTexts; // Поля для вывода описания для каждого баффа
    private List<Upgrade> upgrades = new List<Upgrade>(); // Список доступных баффов
    
    public Button refreshButton; // Кнопка обновления
    private int currentRefreshCost = 7; // Начальная стоимость обновления
    public TextMeshProUGUI refreshCostText;  // Текстовый объект для отображения стоимости обновления
    private float baseCost = 50f; // Используем float для более точных вычислений
    private float priceIncreasePercentage = 0.1f; // 10% увеличение


    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private PlayerGold playerGold;
    private LevelUpMenu levelUpMenu;
    private WaveManager waveManager;
    private List<Weapon> playerWeapons; // Список оружий
    private Weapon weapon;
    private CursorManager cursorManager;
    

    public UpgradeOption[] upgradeOptions; // Массив доступных баффов
    private HashSet<UpgradeType> existingBuffs = new HashSet<UpgradeType>(); // HashSet для отслеживания существующих баффов

    private float totalAttackSpeedBonus = 0f;
    private float totalAttackRangeBonus = 0f;
    private float totalPickupRadiusBonus = 0f;
    private float totalLifestealBonus = 0f;
    private float totalRegenBonus = 0f;

    // Массив для хранения пустых иконок
    [SerializeField] private Image[] emptyIcons;

    private HashSet<UpgradeType> oneTimeBuffs = new HashSet<UpgradeType>
{
    UpgradeType.ShieldOnKill,
    UpgradeType.BarrierOnLowHealth,
    UpgradeType.HealthRegenPerWave,
    UpgradeType.CritChanceBuff,
    UpgradeType.CritDamageBuff
};


    public enum UpgradeType
    {
        ShieldPerWave,    // Барьер за волну
        ShieldOnKill,
        BarrierOnLowHealth,
        HealthRegenPerWave,
        CritChanceBuff,
        CritDamageBuff,
        AttackSpeedDamage,
        AttackSpeedDamageCritMove,
        CritDamageCritChance,
        MaxHpArmorMove,
        AttackSpeedHp,
        DamageMove,
        DamageRegen,
        CritChanceDamage,
        AttackSpeedCritArmor,
        AttackRangeMoveSpeed,
        RegenLuck,
        LifestealDamage,
        InvestmentLuckMaxHp,
        MoveSpeedDamage,
        ArmorAttackRange,
        CritDamageCritChance1,
        LuckRegen,
        PickupRadiusAttackSpeed,
        RegenMoveSpeed,
        InvestmentDamageArmor,
        MaxHpCritChanceMove,
        AttackSpeedLuck,
        LifestealPickupRadius,
        CritDamageDamageArmor,
        AttackSpeedCritChancePickup,
        DamageMaxHpArmor,
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
        Luck,
    }


    [System.Serializable]
    public class UpgradeOption
    {
        public string upgradeName;
        public Sprite upgradeSprite;
        public UpgradeType upgradeType;
    }



    private void Start()
    {
        cursorManager = FindObjectOfType<CursorManager>();
        refreshButton.onClick.AddListener(TryRefreshBuffs);
        playerWeapons = new List<Weapon>(FindObjectsOfType<Weapon>()); // Получаем все оружия в игре
        waveManager = FindObjectOfType<WaveManager>();
        shopPanel.SetActive(false);

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseShop);
        }

        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(RefreshBuffs); // Добавляем обработчик для кнопки обновления
        }


        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerGold = FindObjectOfType<PlayerGold>();

        for (int i = 0; i < buffButtons.Length; i++)
        {
            int index = i;
            buffButtons[i].onClick.AddListener(() => PurchaseUpgrade(index));
        }

        UpdateRefreshButton();
        GenerateUpgrades();
        float moveSpeed = playerMovement.moveSpeed * 200;
    }

    // Обновляет состояние кнопки и отображение стоимости
    void UpdateRefreshButton()
    {
        refreshCostText.text = currentRefreshCost.ToString() + " Gold";

        // Проверяем, хватает ли золота
        if (playerGold.currentGold >= currentRefreshCost)
        {
            refreshButton.interactable = true;  // Активируем кнопку
        }
        else
        {
            refreshButton.interactable = false; // Деактивируем кнопку
        }
    }

    public void OpenShop()
    {
        cursorManager.ShowCursor();
        InitializeShop(); // Инициализация доступных баффов
        shopPanel.SetActive(true);
        Time.timeScale = 0f;
        currentRefreshCost = 7; // Сбрасываем стоимость обновления до начального значения
        UpdateRefreshButton();
        UpdatePlayerStats();
        UpdateUpgradeUI();

    }

    public void CloseShop()
    {
        cursorManager.HideCursor();
        shopPanel.SetActive(false);
        Time.timeScale = 1f;
        playerGold.OnShopClosed();

        // Обновляем баффы при закрытии магазина
        GenerateUpgrades();
    }

    private void TryRefreshBuffs()
    {
        if (playerGold.currentGold >= currentRefreshCost)
        {
            playerGold.currentGold -= currentRefreshCost; // Вычитаем золото за обновление
            RefreshBuffs();
            IncreaseRefreshCost();
            UpdateRefreshButton();
            playerGold.UpdateGoldDisplay();
        }
        else
        {
            Debug.Log("Недостаточно золота для обновления магазина!");
        }
    }
    private void IncreaseRefreshCost()
    {
        // Увеличиваем стоимость следующего обновления
        currentRefreshCost = currentRefreshCost * 2 + Random.Range(0, 6);
    }

    private void RefreshBuffs()
    {

        InitializeShop();
        UpdateUpgradeUI();   // Обновляем UI
    }


    private void UpdatePlayerStats()
    {
        float moveSpeed = playerMovement.moveSpeed * 200;
        float averageDamage = 0f; // Объявляем переменную вне блока if
        float averageCritDamage = 0f; // Для среднего критического урона
        float averageCritChance = 0f; // Для среднего критического шанса

        if (playerHealth != null && playerStatsText != null && playerMovement != null)
        {
            if (playerWeapons != null && playerWeapons.Count > 0)
            {
                // Суммируем урон всех оружий
                float totalDamage = 0f;
                float totalCritDamage = 0f; // Для суммирования критического урона
                float totalCritChance = 0f; // Суммируем критический шанс

                foreach (var weapon in playerWeapons)
                {
                    totalDamage += weapon.damage; // Предполагается, что у вас есть свойство damage в классе Weapon
                    totalCritDamage += weapon.criticalDamage; // Суммируем критический урон
                    totalCritChance += weapon.criticalChance; // Суммируем критический шанс
                }

                averageDamage = totalDamage / playerWeapons.Count; // Средний урон
                averageCritDamage = totalCritDamage / playerWeapons.Count; // Средний критический урон
                averageCritChance = totalCritChance / playerWeapons.Count; // Средний критический шанс
            }

            UpdateTotalAttackSpeedBonus();
            UpdateTotalAttackRangeBonus();
            UpdateTotalPickupRadiusBonus();
            UpdateTotalLifestealBonus();
            UpdateTotalRegenBonus();
            UpdateRefreshButton();

            playerStatsText.text = $"Здоровье: {FormatStatTextMaxHp((int)playerHealth.maxHealth)}\n" +
                                   $"Урон: {FormatStatTextDamage((int)averageDamage)}\n" + // Отображаем средний урон
                                   $"Крит. урон: {FormatStatText((int)averageCritDamage)}%\n" + // Критический урон
                                   $"Крит. шанс: {FormatStatText((int)(averageCritChance * 100))}%\n" + // Критический шанс
                                   $"Скорость атаки: {FormatStatText((int)totalAttackSpeedBonus)}%\n" + // Бонус скорости атаки
                                   $"Дальность атаки: {FormatStatText((int)totalAttackRangeBonus)}%\n" + // Бонус дальности атаки
                                   $"Регенерация: {FormatStatText((int)totalRegenBonus)}%\n" + // Регенерация
                                   $"Вампиризм: {FormatStatText((int)totalLifestealBonus)}%\n" + // Вампиризм
                                   $"Защита: {FormatStatText(playerHealth.defense)}\n" +
                                   $"Скорость бега: {FormatStatTextMoveSpeed((int)moveSpeed)}\n" +
                                   $"Радиус сбора: {FormatStatText((int)totalPickupRadiusBonus)}%\n" +
                                   $"Инвестиции: {FormatStatText((int)playerHealth.investment)}\n" +
                                   $"Удача: {FormatStatText((int)playerHealth.luck)}\n";
        }
    }

    private string FormatStatText(int value)
    {
        string color;

        if (value > 0)
        {
            color = "green"; // Зелёный для положительных значений
        }
        else if (value < 0)
        {
            color = "red"; // Красный для отрицательных значений
        }
        else
        {
            color = "white"; // Белый для 0
        }

        // Возвращаем строку с цветом
        return $"<color={color}>{value}</color>";
    }

    private string FormatStatTextMaxHp(int value)
    {
        string color;

        if (value > playerHealth.baseMaxHp)
        {
            color = "green"; // Зелёный для положительных значений
        }
        else if (value < playerHealth.baseMaxHp)
        {
            color = "red"; // Красный для отрицательных значений
        }
        else
        {
            color = "white";
        }

        // Возвращаем строку с цветом
        return $"<color={color}>{value}</color>";
    }

    
    
    private string FormatStatTextMoveSpeed(int value)
    {
        string color;

        if (value > 240)
        {
            color = "green"; // Зелёный для положительных значений
        }
        else if (value < 240)
        {
            color = "red"; // Красный для отрицательных значений
        }
        else
        {
            color = "white"; // Белый для 240
        }

        // Возвращаем строку с цветом
        return $"<color={color}>{value}</color>";
    }

    private string FormatStatTextDamage(int value)
    {
        string color;

        if (value > 27)
        {
            color = "green"; // Зелёный для положительных значений
        }
        else if (value < 27)
        {
            color = "red"; // Красный для отрицательных значений
        }
        else
        {
            color = "white"; // Белый для 27
        }

        // Возвращаем строку с цветом
        return $"<color={color}>{value}</color>";
    }




    public void UpdateTotalAttackSpeedBonus()
    {
        totalAttackSpeedBonus = 0f;

        if (playerWeapons.Count > 0)
        {
            var weapon = playerWeapons[0]; // Берём первое оружие
            totalAttackSpeedBonus = (weapon.attackSpeed - weapon.baseAttackSpeed) / weapon.baseAttackSpeed * 100;
        }
    }

    public void UpdateTotalAttackRangeBonus()
    {
        totalAttackRangeBonus = 0f;

        if (playerWeapons.Count > 0)
        {
            var weapon = playerWeapons[0]; // Берём первое оружие
            totalAttackRangeBonus = (weapon.attackRange - weapon.baseAttackRange) / weapon.baseAttackRange * 100;
        }
    }

    public void UpdateTotalPickupRadiusBonus()
    {
        totalPickupRadiusBonus = 0f;

        totalPickupRadiusBonus = (playerHealth.pickupRadius - playerHealth.basePickupRadius) / playerHealth.basePickupRadius * 100;

    }
    public void UpdateTotalLifestealBonus()
    {
        totalLifestealBonus = 0f;

        totalLifestealBonus = (playerHealth.lifesteal - playerHealth.baseLifesteal) / playerHealth.baseLifesteal * 100;

    }

    public void UpdateTotalRegenBonus()
    {
        totalRegenBonus = 0f;

        totalRegenBonus = (playerHealth.regen - playerHealth.baseRegen) / playerHealth.baseRegen * 100;

    }

    private void GenerateUpgrades()
    {
        upgrades.Clear(); // Очистить список доступных баффов
        HashSet<UpgradeType> usedTypes = new HashSet<UpgradeType>(); // Для отслеживания использованных типов

        // Создайте список всех доступных опций
        List<UpgradeOption> availableOptions = new List<UpgradeOption>(upgradeOptions);

        while (upgrades.Count < buffButtons.Length)
        {
            Upgrade newUpgrade = GenerateRandomUpgrade(availableOptions);

            // Проверка, существует ли уже такой тип баффа
            if (!usedTypes.Contains(newUpgrade.type))
            {
                upgrades.Add(newUpgrade);
                usedTypes.Add(newUpgrade.type); // Добавить тип баффа в используемые
            }
        }

        // Обновление интерфейса после генерации уникальных баффов
        UpdateUpgradeUI();
    }




    private Upgrade GenerateRandomUpgrade(List<UpgradeOption> availableOptions)
    {
        // Проверка на пустой список доступных опций
        if (availableOptions.Count == 0)
        {
            return null; // Или обработайте случай, когда нет доступных опций
        }

        // Генерация случайного индекса
        int randomIndex = Random.Range(0, availableOptions.Count);
        UpgradeOption selectedOption = availableOptions[randomIndex];

        // Генерация стоимости апгрейда с учетом увеличения и случайного значения
        float currentCost = CalculateCurrentCost();
        int randomAdjustment = Random.Range(-10, 21); // Случайное значение от -10 до +20
        int finalCost = Mathf.Max(0, (int)(currentCost + randomAdjustment)); // Убедитесь, что стоимость не отрицательная

        // Удаляем выбранный вариант из доступных
        availableOptions.RemoveAt(randomIndex);

        return new Upgrade(selectedOption.upgradeType, finalCost, selectedOption.upgradeSprite);
    }
    private float CalculateCurrentCost()
    {
        return baseCost * Mathf.Pow(1 + priceIncreasePercentage, waveManager.waveNumber);
    }


    private void UpdateUpgradeUI()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            buffButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{upgrades[i].type}";
            buffCostTexts[i].text = $"Cost: {upgrades[i].cost}";

            // Устанавливаем иконку на пустую иконку
            if (emptyIcons[i] != null && upgrades[i].upgradeSprite != null)
            {
                emptyIcons[i].sprite = upgrades[i].upgradeSprite;
            }
            else if (emptyIcons[i] != null)
            {
                emptyIcons[i].sprite = null; // Сбрасываем иконку, если нет спрайта
            }

            // Обновляем текст описания для соответствующего баффа
            if (i < buffDescriptionTexts.Length) // Проверка, чтобы избежать выхода за границы массива
            {
                buffDescriptionTexts[i].text = GetUpgradeDescription(upgrades[i].type);
            }
        }
    }


    private void PurchaseUpgrade(int index)
    {
        Upgrade selectedUpgrade = upgrades[index];

        if (playerGold.currentGold >= selectedUpgrade.cost)
        {
            // Проверяем, был ли уже куплен этот бафф
            if (oneTimeBuffs.Contains(selectedUpgrade.type) && existingBuffs.Contains(selectedUpgrade.type))
            {
                Debug.Log("Этот бафф уже куплен и его нельзя купить снова!");
                return; // Не позволяем купить этот бафф снова
            }

            playerGold.currentGold -= selectedUpgrade.cost;
            ApplyUpgrade(selectedUpgrade);

            // Добавляем бафф в список купленных, если это бафф, который можно купить только один раз
            if (oneTimeBuffs.Contains(selectedUpgrade.type))
            {
                existingBuffs.Add(selectedUpgrade.type);
            }

            // Обновляем список доступных опций, исключая уже купленные и разовые баффы
            List<UpgradeOption> availableOptions = new List<UpgradeOption>(upgradeOptions);

            // Убираем все купленные и одноразовые баффы из доступных опций
            foreach (var upgrade in upgrades)
            {
                availableOptions.RemoveAll(option => option.upgradeType == upgrade.type || existingBuffs.Contains(option.upgradeType));
            }

            // Генерируем новый уникальный бафф, который отсутствует на других слотах
            Upgrade newUpgrade = GenerateRandomUpgrade(availableOptions);
            upgrades[index] = newUpgrade; // Обновляем бафф на кнопке

            UpdateUpgradeUI();
            UpdatePlayerStats(); // Обновляем характеристики героя
            playerGold.UpdateGoldDisplay();
        }
    }

    private void InitializeShop()
    {
        // Обнуляем список доступных опций
        List<UpgradeOption> availableOptions = new List<UpgradeOption>(upgradeOptions);

        // Убираем все купленные и одноразовые баффы из доступных опций
        foreach (var upgrade in upgrades)
        {
            availableOptions.RemoveAll(option => option.upgradeType == upgrade.type || existingBuffs.Contains(option.upgradeType));
        }

        // Генерируем новые доступные баффы
        for (int i = 0; i < upgrades.Count; i++) // Измените Length на Count
        {
            Upgrade newUpgrade = GenerateRandomUpgrade(availableOptions);
            upgrades[i] = newUpgrade; // Обновляем доступные баффы на кнопках
        }

        UpdateUpgradeUI(); // Обновляем пользовательский интерфейс магазина
    }


    public class Upgrade
    {
        public UpgradeType type;
        public int cost;
        public Sprite upgradeSprite;

        public Upgrade(UpgradeType type, int cost, Sprite sprite)
        {
            this.type = type;
            this.cost = cost;
            this.upgradeSprite = sprite;
        }
    }

    private string GetUpgradeDescription(UpgradeType upgradeType)
    {
        string description = string.Empty;

        switch (upgradeType)
        {
            case UpgradeType.ShieldPerWave:
                description = "Барьер <color=green>30%</color> от макс хп каждую волну пока хп не упадет до <color=green>29%</color>";
                break;

            case UpgradeType.ShieldOnKill:
                description = "Шанс <color=green>5%</color> получить барьер при убийстве врага, <color=green>10%</color> от макс. здоровья";
                break;

            case UpgradeType.BarrierOnLowHealth:
                description = "При снижении здоровья ниже <color=green>50%</color> раз за волну даёт барьер в <color=green>20%</color> от макс. здоровья";
                break;

            case UpgradeType.HealthRegenPerWave:
                description = "Регенерация +200%, но у тебя <color=green>30%</color> хп в начале каждой волны";
                break;

            case UpgradeType.CritChanceBuff:
                description = "Каждую секунду увеличивает крит шанс на <color=green>0.5%</color> до конца волны";
                break;

            case UpgradeType.CritDamageBuff:
                description = "Каждую секунду увеличивает крит урон на <color=green>1%</color> до конца волны";
                break;

            case UpgradeType.AttackSpeedDamage:
                description = "Скорость атаки +20%\nУрон -5%";
                break;

            case UpgradeType.AttackSpeedDamageCritMove:
                description = "Скорость атаки +15%\nУрон +5%\nКрит Урон +5%\nСкорость бега -10%";
                break;

            case UpgradeType.CritDamageCritChance:
                description = "Крит. урон +30%\nШанс крита -10%";
                break;

            case UpgradeType.MaxHpArmorMove:
                description = "Здоровье +30%\nБроня +20\nСкорость бега -10%";
                break;

            case UpgradeType.AttackSpeedHp:
                description = "Здоровье -15%\nСкорость атаки +20%";
                break;

            case UpgradeType.DamageMove:
                description = "Урон +20%\nСкорость бега -5%";
                break;

            case UpgradeType.DamageRegen:
                description = "Урон +15%\nРегенерация  -10%";
                break;

            case UpgradeType.CritChanceDamage:
                description = "Шанс крита +10%\nУрон -3%";
                break;

            case UpgradeType.AttackSpeedCritArmor:
                description = "Шанс крита +5%\nБроня +10\nСкорость атаки -5%";
                break;

            case UpgradeType.AttackRangeMoveSpeed:
                description = "Дальность атаки +15%\nСкорость бега -3%";
                break;

            case UpgradeType.RegenLuck:
                description = "Регенерация +20%\nУдача -20";
                break;

            case UpgradeType.LifestealDamage:
                description = "Вампиризм +15%\nУрон -3%";
                break;

            case UpgradeType.InvestmentLuckMaxHp:
                description = "Инвестиции +75\nУдача +30\nЗдоровье -15%";
                break;

            case UpgradeType.MoveSpeedDamage:
                description = "Скорость бега +15%\nУрон -5%";
                break;

            case UpgradeType.ArmorAttackRange:
                description = "Броня +20\nДальность атаки -5%";
                break;

            case UpgradeType.CritDamageCritChance1:
                description = "Крит. урон +20%\nШанс крита -5%";
                break;

            case UpgradeType.LuckRegen:
                description = "Удача +40\nРегенерация -15%";
                break;

            case UpgradeType.PickupRadiusAttackSpeed:
                description = "Радиус подбора +20%\nСкорость атаки -5%";
                break;

            case UpgradeType.RegenMoveSpeed:
                description = "Регенерация +15%\nСкорость бега -5%";
                break;

            case UpgradeType.InvestmentDamageArmor:
                description = "Инвестиции +50\nУрон +3%\nБроня -15";
                break;

            case UpgradeType.MaxHpCritChanceMove:
                description = "Здоровье +10%\nШанс крита +5%\nСкорость передвижения  -5%";
                break;

            case UpgradeType.AttackSpeedLuck:
                description = "Скорость атаки +15%\nУдача -30";
                break;

            case UpgradeType.LifestealPickupRadius:
                description = "Вампиризм +15%\nРадиус подбора -15%";
                break;

            case UpgradeType.CritDamageDamageArmor:
                description = "Крит. урон +25%\nБроня +10\nУрон -5%";
                break;

            case UpgradeType.AttackSpeedCritChancePickup:
                description = "Скорость атаки +15%\nШанс крита +3%\nРадиус подбора  -10%";
                break;

            case UpgradeType.DamageMaxHpArmor:
                description = "Урон +25%\nЗдоровье -5%\nБроня -5";
                break;

            case UpgradeType.Damage:
                description = "Урон +15%";
                break;

            case UpgradeType.CritDamage:
                description = "Крит. урон +20%";
                break;

            case UpgradeType.AttackSpeed:
                description = "Скорость атаки +20%";
                break;

            case UpgradeType.CritChance:
                description = "Шанс крита +10%";
                break;

            case UpgradeType.AttackRange:
                description = "Дальность атаки +20%";
                break;

            case UpgradeType.MaxHealth:
                description = "Здоровье +25%";
                break;

            case UpgradeType.Armor:
                description = "Броня +15";
                break;

            case UpgradeType.HealthRegen:
                description = "Регенерация +20%";
                break;

            case UpgradeType.Lifesteal:
                description = "Вампиризм +20%";
                break;

            case UpgradeType.Investment:
                description = "Инвестиции +75";
                break;

            case UpgradeType.PickupRadius:
                description = "Радиус подбора +40%";
                break;

            case UpgradeType.MoveSpeed:
                description = "Скорость бега +15%";
                break;

            case UpgradeType.Luck:
                description = "Удача +20";
                break;

            default:
                description = "Неизвестный бафф";
                break;
        }

        return FormatDescription(description);
    }


    private string FormatDescription(string description)
    {
        // Ищем значения с + и выделяем зелёным цветом
        description = Regex.Replace(description, @"\+\d+%?", match => $"<color=green>{match.Value}</color>");

        // Ищем значения с - и выделяем красным цветом
        description = Regex.Replace(description, @"-\d+%?", match => $"<color=red>{match.Value}</color>");

        return description;
    }





    private void ApplyUpgrade(Upgrade upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeType.ShieldPerWave:
                playerHealth.AddShieldBuff(); // Увеличиваем количество активированных баффов
                playerHealth.ActivateShield(); // Активируем щит
                Debug.Log("Бафф ShieldPerWave применен: щит активирован.");
                break;

            case UpgradeType.ShieldOnKill:
                playerHealth.ActivateShieldOnKillBuff();
                break;

            case UpgradeType.BarrierOnLowHealth:
                playerHealth.ActivateBarrierOnLowHealthBuff();
                break;

            case UpgradeType.HealthRegenPerWave:
                playerHealth.ActivateHealthRegenPerWaveBuff();
                playerHealth.regen += playerHealth.baseRegen * 2;
                break;

            case UpgradeType.CritChanceBuff: // Новый тип апгрейда для крит шанса
                foreach (var weapon in FindObjectsOfType<Weapon>())
                {
                    weapon.PurchaseCritChanceBuff(); // Покупаем и активируем бафф для всех оружий
                }
                break;

            case UpgradeType.CritDamageBuff: // Новый тип апгрейда для крит шанса
                foreach (var weapon in FindObjectsOfType<Weapon>())
                {
                    weapon.PurchaseCritDamageBuff(); // Покупаем и активируем бафф для всех оружий
                }
                break;

            case UpgradeType.AttackSpeedDamage:

                float attackSpeedIncrease = 0.20f;
                float damageIncrease = -0.05f;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackSpeed(attackSpeedIncrease);
                    weapon.IncreaseDamage(damageIncrease);
                }
                break;

            case UpgradeType.AttackSpeedDamageCritMove:
                float attackSpeedIncrease1 = 0.15f;
                float damageIncrease1 = 0.05f;
                float critDamageIncrease1 =5f;
                float moveSpeedIncrease1 = -0.10f;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackSpeed(attackSpeedIncrease1);
                    weapon.IncreaseDamage(damageIncrease1);
                    weapon.IncreaseCritDamage(critDamageIncrease1);
                }
                playerMovement.IncreaseMoveSpeed(moveSpeedIncrease1);
                break;

            case UpgradeType.CritDamageCritChance:
                float critDamageIncrease2 = 30f;
                float critChanceIncrease2 = -0.1f;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseCritDamage(critDamageIncrease2);
                    weapon.IncreaseCritChance(critChanceIncrease2);

                }
                break;

            case UpgradeType.MaxHpArmorMove:
                float maxHealthIncrease3 = 0.3f;
                int armorIncrease3 = 20;
                float moveSpeedIncrease3 = -0.10f;
         
                playerHealth.IncreaseMaxHealth(maxHealthIncrease3);
                playerHealth.IncreaseArmor(armorIncrease3);
                playerMovement.IncreaseMoveSpeed(moveSpeedIncrease3);
                
                break;

            case UpgradeType.AttackSpeedHp:
                float attackSpeedIncrease4 = 0.20f;
                float maxHealthIncrease4 = -0.15f;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackSpeed(attackSpeedIncrease4);
                }
                playerHealth.IncreaseMaxHealth(maxHealthIncrease4);
                break;

            case UpgradeType.DamageMove:
                float damageIncrease5 = 0.2f;
                float moveSpeedIncrease5 = -0.05f;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseDamage(damageIncrease5);
                }
                playerMovement.IncreaseMoveSpeed(moveSpeedIncrease5);
                break;

            case UpgradeType.DamageRegen:
                float damageIncrease6 = 0.15f;
                float regenIncrease6 = -0.10f;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseDamage(damageIncrease6);
                }
                playerHealth.IncreaseHealthRegen(regenIncrease6);

                break;

            case UpgradeType.CritChanceDamage:
                float critChanceIncrease7 = 0.1f;
                float damageIncrease7 = -0.03f;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseCritChance(critChanceIncrease7);
                    weapon.IncreaseDamage(damageIncrease7);

                }
                break;
            case UpgradeType.AttackSpeedCritArmor:
                float attackSpeedDecrease = -0.05f;
                float critChanceIncrease = 0.05f;
                int armorIncrease = 10;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackSpeed(attackSpeedDecrease);
                    weapon.IncreaseCritChance(critChanceIncrease);
                }
                playerHealth.IncreaseArmor(armorIncrease);
                break;

            case UpgradeType.AttackRangeMoveSpeed:
                float attackRangeIncrease = 0.15f;
                float moveSpeedDecrease = -0.03f;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackRange(attackRangeIncrease);
                }
                playerMovement.IncreaseMoveSpeed(moveSpeedDecrease);
                break;

            case UpgradeType.RegenLuck:
                float regenIncrease = 0.20f;
                int luckDecrease = -10;

                playerHealth.IncreaseHealthRegen(regenIncrease);
                playerHealth.IncreaseLuck(luckDecrease);
                break;

            case UpgradeType.LifestealDamage:
                float lifestealIncrease = 0.15f;
                float damageDecrease = -0.03f;

                playerHealth.IncreaseLifesteal(lifestealIncrease);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseDamage(damageDecrease);
                }
                break;

            case UpgradeType.InvestmentLuckMaxHp:
                int investmentIncrease2 = 75;
                int luckIncrease2 = 30;
                float maxHealthDecrease = -0.15f;

                playerHealth.IncreaseInvestment(investmentIncrease2);
                playerHealth.IncreaseLuck(luckIncrease2);
                playerHealth.IncreaseMaxHealth(maxHealthDecrease);
                break;

            case UpgradeType.MoveSpeedDamage:
                float moveSpeedIncrease = 0.15f;
                float damageDecrease2 = -0.05f;

                playerMovement.IncreaseMoveSpeed(moveSpeedIncrease);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseDamage(damageDecrease2);
                }
                break;

            case UpgradeType.ArmorAttackRange:
                int armorIncrease2 = 20;
                float attackRangeDecrease = -0.05f;

                playerHealth.IncreaseArmor(armorIncrease2);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackRange(attackRangeDecrease);
                }
                break;

            case UpgradeType.CritDamageCritChance1:
                float critDamageIncrease = 20f;
                float critChanceDecrease = -0.05f;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseCritDamage(critDamageIncrease);
                    weapon.IncreaseCritChance(critChanceDecrease);
                }
                break;

            case UpgradeType.LuckRegen:
                int luckIncrease3 = 40;
                float regenDecrease = -0.15f;

                playerHealth.IncreaseLuck(luckIncrease3);
                playerHealth.IncreaseHealthRegen(regenDecrease);
                break;

            case UpgradeType.PickupRadiusAttackSpeed:
                float pickupRadiusIncrease = 0.20f;
                float attackSpeedDecrease2 = -0.05f;

                playerHealth.IncreasePickupRadius(pickupRadiusIncrease);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackSpeed(attackSpeedDecrease2);
                }
                break;

            case UpgradeType.RegenMoveSpeed:
                float regenIncrease2 = 0.15f;
                float moveSpeedDecrease2 = -0.05f;

                playerHealth.IncreaseHealthRegen(regenIncrease2);
                playerMovement.IncreaseMoveSpeed(moveSpeedDecrease2);
                break;

            case UpgradeType.InvestmentDamageArmor:
                int investmentIncrease3 = 50;
                float damageIncrease3 = 0.03f;
                int armorDecrease = -15;

                playerHealth.IncreaseInvestment(investmentIncrease3);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseDamage(damageIncrease3);
                }
                playerHealth.IncreaseArmor(armorDecrease);
                break;

            case UpgradeType.MaxHpCritChanceMove:
                float maxHealthIncrease = 0.10f;
                float critChanceIncrease3 = 0.05f;
                float moveSpeedDecrease3 = -0.05f;

                playerHealth.IncreaseMaxHealth(maxHealthIncrease);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseCritChance(critChanceIncrease3);
                }
                playerMovement.IncreaseMoveSpeed(moveSpeedDecrease3);
                break;

            case UpgradeType.AttackSpeedLuck:
                float attackSpeedIncrease2 = 0.15f;
                int luckDecrease2 = -30;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackSpeed(attackSpeedIncrease2);
                }
                playerHealth.IncreaseLuck(luckDecrease2);
                break;

            case UpgradeType.LifestealPickupRadius:
                float lifestealIncrease2 = 0.15f;
                float pickupRadiusDecrease = -0.15f;

                playerHealth.IncreaseLifesteal(lifestealIncrease2);
                playerHealth.IncreasePickupRadius(pickupRadiusDecrease);
                break;

            case UpgradeType.CritDamageDamageArmor:
                float critDamageIncrease7 = 25f;
                float damageDecrease3 = -0.05f;
                int armorIncrease7 = 10;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseCritDamage(critDamageIncrease7);
                    weapon.IncreaseDamage(damageDecrease3);
                }
                playerHealth.IncreaseArmor(armorIncrease7);
                break;

            case UpgradeType.AttackSpeedCritChancePickup:
                float attackSpeedIncrease3 = 0.15f;
                float critChanceIncrease1 = 0.03f;
                float pickupRadiusDecrease2 = -0.10f;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackSpeed(attackSpeedIncrease3);
                    weapon.IncreaseCritChance(critChanceIncrease1);
                }
                playerHealth.IncreasePickupRadius(pickupRadiusDecrease2);
                break;

            case UpgradeType.DamageMaxHpArmor:
                float damageIncrease4 = 0.25f;
                float maxHealthDecrease2 = -0.05f;
                int armorDecrease2 = -5;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseDamage(damageIncrease4);
                }
                playerHealth.IncreaseMaxHealth(maxHealthDecrease2);
                playerHealth.IncreaseArmor(armorDecrease2);
                break;

            case UpgradeType.Damage:
                float damageIncrease9 =0.15f;
                foreach (var weapon in playerWeapons) // Применяем к каждому оружию
                {
                    weapon.IncreaseDamage(damageIncrease9);
                }
                break;

            case UpgradeType.CritDamage:
                float critDamageIncrease9 = 20f;
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseCritDamage(critDamageIncrease9);
                }
                break;

            case UpgradeType.AttackSpeed:
                float attackSpeedIncrease9 =0.20f;
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackSpeed(attackSpeedIncrease9);
                }
                break;

            case UpgradeType.CritChance:
                float critChanceIncrease9 = 0.10f;
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseCritChance(critChanceIncrease9);
                }
                break;

            case UpgradeType.AttackRange:
                float attackRangeIncrease9 = 0.20f;
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackRange(attackRangeIncrease9);
                }
                break;

            case UpgradeType.MaxHealth:
                float maxHealthIncrease9 = 0.25f;
                playerHealth.IncreaseMaxHealth(maxHealthIncrease9);
                break;

            case UpgradeType.Armor:
                int armorIncrease9 = 15;
                playerHealth.IncreaseArmor(armorIncrease9);
                break;

            case UpgradeType.HealthRegen:
                float regenIncrease9 = 0.20f;
                playerHealth.IncreaseHealthRegen(regenIncrease9);
                break;

            case UpgradeType.Lifesteal:
                float lifestealIncrease9 = 0.20f;
                playerHealth.IncreaseLifesteal(lifestealIncrease9);
                break;

            case UpgradeType.Investment:
                int investmentIncrease9 = 75;
                playerHealth.IncreaseInvestment(investmentIncrease9);
                break;

            case UpgradeType.PickupRadius:
                float pickupRadiusIncrease9 = 0.40f;
                playerHealth.IncreasePickupRadius(pickupRadiusIncrease9);
                break;

            case UpgradeType.MoveSpeed:
                float moveSpeedIncrease9 = 0.15f;
                playerMovement.IncreaseMoveSpeed(moveSpeedIncrease9);
                break;

            case UpgradeType.Luck:
                float luckIncrease9 = 20;
                playerHealth.IncreaseLuck((int)luckIncrease9);;
                break;
        }
    }
}
