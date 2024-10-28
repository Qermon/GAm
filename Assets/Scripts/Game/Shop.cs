using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public Button closeButton;
    public TextMeshProUGUI playerStatsText;

    public Button[] buffButtons; // Массив кнопок для выбора баффов
    public TextMeshProUGUI[] buffCostTexts; // Тексты для отображения стоимости баффов
    public TextMeshProUGUI[] buffDescriptionTexts; // Поля для вывода описания для каждого баффа
    private List<Upgrade> upgrades = new List<Upgrade>(); // Список доступных баффов

    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private PlayerGold playerGold;
    private List<Weapon> playerWeapons; // Список оружий

    public UpgradeOption[] upgradeOptions; // Массив доступных баффов
    private HashSet<UpgradeType> existingBuffs = new HashSet<UpgradeType>(); // HashSet для отслеживания существующих баффов



    // Массив для хранения пустых иконок
    [SerializeField] private Image[] emptyIcons;



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
        playerWeapons = new List<Weapon>(FindObjectsOfType<Weapon>()); // Получаем все оружия в игре
        shopPanel.SetActive(false);

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseShop);
        }

        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerGold = FindObjectOfType<PlayerGold>();

        for (int i = 0; i < buffButtons.Length; i++)
        {
            int index = i;
            buffButtons[i].onClick.AddListener(() => PurchaseUpgrade(index));
        }

        GenerateUpgrades();
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        Time.timeScale = 0f;
        UpdatePlayerStats();
        UpdateUpgradeUI();

    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1f;
        playerGold.OnShopClosed();

        // Обновляем баффы при закрытии магазина
        GenerateUpgrades();
    }


    private void UpdatePlayerStats()
    {
        if (playerHealth != null && playerStatsText != null && playerMovement != null)
        {
            playerStatsText.text = $"Здоровье: {playerHealth.currentHealth}/{playerHealth.maxHealth}\n" +
                                   $"Защита: {playerHealth.defense}\n" +
                                   $"Вампиризм: {playerHealth.lifesteal}%\n" +
                                   $"Инвестиции: {playerHealth.investment}\n" +
                                   $"Радиус сбора: {playerHealth.pickupRadius}\n" +
                                   $"Удача: {playerHealth.luck}\n" +
                                   $"Скорость движения: {playerMovement.moveSpeed:F1}";
        }
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

        // Генерация стоимости апгрейда
        int baseCost = 10;
        int randomCost = baseCost + Random.Range(5, 15);

        // Удаляем выбранный вариант из доступных
        availableOptions.RemoveAt(randomIndex);

        return new Upgrade(selectedOption.upgradeType, randomCost, selectedOption.upgradeSprite);
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
            playerGold.currentGold -= selectedUpgrade.cost;
            ApplyUpgrade(selectedUpgrade);

            // Обновляем список доступных опций, исключая текущие баффы
            List<UpgradeOption> availableOptions = new List<UpgradeOption>(upgradeOptions);
            foreach (var upgrade in upgrades)
            {
                availableOptions.RemoveAll(option => option.upgradeType == upgrade.type);
            }

            // Генерируем новый уникальный бафф, который отсутствует на других слотах
            Upgrade newUpgrade = GenerateRandomUpgrade(availableOptions);
            upgrades[index] = newUpgrade; // Обновляем бафф на кнопке

            UpdateUpgradeUI();
            UpdatePlayerStats(); // Обновляем характеристики героя
            playerGold.UpdateGoldDisplay();
        }
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
        switch (upgradeType)
        {
            case UpgradeType.ShieldPerWave:
                return "Добавляет щит в начале каждой волны, блокируя входящий урон.";

            case UpgradeType.ShieldOnKill:
                return "5% шанс получить щит, равный 10% от макс. здоровья, при убийстве врага.";

            case UpgradeType.BarrierOnLowHealth:
                return "При снижении здоровья ниже 50% раз в волну даёт барьер в 20% от макс. здоровья.";

            case UpgradeType.HealthRegenPerWave:
                return "Регенерация здоровья 2% от макс. здоровья, обнуляется в начале каждой волны.";

            case UpgradeType.CritChanceBuff:
                return "Увеличивает шанс критического удара всех видов оружия на 5%.";

            case UpgradeType.CritDamageBuff:
                return "Увеличивает урон от критического удара всех видов оружия.";

            case UpgradeType.AttackSpeedDamage:
                return "+20% скорости атаки, но -5% урона всех видов оружия.";

            case UpgradeType.AttackSpeedDamageCritMove:
                return "+15% скорости атаки и +5% урона, но снижает крит. урон на 5% и скорость передвижения на 10%.";

            case UpgradeType.CritDamageCritChance:
                return "+30% крит. урона, но -1% шанс крит. удара.";

            case UpgradeType.MaxHpArmorMove:
                return "+30% макс. здоровья и +20 брони, но -15% к скорости передвижения.";

            case UpgradeType.AttackSpeedHp:
                return "+20% скорости атаки, но -20% макс. здоровья.";

            case UpgradeType.DamageMove:
                return "+20% урона, но -10% к скорости передвижения.";

            case UpgradeType.DamageRegen:
                return "+15% урона, но -10% регенерации здоровья.";

            case UpgradeType.CritChanceDamage:
                return "+10% шанс крит. удара, но -5% урона.";

            case UpgradeType.AttackSpeedCritArmor:
                return "-5% к скорости атаки, но +5% крит. шанс и +10 к броне.";

            case UpgradeType.AttackRangeMoveSpeed:
                return "+15% дальности атаки, но -5% скорости передвижения.";

            case UpgradeType.RegenLuck:
                return "+20% регенерации здоровья, но -20 удачи.";

            case UpgradeType.LifestealDamage:
                return "+15% вампиризма, но -10% урона.";

            case UpgradeType.InvestmentLuckMaxHp:
                return "+75% инвестиции и +30 удачи, но -25% макс. здоровья.";

            case UpgradeType.MoveSpeedDamage:
                return "+15% к скорости передвижения, но -10% урона.";

            case UpgradeType.ArmorAttackRange:
                return "+20 брони, но -10% дальности атаки.";

            case UpgradeType.CritDamageCritChance1:
                return "+30% крит. урона, но -15% шанс крит. удара.";

            case UpgradeType.LuckRegen:
                return "+40 удачи, но -15% регенерации здоровья.";

            case UpgradeType.PickupRadiusAttackSpeed:
                return "+20% радиуса подбора, но -3% скорости атаки.";

            case UpgradeType.RegenMoveSpeed:
                return "+15% регенерации здоровья, но -5% скорости передвижения.";

            case UpgradeType.InvestmentDamageArmor:
                return "+50% инвестиции и +3% урона, но -15 брони.";

            case UpgradeType.MaxHpCritChanceMove:
                return "+10% макс. здоровья и +10% крит. шанс, но -10% скорости передвижения.";

            case UpgradeType.AttackSpeedLuck:
                return "+15% скорости атаки, но -30 удачи.";

            case UpgradeType.LifestealPickupRadius:
                return "+15% вампиризма, но -15% радиуса подбора.";

            case UpgradeType.CritDamageDamageArmor:
                return "+25% крит. урона и +10 брони, но -10% урона.";

            case UpgradeType.AttackSpeedCritChancePickup:
                return "+15% скорости атаки и +3% крит. шанс, но -10% радиуса подбора.";

            case UpgradeType.DamageMaxHpArmor:
                return "+25% урона, но -10% макс. здоровья и -5 брони.";

            case UpgradeType.Damage:
                return "+15% урона для всех видов оружия.";

            case UpgradeType.CritDamage:
                return "+20% крит. урона для всех видов оружия.";

            case UpgradeType.AttackSpeed:
                return "+20% скорости атаки для всех видов оружия.";

            case UpgradeType.CritChance:
                return "+10% крит. шанс для всех видов оружия.";

            case UpgradeType.AttackRange:
                return "+20% дальности атаки для всех видов оружия.";

            case UpgradeType.MaxHealth:
                return "+25% макс. здоровья.";

            case UpgradeType.Armor:
                return "+15 брони.";

            case UpgradeType.HealthRegen:
                return "+20% регенерации здоровья.";

            case UpgradeType.Lifesteal:
                return "+20% вампиризма.";

            case UpgradeType.Investment:
                return "+75% инвестиции на старт каждой волны.";

            case UpgradeType.PickupRadius:
                return "+40% радиуса подбора.";

            case UpgradeType.MoveSpeed:
                return "+15% к скорости передвижения.";

            case UpgradeType.Luck:
                return "+20 к удаче.";

            default:
                return "Неизвестный бафф.";
        }
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
                playerHealth.regen += playerHealth.maxHealth * 0.0002f;
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
                float critChanceIncrease2 = -0.01f;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseCritDamage(critDamageIncrease2);
                    weapon.IncreaseCritChance(critChanceIncrease2);

                }
                break;

            case UpgradeType.MaxHpArmorMove:
                float maxHealthIncrease3 = 0.3f;
                int armorIncrease3 = 20;
                float moveSpeedIncrease3 = -0.15f;
         
                playerHealth.IncreaseMaxHealth(maxHealthIncrease3);
                playerHealth.IncreaseArmor(armorIncrease3);
                playerMovement.IncreaseMoveSpeed(moveSpeedIncrease3);
                
                break;

            case UpgradeType.AttackSpeedHp:
                float attackSpeedIncrease4 = 0.20f;
                float maxHealthIncrease4 = -0.2f;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackSpeed(attackSpeedIncrease4);
                }
                playerHealth.IncreaseMaxHealth(maxHealthIncrease4);
                break;

            case UpgradeType.DamageMove:
                float damageIncrease5 = 0.2f;
                float moveSpeedIncrease5 = -0.1f;

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
                float damageIncrease7 = -0.05f;

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
                float moveSpeedDecrease = -0.05f;

                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackRange(attackRangeIncrease);
                }
                playerMovement.IncreaseMoveSpeed(moveSpeedDecrease);
                break;

            case UpgradeType.RegenLuck:
                float regenIncrease = 0.20f;
                int luckDecrease = -20;

                playerHealth.IncreaseHealthRegen(regenIncrease);
                playerHealth.IncreaseLuck(luckDecrease);
                break;

            case UpgradeType.LifestealDamage:
                float lifestealIncrease = 0.15f;
                float damageDecrease = -0.10f;

                playerHealth.IncreaseLifesteal(lifestealIncrease);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseDamage(damageDecrease);
                }
                break;

            case UpgradeType.InvestmentLuckMaxHp:
                int investmentIncrease2 = 75;
                int luckIncrease2 = 30;
                float maxHealthDecrease = -0.25f;

                playerHealth.IncreaseInvestment(investmentIncrease2);
                playerHealth.IncreaseLuck(luckIncrease2);
                playerHealth.IncreaseMaxHealth(maxHealthDecrease);
                break;

            case UpgradeType.MoveSpeedDamage:
                float moveSpeedIncrease = 0.15f;
                float damageDecrease2 = -0.10f;

                playerMovement.IncreaseMoveSpeed(moveSpeedIncrease);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseDamage(damageDecrease2);
                }
                break;

            case UpgradeType.ArmorAttackRange:
                int armorIncrease2 = 20;
                float attackRangeDecrease = -0.10f;

                playerHealth.IncreaseArmor(armorIncrease2);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackRange(attackRangeDecrease);
                }
                break;

            case UpgradeType.CritDamageCritChance1:
                float critDamageIncrease = 0.30f;
                float critChanceDecrease = -0.15f;

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
                float attackSpeedDecrease2 = -0.03f;

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
                float critChanceIncrease3 = 0.10f;
                float moveSpeedDecrease3 = -0.10f;

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
                float critDamageIncrease7 = 0.25f;
                float damageDecrease3 = -0.10f;
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
                float maxHealthDecrease2 = -0.10f;
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
