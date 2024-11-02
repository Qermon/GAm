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

    public Button[] buffButtons; // ������ ������ ��� ������ ������
    public TextMeshProUGUI[] buffCostTexts; // ������ ��� ����������� ��������� ������
    public TextMeshProUGUI[] buffDescriptionTexts; // ���� ��� ������ �������� ��� ������� �����
    private List<Upgrade> upgrades = new List<Upgrade>(); // ������ ��������� ������
    
    public Button refreshButton; // ������ ����������
    private int currentRefreshCost = 7; // ��������� ��������� ����������
    public TextMeshProUGUI refreshCostText;  // ��������� ������ ��� ����������� ��������� ����������
    private float baseCost = 50f; // ���������� float ��� ����� ������ ����������
    private float priceIncreasePercentage = 0.1f; // 10% ����������


    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private PlayerGold playerGold;
    private LevelUpMenu levelUpMenu;
    private WaveManager waveManager;
    private List<Weapon> playerWeapons; // ������ ������
    private Weapon weapon;
    private CursorManager cursorManager;
    

    public UpgradeOption[] upgradeOptions; // ������ ��������� ������
    private HashSet<UpgradeType> existingBuffs = new HashSet<UpgradeType>(); // HashSet ��� ������������ ������������ ������

    private float totalAttackSpeedBonus = 0f;
    private float totalAttackRangeBonus = 0f;
    private float totalPickupRadiusBonus = 0f;
    private float totalLifestealBonus = 0f;
    private float totalRegenBonus = 0f;

    // ������ ��� �������� ������ ������
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
        ShieldPerWave,    // ������ �� �����
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
        Damage,          // ����
        CritDamage,      // ����������� ����
        AttackSpeed,     // �������� �����
        CritChance,      // ���� ������������ �����
        AttackRange,     // ��������� �����
        MaxHealth,       // ��������
        Armor,           // �����
        HealthRegen,     // ����������� ��������
        Lifesteal,       // ���������
        Investment,      // ����������
        PickupRadius,    // ������ �����
        MoveSpeed,       // �������� ����
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
        playerWeapons = new List<Weapon>(FindObjectsOfType<Weapon>()); // �������� ��� ������ � ����
        waveManager = FindObjectOfType<WaveManager>();
        shopPanel.SetActive(false);

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseShop);
        }

        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(RefreshBuffs); // ��������� ���������� ��� ������ ����������
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

    // ��������� ��������� ������ � ����������� ���������
    void UpdateRefreshButton()
    {
        refreshCostText.text = currentRefreshCost.ToString() + " Gold";

        // ���������, ������� �� ������
        if (playerGold.currentGold >= currentRefreshCost)
        {
            refreshButton.interactable = true;  // ���������� ������
        }
        else
        {
            refreshButton.interactable = false; // ������������ ������
        }
    }

    public void OpenShop()
    {
        cursorManager.ShowCursor();
        InitializeShop(); // ������������� ��������� ������
        shopPanel.SetActive(true);
        Time.timeScale = 0f;
        currentRefreshCost = 7; // ���������� ��������� ���������� �� ���������� ��������
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

        // ��������� ����� ��� �������� ��������
        GenerateUpgrades();
    }

    private void TryRefreshBuffs()
    {
        if (playerGold.currentGold >= currentRefreshCost)
        {
            playerGold.currentGold -= currentRefreshCost; // �������� ������ �� ����������
            RefreshBuffs();
            IncreaseRefreshCost();
            UpdateRefreshButton();
            playerGold.UpdateGoldDisplay();
        }
        else
        {
            Debug.Log("������������ ������ ��� ���������� ��������!");
        }
    }
    private void IncreaseRefreshCost()
    {
        // ����������� ��������� ���������� ����������
        currentRefreshCost = currentRefreshCost * 2 + Random.Range(0, 6);
    }

    private void RefreshBuffs()
    {

        InitializeShop();
        UpdateUpgradeUI();   // ��������� UI
    }


    private void UpdatePlayerStats()
    {
        float moveSpeed = playerMovement.moveSpeed * 200;
        float averageDamage = 0f; // ��������� ���������� ��� ����� if
        float averageCritDamage = 0f; // ��� �������� ������������ �����
        float averageCritChance = 0f; // ��� �������� ������������ �����

        if (playerHealth != null && playerStatsText != null && playerMovement != null)
        {
            if (playerWeapons != null && playerWeapons.Count > 0)
            {
                // ��������� ���� ���� ������
                float totalDamage = 0f;
                float totalCritDamage = 0f; // ��� ������������ ������������ �����
                float totalCritChance = 0f; // ��������� ����������� ����

                foreach (var weapon in playerWeapons)
                {
                    totalDamage += weapon.damage; // ��������������, ��� � ��� ���� �������� damage � ������ Weapon
                    totalCritDamage += weapon.criticalDamage; // ��������� ����������� ����
                    totalCritChance += weapon.criticalChance; // ��������� ����������� ����
                }

                averageDamage = totalDamage / playerWeapons.Count; // ������� ����
                averageCritDamage = totalCritDamage / playerWeapons.Count; // ������� ����������� ����
                averageCritChance = totalCritChance / playerWeapons.Count; // ������� ����������� ����
            }

            UpdateTotalAttackSpeedBonus();
            UpdateTotalAttackRangeBonus();
            UpdateTotalPickupRadiusBonus();
            UpdateTotalLifestealBonus();
            UpdateTotalRegenBonus();
            UpdateRefreshButton();

            playerStatsText.text = $"��������: {FormatStatTextMaxHp((int)playerHealth.maxHealth)}\n" +
                                   $"����: {FormatStatTextDamage((int)averageDamage)}\n" + // ���������� ������� ����
                                   $"����. ����: {FormatStatText((int)averageCritDamage)}%\n" + // ����������� ����
                                   $"����. ����: {FormatStatText((int)(averageCritChance * 100))}%\n" + // ����������� ����
                                   $"�������� �����: {FormatStatText((int)totalAttackSpeedBonus)}%\n" + // ����� �������� �����
                                   $"��������� �����: {FormatStatText((int)totalAttackRangeBonus)}%\n" + // ����� ��������� �����
                                   $"�����������: {FormatStatText((int)totalRegenBonus)}%\n" + // �����������
                                   $"���������: {FormatStatText((int)totalLifestealBonus)}%\n" + // ���������
                                   $"������: {FormatStatText(playerHealth.defense)}\n" +
                                   $"�������� ����: {FormatStatTextMoveSpeed((int)moveSpeed)}\n" +
                                   $"������ �����: {FormatStatText((int)totalPickupRadiusBonus)}%\n" +
                                   $"����������: {FormatStatText((int)playerHealth.investment)}\n" +
                                   $"�����: {FormatStatText((int)playerHealth.luck)}\n";
        }
    }

    private string FormatStatText(int value)
    {
        string color;

        if (value > 0)
        {
            color = "green"; // ������ ��� ������������� ��������
        }
        else if (value < 0)
        {
            color = "red"; // ������� ��� ������������� ��������
        }
        else
        {
            color = "white"; // ����� ��� 0
        }

        // ���������� ������ � ������
        return $"<color={color}>{value}</color>";
    }

    private string FormatStatTextMaxHp(int value)
    {
        string color;

        if (value > playerHealth.baseMaxHp)
        {
            color = "green"; // ������ ��� ������������� ��������
        }
        else if (value < playerHealth.baseMaxHp)
        {
            color = "red"; // ������� ��� ������������� ��������
        }
        else
        {
            color = "white";
        }

        // ���������� ������ � ������
        return $"<color={color}>{value}</color>";
    }

    
    
    private string FormatStatTextMoveSpeed(int value)
    {
        string color;

        if (value > 240)
        {
            color = "green"; // ������ ��� ������������� ��������
        }
        else if (value < 240)
        {
            color = "red"; // ������� ��� ������������� ��������
        }
        else
        {
            color = "white"; // ����� ��� 240
        }

        // ���������� ������ � ������
        return $"<color={color}>{value}</color>";
    }

    private string FormatStatTextDamage(int value)
    {
        string color;

        if (value > 27)
        {
            color = "green"; // ������ ��� ������������� ��������
        }
        else if (value < 27)
        {
            color = "red"; // ������� ��� ������������� ��������
        }
        else
        {
            color = "white"; // ����� ��� 27
        }

        // ���������� ������ � ������
        return $"<color={color}>{value}</color>";
    }




    public void UpdateTotalAttackSpeedBonus()
    {
        totalAttackSpeedBonus = 0f;

        if (playerWeapons.Count > 0)
        {
            var weapon = playerWeapons[0]; // ���� ������ ������
            totalAttackSpeedBonus = (weapon.attackSpeed - weapon.baseAttackSpeed) / weapon.baseAttackSpeed * 100;
        }
    }

    public void UpdateTotalAttackRangeBonus()
    {
        totalAttackRangeBonus = 0f;

        if (playerWeapons.Count > 0)
        {
            var weapon = playerWeapons[0]; // ���� ������ ������
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
        upgrades.Clear(); // �������� ������ ��������� ������
        HashSet<UpgradeType> usedTypes = new HashSet<UpgradeType>(); // ��� ������������ �������������� �����

        // �������� ������ ���� ��������� �����
        List<UpgradeOption> availableOptions = new List<UpgradeOption>(upgradeOptions);

        while (upgrades.Count < buffButtons.Length)
        {
            Upgrade newUpgrade = GenerateRandomUpgrade(availableOptions);

            // ��������, ���������� �� ��� ����� ��� �����
            if (!usedTypes.Contains(newUpgrade.type))
            {
                upgrades.Add(newUpgrade);
                usedTypes.Add(newUpgrade.type); // �������� ��� ����� � ������������
            }
        }

        // ���������� ���������� ����� ��������� ���������� ������
        UpdateUpgradeUI();
    }




    private Upgrade GenerateRandomUpgrade(List<UpgradeOption> availableOptions)
    {
        // �������� �� ������ ������ ��������� �����
        if (availableOptions.Count == 0)
        {
            return null; // ��� ����������� ������, ����� ��� ��������� �����
        }

        // ��������� ���������� �������
        int randomIndex = Random.Range(0, availableOptions.Count);
        UpgradeOption selectedOption = availableOptions[randomIndex];

        // ��������� ��������� �������� � ������ ���������� � ���������� ��������
        float currentCost = CalculateCurrentCost();
        int randomAdjustment = Random.Range(-10, 21); // ��������� �������� �� -10 �� +20
        int finalCost = Mathf.Max(0, (int)(currentCost + randomAdjustment)); // ���������, ��� ��������� �� �������������

        // ������� ��������� ������� �� ���������
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

            // ������������� ������ �� ������ ������
            if (emptyIcons[i] != null && upgrades[i].upgradeSprite != null)
            {
                emptyIcons[i].sprite = upgrades[i].upgradeSprite;
            }
            else if (emptyIcons[i] != null)
            {
                emptyIcons[i].sprite = null; // ���������� ������, ���� ��� �������
            }

            // ��������� ����� �������� ��� ���������������� �����
            if (i < buffDescriptionTexts.Length) // ��������, ����� �������� ������ �� ������� �������
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
            // ���������, ��� �� ��� ������ ���� ����
            if (oneTimeBuffs.Contains(selectedUpgrade.type) && existingBuffs.Contains(selectedUpgrade.type))
            {
                Debug.Log("���� ���� ��� ������ � ��� ������ ������ �����!");
                return; // �� ��������� ������ ���� ���� �����
            }

            playerGold.currentGold -= selectedUpgrade.cost;
            ApplyUpgrade(selectedUpgrade);

            // ��������� ���� � ������ ���������, ���� ��� ����, ������� ����� ������ ������ ���� ���
            if (oneTimeBuffs.Contains(selectedUpgrade.type))
            {
                existingBuffs.Add(selectedUpgrade.type);
            }

            // ��������� ������ ��������� �����, �������� ��� ��������� � ������� �����
            List<UpgradeOption> availableOptions = new List<UpgradeOption>(upgradeOptions);

            // ������� ��� ��������� � ����������� ����� �� ��������� �����
            foreach (var upgrade in upgrades)
            {
                availableOptions.RemoveAll(option => option.upgradeType == upgrade.type || existingBuffs.Contains(option.upgradeType));
            }

            // ���������� ����� ���������� ����, ������� ����������� �� ������ ������
            Upgrade newUpgrade = GenerateRandomUpgrade(availableOptions);
            upgrades[index] = newUpgrade; // ��������� ���� �� ������

            UpdateUpgradeUI();
            UpdatePlayerStats(); // ��������� �������������� �����
            playerGold.UpdateGoldDisplay();
        }
    }

    private void InitializeShop()
    {
        // �������� ������ ��������� �����
        List<UpgradeOption> availableOptions = new List<UpgradeOption>(upgradeOptions);

        // ������� ��� ��������� � ����������� ����� �� ��������� �����
        foreach (var upgrade in upgrades)
        {
            availableOptions.RemoveAll(option => option.upgradeType == upgrade.type || existingBuffs.Contains(option.upgradeType));
        }

        // ���������� ����� ��������� �����
        for (int i = 0; i < upgrades.Count; i++) // �������� Length �� Count
        {
            Upgrade newUpgrade = GenerateRandomUpgrade(availableOptions);
            upgrades[i] = newUpgrade; // ��������� ��������� ����� �� �������
        }

        UpdateUpgradeUI(); // ��������� ���������������� ��������� ��������
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
                description = "������ <color=green>30%</color> �� ���� �� ������ ����� ���� �� �� ������ �� <color=green>29%</color>";
                break;

            case UpgradeType.ShieldOnKill:
                description = "���� <color=green>5%</color> �������� ������ ��� �������� �����, <color=green>10%</color> �� ����. ��������";
                break;

            case UpgradeType.BarrierOnLowHealth:
                description = "��� �������� �������� ���� <color=green>50%</color> ��� �� ����� ��� ������ � <color=green>20%</color> �� ����. ��������";
                break;

            case UpgradeType.HealthRegenPerWave:
                description = "����������� +200%, �� � ���� <color=green>30%</color> �� � ������ ������ �����";
                break;

            case UpgradeType.CritChanceBuff:
                description = "������ ������� ����������� ���� ���� �� <color=green>0.5%</color> �� ����� �����";
                break;

            case UpgradeType.CritDamageBuff:
                description = "������ ������� ����������� ���� ���� �� <color=green>1%</color> �� ����� �����";
                break;

            case UpgradeType.AttackSpeedDamage:
                description = "�������� ����� +20%\n���� -5%";
                break;

            case UpgradeType.AttackSpeedDamageCritMove:
                description = "�������� ����� +15%\n���� +5%\n���� ���� +5%\n�������� ���� -10%";
                break;

            case UpgradeType.CritDamageCritChance:
                description = "����. ���� +30%\n���� ����� -10%";
                break;

            case UpgradeType.MaxHpArmorMove:
                description = "�������� +30%\n����� +20\n�������� ���� -10%";
                break;

            case UpgradeType.AttackSpeedHp:
                description = "�������� -15%\n�������� ����� +20%";
                break;

            case UpgradeType.DamageMove:
                description = "���� +20%\n�������� ���� -5%";
                break;

            case UpgradeType.DamageRegen:
                description = "���� +15%\n�����������  -10%";
                break;

            case UpgradeType.CritChanceDamage:
                description = "���� ����� +10%\n���� -3%";
                break;

            case UpgradeType.AttackSpeedCritArmor:
                description = "���� ����� +5%\n����� +10\n�������� ����� -5%";
                break;

            case UpgradeType.AttackRangeMoveSpeed:
                description = "��������� ����� +15%\n�������� ���� -3%";
                break;

            case UpgradeType.RegenLuck:
                description = "����������� +20%\n����� -20";
                break;

            case UpgradeType.LifestealDamage:
                description = "��������� +15%\n���� -3%";
                break;

            case UpgradeType.InvestmentLuckMaxHp:
                description = "���������� +75\n����� +30\n�������� -15%";
                break;

            case UpgradeType.MoveSpeedDamage:
                description = "�������� ���� +15%\n���� -5%";
                break;

            case UpgradeType.ArmorAttackRange:
                description = "����� +20\n��������� ����� -5%";
                break;

            case UpgradeType.CritDamageCritChance1:
                description = "����. ���� +20%\n���� ����� -5%";
                break;

            case UpgradeType.LuckRegen:
                description = "����� +40\n����������� -15%";
                break;

            case UpgradeType.PickupRadiusAttackSpeed:
                description = "������ ������� +20%\n�������� ����� -5%";
                break;

            case UpgradeType.RegenMoveSpeed:
                description = "����������� +15%\n�������� ���� -5%";
                break;

            case UpgradeType.InvestmentDamageArmor:
                description = "���������� +50\n���� +3%\n����� -15";
                break;

            case UpgradeType.MaxHpCritChanceMove:
                description = "�������� +10%\n���� ����� +5%\n�������� ������������  -5%";
                break;

            case UpgradeType.AttackSpeedLuck:
                description = "�������� ����� +15%\n����� -30";
                break;

            case UpgradeType.LifestealPickupRadius:
                description = "��������� +15%\n������ ������� -15%";
                break;

            case UpgradeType.CritDamageDamageArmor:
                description = "����. ���� +25%\n����� +10\n���� -5%";
                break;

            case UpgradeType.AttackSpeedCritChancePickup:
                description = "�������� ����� +15%\n���� ����� +3%\n������ �������  -10%";
                break;

            case UpgradeType.DamageMaxHpArmor:
                description = "���� +25%\n�������� -5%\n����� -5";
                break;

            case UpgradeType.Damage:
                description = "���� +15%";
                break;

            case UpgradeType.CritDamage:
                description = "����. ���� +20%";
                break;

            case UpgradeType.AttackSpeed:
                description = "�������� ����� +20%";
                break;

            case UpgradeType.CritChance:
                description = "���� ����� +10%";
                break;

            case UpgradeType.AttackRange:
                description = "��������� ����� +20%";
                break;

            case UpgradeType.MaxHealth:
                description = "�������� +25%";
                break;

            case UpgradeType.Armor:
                description = "����� +15";
                break;

            case UpgradeType.HealthRegen:
                description = "����������� +20%";
                break;

            case UpgradeType.Lifesteal:
                description = "��������� +20%";
                break;

            case UpgradeType.Investment:
                description = "���������� +75";
                break;

            case UpgradeType.PickupRadius:
                description = "������ ������� +40%";
                break;

            case UpgradeType.MoveSpeed:
                description = "�������� ���� +15%";
                break;

            case UpgradeType.Luck:
                description = "����� +20";
                break;

            default:
                description = "����������� ����";
                break;
        }

        return FormatDescription(description);
    }


    private string FormatDescription(string description)
    {
        // ���� �������� � + � �������� ������ ������
        description = Regex.Replace(description, @"\+\d+%?", match => $"<color=green>{match.Value}</color>");

        // ���� �������� � - � �������� ������� ������
        description = Regex.Replace(description, @"-\d+%?", match => $"<color=red>{match.Value}</color>");

        return description;
    }





    private void ApplyUpgrade(Upgrade upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeType.ShieldPerWave:
                playerHealth.AddShieldBuff(); // ����������� ���������� �������������� ������
                playerHealth.ActivateShield(); // ���������� ���
                Debug.Log("���� ShieldPerWave ��������: ��� �����������.");
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

            case UpgradeType.CritChanceBuff: // ����� ��� �������� ��� ���� �����
                foreach (var weapon in FindObjectsOfType<Weapon>())
                {
                    weapon.PurchaseCritChanceBuff(); // �������� � ���������� ���� ��� ���� ������
                }
                break;

            case UpgradeType.CritDamageBuff: // ����� ��� �������� ��� ���� �����
                foreach (var weapon in FindObjectsOfType<Weapon>())
                {
                    weapon.PurchaseCritDamageBuff(); // �������� � ���������� ���� ��� ���� ������
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
                foreach (var weapon in playerWeapons) // ��������� � ������� ������
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
