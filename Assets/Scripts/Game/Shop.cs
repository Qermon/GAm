using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;
using System.Drawing;

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
    private WeaponSelectionManager weaponSelectionManager;
    public Shuriken shuriken;
    private LightningWeapon lightningWeapon;
    private ZeusLight zeusLight;
    private BleedStrike bleedStrike;
    private FireStrike fireStrike;
    private FireBallController fireBallController;
    private BoomerangController boomerangController;
    private KnifeController knifeController;

    public UpgradeOption[] upgradeOptions; // ������ ��������� ������
    private HashSet<UpgradeType> existingBuffs = new HashSet<UpgradeType>(); // HashSet ��� ������������ ������������ ������

    public AudioSource shopSound;

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
        ShieldPerWave,    
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
        Damage,          
        CritDamage,      
        AttackSpeed,  
        CritChance,      
        AttackRange,     
        MaxHealth,       
        Armor,          
        HealthRegen,    
        Lifesteal,    
        Investment,     
        PickupRadius,    
        MoveSpeed,      
        Luck,
        ProjectileSizeBuff_Lighting,
        ProjectileSizeBuff_FireBall,
        ProjectileSizeBuff_Boomerang,
        ProjectileSizeBuff_Shuriken,
        ProjectileSizeBuff_Knife,
        ProjectileSizeBuff_FireStrike,
        ProjectileSizeBuff_BleedStrike,
        ProjectileCountBuff_Shuriken,
        ProjectileCountBuff_Lighting,
        ProjectileCountBounceBuff_ZeusLight,
        ProjectileSlowBuff_BleedStrike,
        ProjectileBurnBuff_FireStrike,
        ProjectileStunBuff_FireBall,
        ProjectileSplit_ZeusLight,
        ProjectileDoubleDamage_Boomerang,
        ProjectileDoubleDamage_Knife,
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

        GameObject shopSoundObject = GameObject.Find("ShopSound");

        if (shopSoundObject != null)
        {
            shopSound = shopSoundObject.GetComponent<AudioSource>();
        }

        knifeController = FindObjectOfType<KnifeController>();
        boomerangController = FindObjectOfType<BoomerangController>();
        fireBallController = FindObjectOfType<FireBallController>();
        fireStrike = FindObjectOfType<FireStrike>();
        bleedStrike = FindObjectOfType<BleedStrike>();
        zeusLight = FindObjectOfType<ZeusLight>();
        lightningWeapon = FindObjectOfType<LightningWeapon>();
        shuriken = FindObjectOfType<Shuriken>();
        weaponSelectionManager = FindObjectOfType<WeaponSelectionManager>();
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

    public void RestartScript()
    {

        // ������� ������� ����� � ��������� ���������
        upgrades.Clear();
        existingBuffs.Clear();

        // ���������� ��������� ����������
        currentRefreshCost = 7;

        // �������������� ������ ��������, ������� ��
        shopPanel.SetActive(false);

        // ��������� ������ ������, �������� � ������ �����������
        lightningWeapon = FindObjectOfType<LightningWeapon>();
        cursorManager = FindObjectOfType<CursorManager>();
        playerWeapons = new List<Weapon>(FindObjectsOfType<Weapon>());
        waveManager = FindObjectOfType<WaveManager>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerGold = FindObjectOfType<PlayerGold>();
        shuriken = FindObjectOfType<Shuriken>();
        zeusLight = FindObjectOfType<ZeusLight>();
        fireStrike = FindObjectOfType<FireStrike>();
        bleedStrike = FindObjectOfType<BleedStrike>();
        fireBallController = FindObjectOfType<FireBallController>();
        boomerangController = FindObjectOfType<BoomerangController>();
        knifeController = FindObjectOfType<KnifeController>();

        GameObject shopSoundObject = GameObject.Find("ShopSound");

        if (shopSoundObject != null)
        {
            shopSound = shopSoundObject.GetComponent<AudioSource>();
        }

        // ���������� ������ � ���������������
        totalAttackSpeedBonus = 0f;
        totalAttackRangeBonus = 0f;
        totalPickupRadiusBonus = 0f;
        totalLifestealBonus = 0f;
        totalRegenBonus = 0f;

        // ���������� ����� ����� � ��������� UI
        GenerateUpgrades();
        UpdateUpgradeUI();
        UpdatePlayerStats();
        UpdateRefreshButton();
    }


    // ��������� ��������� ������ � ����������� ���������
    void UpdateRefreshButton()
    {
        refreshCostText.text = currentRefreshCost.ToString();

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
        if (shopSound != null)
        {
            shopSound.Play();
        }
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

        if (weaponSelectionManager.isShurikenActive)
        {
            shuriken.RecreateShurikens();
        }
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
        float averageDamage = 0f;
        float averageCritDamage = 0f;
        float averageCritChance = 0f;

        if (playerHealth != null && playerStatsText != null && playerMovement != null)
        {
            if (playerWeapons != null && playerWeapons.Count > 0)
            {
                float totalDamage = 0f;
                float totalCritDamage = 0f;
                float totalCritChance = 0f;
                int activeWeaponsCount = 0;

                foreach (var weapon in playerWeapons)
                {
                    if (weapon != null && weapon.IsActive()) // ���������, ������� �� ������
                    {
                        totalDamage += weapon.damage;
                        totalCritDamage += weapon.criticalDamage;
                        totalCritChance += weapon.criticalChance;
                        activeWeaponsCount++;
                    }
                }

                if (activeWeaponsCount > 0)
                {
                    averageDamage = totalDamage / activeWeaponsCount;
                    averageCritDamage = totalCritDamage / activeWeaponsCount;
                    averageCritChance = totalCritChance / activeWeaponsCount;
                }
            }

            UpdateTotalAttackSpeedBonus();
            UpdateTotalAttackRangeBonus();
            UpdateTotalPickupRadiusBonus();
            UpdateTotalLifestealBonus();
            UpdateTotalRegenBonus();
            UpdateRefreshButton();

            playerStatsText.text = $"��������: {FormatStatTextMaxHp((int)playerHealth.maxHealth)}\n" +
                                   $"����: {FormatStatTextDamage((int)averageDamage)}\n" +
                                   $"����. ����: {FormatStatText((int)averageCritDamage)}%\n" +
                                   $"����. ����: {FormatStatText((int)(averageCritChance * 100))}%\n" +
                                   $"�������� �����: {FormatStatText((int)totalAttackSpeedBonus)}%\n" +
                                   $"��������� �����: {FormatStatText((int)totalAttackRangeBonus)}%\n" +
                                   $"�����������: {FormatStatText((int)totalRegenBonus)}%\n" +
                                   $"���������: {FormatStatText((int)totalLifestealBonus)}%\n" +
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

        if (value > playerHealth.baseMaxHealth)
        {
            color = "green"; // ������ ��� ������������� ��������
        }
        else if (value < playerHealth.baseMaxHealth)
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
        float averageDamage = 0f;

        if (playerHealth != null && playerStatsText != null && playerMovement != null)
        {
            if (playerWeapons != null && playerWeapons.Count > 0)
            {
                float totalDamage = 0f;
                int activeWeaponsCount = 0;

                foreach (var weapon in playerWeapons)
                {
                    if (weapon != null && weapon.IsActive()) // ���������, ������� �� ������
                    {
                        totalDamage += weapon.baseDamage;
                        activeWeaponsCount++;
                    }
                }

                if (activeWeaponsCount > 0)
                {
                    averageDamage = totalDamage / activeWeaponsCount;
                }
            }
        }

        string color;

        if (value > averageDamage)
        {
            color = "green"; // ������ ��� ������������� ��������
        }
        else if (value < averageDamage)
        {
            color = "red"; // ������� ��� ������������� ��������
        }
        else
        {
            color = "white";
        }

        // ���������� ������ � ������
        Debug.Log((averageDamage));
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

        // ������� � ��������� ������ ���� ��������� �����
        List<UpgradeOption> availableOptions = FilterAvailableOptions(new List<UpgradeOption>(upgradeOptions));

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
        if (availableOptions.Count == 0)
        {
            return null; // ��� ��������� �����
        }

        int randomIndex = Random.Range(0, availableOptions.Count);
        UpgradeOption selectedOption = availableOptions[randomIndex];

        float currentCost = CalculateCurrentCost();
        int randomAdjustment = Random.Range(-10, 21); // ��������� �������� �� -10 �� +20
        int finalCost = Mathf.Max(0, (int)(currentCost + randomAdjustment));

        availableOptions.RemoveAt(randomIndex);

        return new Upgrade(selectedOption.upgradeType, finalCost, selectedOption.upgradeSprite);
    }

    private void PurchaseUpgrade(int index)
    {
        Upgrade selectedUpgrade = upgrades[index];

        if (playerGold.currentGold >= selectedUpgrade.cost)
        {
            if (oneTimeBuffs.Contains(selectedUpgrade.type) && existingBuffs.Contains(selectedUpgrade.type))
            {
                Debug.Log("���� ���� ��� ������ � ��� ������ ������ �����!");
                return;
            }

            playerGold.currentGold -= selectedUpgrade.cost;
            ApplyUpgrade(selectedUpgrade);

            if (oneTimeBuffs.Contains(selectedUpgrade.type))
            {
                existingBuffs.Add(selectedUpgrade.type);
            }

            // ��������� ������ ��������� �����, �������� ��������� � ����������� �����
            List<UpgradeOption> availableOptions = FilterAvailableOptions(new List<UpgradeOption>(upgradeOptions));

            foreach (var upgrade in upgrades)
            {
                availableOptions.RemoveAll(option => option.upgradeType == upgrade.type || existingBuffs.Contains(option.upgradeType));
            }

            Upgrade newUpgrade = GenerateRandomUpgrade(availableOptions);
            upgrades[index] = newUpgrade; // ��������� ���� �� ������

            UpdateUpgradeUI();
            UpdatePlayerStats();
            playerGold.UpdateGoldDisplay();
        }
    }

    private void InitializeShop()
    {
        List<UpgradeOption> availableOptions = FilterAvailableOptions(new List<UpgradeOption>(upgradeOptions));

        foreach (var upgrade in upgrades)
        {
            availableOptions.RemoveAll(option => option.upgradeType == upgrade.type || existingBuffs.Contains(option.upgradeType));
        }

        for (int i = 0; i < upgrades.Count; i++)
        {
            Upgrade newUpgrade = GenerateRandomUpgrade(availableOptions);
            upgrades[i] = newUpgrade; // ��������� ��������� ����� �� �������
        }

        UpdateUpgradeUI();
    }

    private List<UpgradeOption> FilterAvailableOptions(List<UpgradeOption> availableOptions)
    {
        List<UpgradeOption> filteredOptions = new List<UpgradeOption>();

        foreach (var option in availableOptions)
        {
            switch (option.upgradeType)
            {
                case UpgradeType.ProjectileSizeBuff_Lighting:
                    if (weaponSelectionManager.isLightningActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileSizeBuff_FireBall:
                    if (weaponSelectionManager.isFireBallActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileSizeBuff_Boomerang:
                    if (weaponSelectionManager.isBoomerangActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileSizeBuff_Shuriken:
                    if (weaponSelectionManager.isShurikenActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileSizeBuff_Knife:
                    if (weaponSelectionManager.isKnifeActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileSizeBuff_FireStrike:
                    if (weaponSelectionManager.isFireStrikeActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileSizeBuff_BleedStrike:
                    if (weaponSelectionManager.isBleedStrikeActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileCountBuff_Shuriken:
                    if (weaponSelectionManager.isShurikenActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileCountBuff_Lighting:
                    if (weaponSelectionManager.isLightningActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileCountBounceBuff_ZeusLight:
                    if (weaponSelectionManager.isZeusLightActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileSlowBuff_BleedStrike:
                    if (weaponSelectionManager.isBleedStrikeActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileBurnBuff_FireStrike:
                    if (weaponSelectionManager.isFireStrikeActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileStunBuff_FireBall:
                    if (weaponSelectionManager.isFireBallActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileSplit_ZeusLight:
                    if (weaponSelectionManager.isZeusLightActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileDoubleDamage_Boomerang:
                    if (weaponSelectionManager.isBoomerangActive) filteredOptions.Add(option);
                    break;

                case UpgradeType.ProjectileDoubleDamage_Knife:
                    if (weaponSelectionManager.isKnifeActive) filteredOptions.Add(option);
                    break;
                    
                default:
                    filteredOptions.Add(option);
                    break;
            }
        }
        return filteredOptions;
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
            buffCostTexts[i].text = $"{upgrades[i].cost}";

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
                description = "���� <color=green>1%</color> �������� ������ ��� �������� �����, <color=green>5%</color> �� ����. ��������";
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
                description = "�������� ����� +20%\n�������� -15%";
                break;

            case UpgradeType.DamageMove:
                description = "���� +20%\n�������� ����\n-5%";
                break;

            case UpgradeType.DamageRegen:
                description = "���� +15%\n�����������\n-10%";
                break;

            case UpgradeType.CritChanceDamage:
                description = "���� ����� +10%\n���� -3%";
                break;

            case UpgradeType.AttackSpeedCritArmor:
                description = "���� ����� +5%\n����� +10\n�������� ����� -5%";
                break;

            case UpgradeType.AttackRangeMoveSpeed:
                description = "��������� ����� +15%\n�������� ����\n-3%";
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
                description = "����� +40\n�����������\n-15%";
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
                description = "�������� +10%\n���� ����� +5%\n�������� ������������\n-5%";
                break;

            case UpgradeType.AttackSpeedLuck:
                description = "�������� ����� +15%\n����� -30";
                break;

            case UpgradeType.LifestealPickupRadius:
                description = "��������� +15%\n������ �������\n-15%";
                break;

            case UpgradeType.CritDamageDamageArmor:
                description = "����. ���� +25%\n����� +10\n���� -5%";
                break;

            case UpgradeType.AttackSpeedCritChancePickup:
                description = "�������� ����� +15%\n���� ����� +3%\n������ �������\n-10%";
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

            case UpgradeType.ProjectileSizeBuff_Lighting:
                description = "������ Lighting +10%";
                break;

            case UpgradeType.ProjectileSizeBuff_FireBall:
                description = "������ FireBall +10%";
                break;

            case UpgradeType.ProjectileSizeBuff_Boomerang:
                description = "������ Boomerang +20%";
                break;

            case UpgradeType.ProjectileSizeBuff_Shuriken:
                description = "������ Shuriken +10%";
                break;

            case UpgradeType.ProjectileSizeBuff_Knife:
                description = "������ Knife +10%";
                break;

            case UpgradeType.ProjectileSizeBuff_FireStrike:
                description = "������ Fire Strike +20%";
                break;

            case UpgradeType.ProjectileSizeBuff_BleedStrike:
                description = "������ BleedStrike +20%";
                break;

            case UpgradeType.ProjectileCountBuff_Shuriken:
                description = "���������� Shuriken +1";
                break;

            case UpgradeType.ProjectileCountBuff_Lighting:
                description = "���������� Lighting +1";
                break;

            case UpgradeType.ProjectileCountBounceBuff_ZeusLight:
                description = "���������� �������� +2";
                break;

            case UpgradeType.ProjectileSlowBuff_BleedStrike:
                description = "���������� +10%";
                break;

            case UpgradeType.ProjectileBurnBuff_FireStrike:
                description = "���� �� ������� +25%";
                break;

            case UpgradeType.ProjectileStunBuff_FireBall:
                description = "��������� <color=green>+0.2</color> ���";
                break;

            case UpgradeType.ProjectileSplit_ZeusLight:
                description = "���� ���������� +5%";
                break;

            case UpgradeType.ProjectileDoubleDamage_Boomerang:
                description = "���� �������� ����� +10%";
                break;

            case UpgradeType.ProjectileDoubleDamage_Knife:
                description = "���� ������������� �������� +5%";
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
                float critDamageIncrease1 = 5f;
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
                float damageIncrease9 = 0.15f;
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
                float attackSpeedIncrease9 = 0.20f;
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
                playerHealth.IncreaseLuck((int)luckIncrease9); ;
                break;

            case UpgradeType.ProjectileSizeBuff_Lighting:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "1") // ID - 1 ��� Lighting
                    {
                        weapon.IncreaseProjectileSize(0.10f);
                    }
                }
                break;

            case UpgradeType.ProjectileSizeBuff_FireBall:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "2") // ID - 2 ��� FireBall
                    {
                        weapon.IncreaseProjectileSize(0.10f);
                    }
                }
                break;

            case UpgradeType.ProjectileSizeBuff_Boomerang:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "3") // ID - 3 ��� Boomerang
                    {
                        weapon.IncreaseProjectileSize(0.20f);
                    }
                }
                break;

            case UpgradeType.ProjectileSizeBuff_Shuriken:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "4") // ID - 4 ��� Shuriken
                    {
                        weapon.IncreaseProjectileSize(0.10f);
                        
                    }
                }
                break;

            case UpgradeType.ProjectileSizeBuff_Knife:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "5") // ID - 5 ��� Knife
                    {
                        weapon.IncreaseProjectileSize(0.10f);
                    }
                }
                break;

            case UpgradeType.ProjectileSizeBuff_FireStrike:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "7") // ID - 7 ��� FireStrike
                    {
                        weapon.IncreaseProjectileSize(0.20f);
                    }
                }
                break;

            case UpgradeType.ProjectileSizeBuff_BleedStrike:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "8") // ID - 8 ��� BleedStrike
                    {
                        weapon.IncreaseProjectileSize(0.20f);
                    }
                }
                break;

            case UpgradeType.ProjectileCountBuff_Shuriken:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "4") // ID - 4 ��� Shuriken
                    {
                        shuriken.ShurikenCountBuff();

                    }
                }
                break;

            case UpgradeType.ProjectileCountBuff_Lighting:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "1") 
                    {
                        lightningWeapon.LightingCountBuff();

                    }
                }
                break;

            case UpgradeType.ProjectileCountBounceBuff_ZeusLight:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "6") 
                    {
                        zeusLight.zeusLightCountBounceBuff();

                    }
                }
                break;

            case UpgradeType.ProjectileSlowBuff_BleedStrike:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "8") // ID - 8 ��� BleedStrike
                    {
                        bleedStrike.IncreaseProjectileSlowEffect(0.1f);
                    }
                }
                break;

            case UpgradeType.ProjectileBurnBuff_FireStrike:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "7") // ID - 7 ��� FireStrike
                    {
                        fireStrike.IncreaseProjectileBurnEffect(0.25f);
                    }
                }
                break;

            case UpgradeType.ProjectileStunBuff_FireBall:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "2") // ID - 2 ��� FireBall
                    {
                        fireBallController.IncreaseProjectileStunEffect(0.2f);
                    }
                }
                break;

            case UpgradeType.ProjectileSplit_ZeusLight:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "6") 
                    {
                        zeusLight.IncreaseProjectileSplitEffect(0.05f);
                    }
                }
                break;

            case UpgradeType.ProjectileDoubleDamage_Boomerang:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "3")
                    {
                        boomerangController.IncreaseProjectileDoubleDomageEffect(0.1f);
                    }
                }
                break;

            case UpgradeType.ProjectileDoubleDamage_Knife:
                foreach (var weapon in playerWeapons)
                {
                    if (weapon.weaponID == "5")
                    {
                        knifeController.MomentKill(0.05f);
                    }
                }
                break;

        }
    } 
}