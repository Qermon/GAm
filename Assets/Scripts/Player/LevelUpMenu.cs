using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public enum UpgradeType
{
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
    Luck             // �����
}

public enum UpgradeRarity
{
    Common = 0,    // 1 ������
    Uncommon = 1,  // 2 ������
    Rare = 2       // 3 ������
}


[System.Serializable]
public class UpgradeOption
{
    public string upgradeName;
    public Sprite upgradeSprite;
    public UpgradeType upgradeType;
    public UpgradeRarity upgradeRarity; // ������� ��������
}

public class LevelUpMenu : MonoBehaviour
{

    public GameObject levelUpPanel;
    public Image[] upgradeIcons;
    public Button[] upgradeButtons;
    public TMP_Text[] upgradeTexts; // ������, ������� ����� ���������� �������� ���������
    public Sprite[] raritySprites; // ������ �������� ��� ���������
    public Image[] rarityImages; // ������ ��� ����������� ���������
    public List<UpgradeOption> upgradeOptions;

    public WaveManager waveManager;

    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private List<Weapon> playerWeapons; // ������ ������
    private int waveNumber; // ���������� ��� �������� ������� �����

    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerWeapons = new List<Weapon>(FindObjectsOfType<Weapon>()); // �������� ��� ������ � ����
        waveManager = FindObjectOfType<WaveManager>(); // ���� WaveManager � �����


        levelUpPanel.SetActive(false);
        foreach (Image icon in upgradeIcons)
        {
            icon.gameObject.SetActive(false);
        }
    }

    public void OpenLevelUpMenu()
    {
        waveNumber = waveManager.GetWaveNumber(); // ��������� �������� waveNumber

        if (upgradeOptions.Count < 3)
        {
            Debug.LogError("������������ ��������� ���������.");
            return;
        }

        List<UpgradeOption> selectedUpgrades = GetRandomUpgrades(3);
        DisplayUpgradeOptions(selectedUpgrades);
    }



    private List<UpgradeOption> GetRandomUpgrades(int count)
    {
        List<UpgradeOption> availableOptions = new List<UpgradeOption>(upgradeOptions);
        List<UpgradeOption> selectedUpgrades = new List<UpgradeOption>();
        HashSet<UpgradeType> selectedTypes = new HashSet<UpgradeType>(); // ��� ������������ ��������� ����� ���������

        while (selectedUpgrades.Count < count && availableOptions.Count > 0)
        {
            UpgradeOption selectedUpgrade = GetRandomUpgradeByRarity(availableOptions);

            // ���������, �� ��� �� ��� ������ ��� ����� ���������
            if (!selectedTypes.Contains(selectedUpgrade.upgradeType))
            {
                selectedUpgrades.Add(selectedUpgrade);
                selectedTypes.Add(selectedUpgrade.upgradeType); // ��������� ��� � ������ ��������� �����
                availableOptions.Remove(selectedUpgrade); // ������� ��������� �� ���������
            }
        }

        return selectedUpgrades;
    }


    // ����� ��� ��������� ���������� ��������� � ������ ��������
    private UpgradeOption GetRandomUpgradeByRarity(List<UpgradeOption> availableOptions)
    {
        int commonChance;
        int uncommonChance;
        int rareChance;

        if (waveNumber < 11)
        {
            // ����� ��� ���� �� 11-�
            commonChance = Mathf.Clamp(80 - (waveNumber - 1), 0, 100); // ������� ���� �������� ���������
            uncommonChance = Mathf.Clamp(19 + (waveNumber - 1), 0, 100 - commonChance); // ����������� ���� ���������� ���������
            rareChance = 100 - commonChance - uncommonChance; // ��������� ���� ������� ���������
        }
        else if (waveNumber <= 21)
        {
            // ����� ��� ���� � 11-� �� 21-�
            commonChance = Mathf.Clamp(68 - (waveNumber - 11) * 2, 0, 100); // ������� ����: 68% -2% �� �����
            uncommonChance = Mathf.Clamp(27 + (waveNumber - 11), 0, 100 - commonChance); // ��������� ����: 27% +1% �� �����
            rareChance = 100 - commonChance - uncommonChance; // ������ ����: 5% +1% �� �����
        }
        else
        {
            // ����� ��� ���� ����� 21-�
            if (48 - (waveNumber - 21) * 2 > 10)
            {
                commonChance = 48 - (waveNumber - 21) * 2; // ��������� common �� 2% �� �����
                uncommonChance = 37 + (waveNumber - 21); // ����������� uncommon �� 1% �� �����
                rareChance = 15 + (waveNumber - 21); // ����������� rare �� 1% �� �����
            }
            else
            {
                // ����� commonChance ������ 10%, ��������� ��� �� 10%
                commonChance = 10;

                // ���������� 90% �������������� �� ancommon � rare
                uncommonChance = 37 + 19; // uncommon �������� 37% + 19%
                rareChance = 15 + 19; // rare �������� 15% + 19%
            }
        }



        // ������� ����� �� ���� � �������
        Debug.Log($"������� �����: {waveNumber}");
        Debug.Log($"���� �� ������� ���������: {commonChance}%");
        Debug.Log($"���� �� ��������� ���������: {uncommonChance}%");
        Debug.Log($"���� �� ������ ���������: {rareChance}%");

        // ��������� ���������� ��������
        float randomValue = Random.Range(0f, 100f);

        UpgradeRarity selectedRarity;
        if (randomValue < commonChance) // ���� �� ������� ���������
        {
            selectedRarity = UpgradeRarity.Common;
        }
        else if (randomValue < commonChance + uncommonChance) // ���� �� ��������� ���������
        {
            selectedRarity = UpgradeRarity.Uncommon;
        }
        else // ���� �� ������ ���������
        {
            selectedRarity = UpgradeRarity.Rare;
        }

        // ������� ��������� ��������
        Debug.Log($"��������� ��������: {selectedRarity}");

        // ��������� ������ ��������� ����� �� ��������� ��������
        List<UpgradeOption> optionsOfSelectedRarity = availableOptions.FindAll(option => option.upgradeRarity == selectedRarity);

        // ���� �� ������� ����� ������ ��������, ���������� ��������� ���������
        if (optionsOfSelectedRarity.Count == 0)
        {
            return availableOptions[Random.Range(0, availableOptions.Count)];
        }

        // ���������� ��������� ����� �� ���������������� ������
        return optionsOfSelectedRarity[Random.Range(0, optionsOfSelectedRarity.Count)];
    }




    private void DisplayUpgradeOptions(List<UpgradeOption> upgrades)
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0;

        for (int i = 0; i < upgrades.Count; i++)
        {
            // ���������� ������ � ����� ��� ������� ���������
            upgradeIcons[i].sprite = upgrades[i].upgradeSprite;
            upgradeIcons[i].gameObject.SetActive(true);

            string description = GetUpgradeDescription(upgrades[i]);
            upgradeTexts[i].text = description;
            upgradeTexts[i].gameObject.SetActive(true);

            // ���������� ��������� �������� ��� ������� ���������
            int rarityIndex = (int)upgrades[i].upgradeRarity; // ����������� �������� � ������
            rarityImages[i].sprite = raritySprites[rarityIndex]; // ������������� ������ ������ �� ������ ��������
            rarityImages[i].gameObject.SetActive(true); // ���������� ������, ���� �� �����

            // ������������� �������� ��� ������� �� ������
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
                return "���� + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.10f, 0.15f, 0.20f) * 100) + "%";
            case UpgradeType.CritDamage:
                return "����������� ���� + " + (GetUpgradePercentage(upgrade.upgradeRarity, 15f, 20f, 25f)) + "%";
            case UpgradeType.AttackSpeed:
                return "�������� ����� + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f) * 100) + "%";
            case UpgradeType.CritChance:
                return "���� ������������ ����� + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.05f, 0.10f, 0.15f) * 100) + "%";
            case UpgradeType.AttackRange:
                return "��������� ����� + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f) * 100) + "%";
            case UpgradeType.MaxHealth:
                return "�������� + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.30f, 0.40f, 0.50f) * 100) + "%";
            case UpgradeType.Armor:
                return "����� + " + GetUpgradeValue(upgrade.upgradeRarity, 10, 15, 20);
            case UpgradeType.HealthRegen:
                return "����������� �������� + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f) * 100) + "%";
            case UpgradeType.Lifesteal:
                return "��������� + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f) * 100) + "%";
            case UpgradeType.Investment:
                return "���������� + " + GetUpgradeValue(upgrade.upgradeRarity, 30, 75, 120);
            case UpgradeType.PickupRadius:
                return "������ ����� + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.30f, 0.40f, 0.50f) * 100) + "%";
            case UpgradeType.MoveSpeed:
                return "�������� ���� + " + (GetUpgradePercentage(upgrade.upgradeRarity, 0.10f, 0.15f, 0.20f) * 100) + "%";
            case UpgradeType.Luck:
                return "����� + " + GetUpgradeValue(upgrade.upgradeRarity, 10, 20, 30);
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

    // ��������������� ������ ��� ��������� �������� �� ��������
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

    // ���������� ���������
    public void ApplyUpgrade(UpgradeOption upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case UpgradeType.Damage:
                float damageIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.10f, 0.15f, 0.20f);
                foreach (var weapon in playerWeapons) // ��������� � ������� ������
                {
                    weapon.IncreaseDamage(damageIncrease);
                }
                Debug.Log("���� �������� ��: " + damageIncrease * 100 + "%");
                break;

            case UpgradeType.CritDamage:
                float critDamageIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 15f, 20f, 25f);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseCritDamage(critDamageIncrease);
                }
                Debug.Log("����������� ���� �������� ��: " + critDamageIncrease * 100 + "%");
                break;

            case UpgradeType.AttackSpeed:
                float attackSpeedIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackSpeed(attackSpeedIncrease);
                }
                Debug.Log("�������� ����� ��������� ��: " + attackSpeedIncrease * 100 + "%");
                break;

            case UpgradeType.CritChance:
                // ���� �����: 5% / 10% / 15%
                float critChanceIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.05f, 0.10f, 0.15f);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseCritChance(critChanceIncrease);
                }
                Debug.Log("���� ������������ ����� �������� ��: " + critChanceIncrease * 100 + "%");
                break;

            case UpgradeType.AttackRange:
                // ��������� �����: 15% / 20% / 25%
                float attackRangeIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f);
                foreach (var weapon in playerWeapons)
                {
                    weapon.IncreaseAttackRange(attackRangeIncrease);
                }
                Debug.Log("��������� ����� ��������� ��: " + attackRangeIncrease * 100 + "%");
                break;

            case UpgradeType.MaxHealth:
                // ��������: 30% / 40% / 50%
                float maxHealthIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.30f, 0.40f, 0.50f);
                playerHealth.IncreaseMaxHealth(maxHealthIncrease);
                Debug.Log("�������� ��������� ��: " + maxHealthIncrease * 100 + "%");
                break;

            case UpgradeType.Armor:
                // �����: 10 / 15 / 20
                int armorIncrease = GetUpgradeValue(upgrade.upgradeRarity, 10, 15, 20);
                playerHealth.IncreaseArmor(armorIncrease);
                Debug.Log("����� ��������� ��: " + armorIncrease);
                break;

            case UpgradeType.HealthRegen:
                // ����������� ��������: 15% / 20% / 25%
                float regenIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f);
                playerHealth.IncreaseHealthRegen(regenIncrease);
                Debug.Log("����������� �������� ��������� ��: " + regenIncrease * 100 + "%");
                break;

            case UpgradeType.Lifesteal:
                // ���������: 15% / 20% / 25%
                float lifestealIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.15f, 0.20f, 0.25f);
                playerHealth.IncreaseLifesteal(lifestealIncrease);
                Debug.Log("��������� �������� ��: " + lifestealIncrease * 100 + "%");
                break;

            case UpgradeType.Investment:
                // ����������: 30 / 75 / 120
                int investmentIncrease = GetUpgradeValue(upgrade.upgradeRarity, 30, 75, 120);
                playerHealth.IncreaseInvestment(investmentIncrease);
                Debug.Log("���������� ��������� ��: " + investmentIncrease);
                break;

            case UpgradeType.PickupRadius:
                // ������ �����: 30% / 40% / 50%
                float pickupRadiusIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.30f, 0.40f, 0.50f);
                playerHealth.IncreasePickupRadius(pickupRadiusIncrease);
                Debug.Log("������ ����� �������� ��: " + pickupRadiusIncrease * 100 + "%");
                break;

            case UpgradeType.MoveSpeed:
                // �������� ����: 10% / 15% / 20%
                float moveSpeedIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 0.10f, 0.15f, 0.20f);
                playerMovement.IncreaseMoveSpeed(moveSpeedIncrease);
                Debug.Log("�������� ���� ��������� ��: " + moveSpeedIncrease * 100 + "%");
                break;

            case UpgradeType.Luck:
                // �����: 10 / 20 / 30
                float luckIncrease = GetUpgradePercentage(upgrade.upgradeRarity, 10, 20, 30);
                playerHealth.IncreaseLuck((int)luckIncrease);
                Debug.Log("����� ��������� ��: " + luckIncrease);
                break;
        }
    }
}