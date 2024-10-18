using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public enum UpgradeType
{
    Speed,
    MaxHealth,
    HealthRegen,
    WeaponBuff,
    Defense,
    Weapon,
    Lifesteal,
    CriticalAttack,   // Новая способность: критический урон
    CriticalChance    // Новая способность: шанс критической атаки
}

[System.Serializable]
public class UpgradeOption
{
    public string upgradeName;
    public Sprite upgradeSprite;
    public UpgradeType upgradeType;
    public Weapon[] weaponsToBuff;
    public GameObject weaponPrefab;
}

public class LevelUpMenu : MonoBehaviour
{
    public GameObject levelUpPanel;
    public Image[] upgradeIcons;
    public Button[] upgradeButtons;
    public List<UpgradeOption> upgradeOptions;

    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private Weapon playerWeapon;

    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerWeapon = FindObjectOfType<Weapon>();

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
            int randomIndex = Random.Range(0, availableOptions.Count);
            selectedUpgrades.Add(availableOptions[randomIndex]);
            availableOptions.RemoveAt(randomIndex);
        }

        return selectedUpgrades;
    }

    private void DisplayUpgradeOptions(List<UpgradeOption> upgrades)
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0;

        for (int i = 0; i < upgrades.Count; i++)
        {
            upgradeIcons[i].sprite = upgrades[i].upgradeSprite;
            upgradeIcons[i].gameObject.SetActive(true);

            int index = i;
            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() => ChooseUpgrade(upgrades[index]));
        }
    }

    public void ChooseUpgrade(UpgradeOption upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case UpgradeType.Speed:
                playerMovement.moveSpeed += 2;
                Debug.Log("Скорость увеличена до: " + playerMovement.moveSpeed);
                break;

            case UpgradeType.MaxHealth:
                playerHealth.maxHealth += 20;
                playerHealth.UpdateHealthUI();
                Debug.Log("Максимальное здоровье увеличено до: " + playerHealth.maxHealth);
                break;

            case UpgradeType.HealthRegen:
                playerHealth.StartHealthRegen();
                Debug.Log("Пассивная регенерация активирована.");
                break;

            case UpgradeType.WeaponBuff:
                foreach (Weapon weapon in upgrade.weaponsToBuff)
                {
                    if (weapon != null)
                    {
                        weapon.damage += 10;
                        Debug.Log(weapon.name + " урон увеличен до: " + weapon.damage);
                    }
                }
                break;

            case UpgradeType.Defense:
                playerHealth.IncreaseDefense(10);
                Debug.Log("Защита игрока увеличена до: " + playerHealth.defense);
                break;

            case UpgradeType.Weapon:
                if (upgrade.weaponPrefab != null)
                {
                    GameObject weapon = Instantiate(upgrade.weaponPrefab, transform.position, Quaternion.identity);
                    weapon.SetActive(true);
                    Debug.Log(upgrade.weaponPrefab.name + " получено.");
                }
                break;

            case UpgradeType.Lifesteal:
                playerHealth.AddLifesteal(10);
                break;

            case UpgradeType.CriticalAttack:
                playerWeapon.criticalDamage += 10;
                Debug.Log("Критический урон увеличен до: " + playerWeapon.criticalDamage);
                break;

            case UpgradeType.CriticalChance:
                playerWeapon.criticalChance += 0.05f;
                Debug.Log("Шанс критического удара увеличен до: " + playerWeapon.criticalChance * 100 + "%");
                break;
        }

        CloseLevelUpMenu();
    }

    public void CloseLevelUpMenu()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1;
    }
}
