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

    public UpgradeOption[] upgradeOptions; // ������ ��������� ������

    // ������ ��� �������� ������ ������
    [SerializeField] private Image[] emptyIcons;

    

    public enum UpgradeType
    {
        ShieldPerWave,    // ������ �� �����
        ShieldOnKill,
        BarrierOnLowHealth,
        HealthRegenPerWave
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
