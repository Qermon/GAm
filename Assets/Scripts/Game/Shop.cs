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

    public Button[] buffButtons; // ������ ������ ��� ������ ������
    public TextMeshProUGUI[] buffCostTexts; // ������ ��� ����������� ��������� ������
    private List<Upgrade> upgrades = new List<Upgrade>(); // ������ ��������� ������

    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private PlayerGold playerGold;
    private List<Weapon> playerWeapons; // ������ ������

    public UpgradeOption[] upgradeOptions; // ������ ��������� ������

    // ������ ��� �������� ������ ������
    [SerializeField] private Image[] emptyIcons;



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
        DamageMaxHpArmor
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
        playerWeapons = new List<Weapon>(FindObjectsOfType<Weapon>()); // �������� ��� ������ � ����
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
      

    }

    private void UpdatePlayerStats()
    {
        if (playerHealth != null && playerStatsText != null && playerMovement != null)
        {
            playerStatsText.text = $"��������: {playerHealth.currentHealth}/{playerHealth.maxHealth}\n" +
                                   $"������: {playerHealth.defense}\n" +
                                   $"���������: {playerHealth.lifesteal}%\n" +
                                   $"����������: {playerHealth.investment}\n" +
                                   $"������ �����: {playerHealth.pickupRadius}\n" +
                                   $"�����: {playerHealth.luck}\n" +
                                   $"�������� ��������: {playerMovement.moveSpeed:F1}";
        }
    }

    private void GenerateUpgrades()
    {
        upgrades.Clear();
        for (int i = 0; i < buffButtons.Length; i++)
        {
            upgrades.Add(GenerateRandomUpgrade());
        }
    }

    private Upgrade GenerateRandomUpgrade()
    {
        // ���������� ��������� ������ ��� ������ UpgradeOption
        int randomIndex = Random.Range(0, upgradeOptions.Length);
        UpgradeOption selectedOption = upgradeOptions[randomIndex];

        // ��������� ��������� ��������
        int baseCost = 10;
        int rarityMultiplier = 1; // �������� ��� �� ���� ������ ��������
        int randomCost = baseCost * rarityMultiplier + Random.Range(5, 15);

        // ������� ����� Upgrade, ��������� ��������� UpgradeOption
        return new Upgrade(selectedOption.upgradeType, randomCost, selectedOption.upgradeSprite);
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
        }
    }

    private void PurchaseUpgrade(int index)
    {
        Upgrade selectedUpgrade = upgrades[index];

        if (playerGold.currentGold >= selectedUpgrade.cost)
        {
            playerGold.currentGold -= selectedUpgrade.cost;
            ApplyUpgrade(selectedUpgrade);
            upgrades[index] = GenerateRandomUpgrade();
            UpdateUpgradeUI();
        }
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
                playerHealth.regen += playerHealth.maxHealth * 0.0002f;
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
}
