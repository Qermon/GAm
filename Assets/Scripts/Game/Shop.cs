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
    private List<Upgrade> upgrades = new List<Upgrade>(); // Список доступных баффов

    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private PlayerGold playerGold;

    public UpgradeOption[] upgradeOptions; // Массив доступных баффов

    // Массив для хранения пустых иконок
    [SerializeField] private Image[] emptyIcons;

    

    public enum UpgradeType
    {
        ShieldPerWave,    // Барьер за волну
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
        upgrades.Clear();
        for (int i = 0; i < buffButtons.Length; i++)
        {
            upgrades.Add(GenerateRandomUpgrade());
        }
    }

    private Upgrade GenerateRandomUpgrade()
    {
        // Генерируем случайный индекс для выбора UpgradeOption
        int randomIndex = Random.Range(0, upgradeOptions.Length);
        UpgradeOption selectedOption = upgradeOptions[randomIndex];

        // Генерация стоимости апгрейда
        int baseCost = 10;
        int rarityMultiplier = 1; // Замените это на вашу логику редкости
        int randomCost = baseCost * rarityMultiplier + Random.Range(5, 15);

        // Создаем новый Upgrade, используя выбранный UpgradeOption
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
