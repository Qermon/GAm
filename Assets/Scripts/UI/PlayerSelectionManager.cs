using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{
    public GameObject playerPrefab1;
    public GameObject playerPrefab2;
    public GameObject playerPrefab3;
    public Transform spawnPoint;
    public GameObject playerSelectionPanel;
    public GameObject statsWindow; // Окно для отображения характеристик
    public TextMeshProUGUI statsText; // Один текстовый элемент для всех характеристик
    public TextMeshProUGUI heroNameText; // Текст для отображения имени героя
    public Button confirmButton; // Кнопка подтверждения выбора

    private GameObject selectedPlayerPrefab;
    private ExperienceBarImage experienceBarImage;
    private PlayerLevelUp playerLevelUp;
    private Shop shop;
    private LevelUpMenu levelUpMenu;
    private PlayerGold playerGold;
    private WaveManager waveManager;
    private WeaponSelectionManager weaponSelectionManager;

    void Start()
    {
        weaponSelectionManager = FindObjectOfType<WeaponSelectionManager>();
        playerGold = FindObjectOfType<PlayerGold>();
        levelUpMenu = FindAnyObjectByType<LevelUpMenu>();
        shop = FindAnyObjectByType<Shop>();
        playerLevelUp = FindObjectOfType<PlayerLevelUp>();
        experienceBarImage = FindObjectOfType<ExperienceBarImage>();
        waveManager = FindObjectOfType<WaveManager>();

        // По умолчанию выбран первый игрок
        selectedPlayerPrefab = playerPrefab1;
        DisplayPlayerStats(selectedPlayerPrefab);
        confirmButton.interactable = false; // Блокируем кнопку подтверждения, пока не выберем другого игрока
    }

    public void OnPlayerButtonClick(GameObject playerPrefab)
    {
        selectedPlayerPrefab = playerPrefab;
        DisplayPlayerStats(selectedPlayerPrefab);
        confirmButton.interactable = true; // Разблокируем кнопку подтверждения
    }

    public void OnConfirmSelection()
    {
        if (selectedPlayerPrefab != null)
        {
            SpawnPlayer(selectedPlayerPrefab);
            playerSelectionPanel.SetActive(false);
        }
    }

    private void DisplayPlayerStats(GameObject playerPrefab)
    {
        // Устанавливаем имя героя
        heroNameText.text = playerPrefab.name; // Убедитесь, что у вашего префаба установлено имя

        // Получаем компоненты здоровья и оружия
        PlayerHealth playerHealth = playerPrefab.GetComponent<PlayerHealth>();
        Weapon[] playerWeapons = playerPrefab.GetComponentsInChildren<Weapon>();
        PlayerMovement playerMovement = playerPrefab.GetComponent<PlayerMovement>();

        // Инициализируем переменные для расчета средних значений
        float totalDamage = 0f;
        float totalCritDamage = 0f;
        float totalCritChance = 0f;
        float totalAttackSpeed = 0f;
        float totalAttackRange = 0f;
        int weaponCount = playerWeapons.Length;

        // Суммируем характеристики всех оружий
        foreach (Weapon weapon in playerWeapons)
        {
            totalDamage += weapon.damage;
            totalCritDamage += weapon.criticalDamage;
            totalCritChance += weapon.criticalChance;
            totalAttackSpeed += weapon.attackSpeed;
            totalAttackRange += weapon.attackRange;
        }

        // Вычисляем средние значения
        float avgDamage = weaponCount > 0 ? totalDamage / weaponCount : 0f;
        float avgCritDamage = weaponCount > 0 ? totalCritDamage / weaponCount : 0f;
        float avgCritChance = weaponCount > 0 ? totalCritChance / weaponCount : 0f;
        float avgAttackSpeed = weaponCount > 0 ? totalAttackSpeed / weaponCount : 0f;
        float avgAttackRange = weaponCount > 0 ? totalAttackRange / weaponCount : 0f;

        // Формируем текст для отображения всех характеристик
        string statsInfo = "";

        statsInfo += $"Здоровье: {(int)playerHealth.baseMaxHealth}\n";
        statsInfo += $"Урон: {(int)avgDamage}\n";
        statsInfo += $"Крит. урон: {(int)avgCritDamage}\n";
        statsInfo += $"Крит. шанс: {(int)(avgCritChance * 100)}%\n";
        statsInfo += $"Скорость атаки: {(int)(avgAttackSpeed * 300)}\n";
        statsInfo += $"Дальность атаки: {(int)(avgAttackRange * 100)}\n";
        statsInfo += $"Регенерация: {(int)(playerHealth.baseRegen * 10000)}\n";
        statsInfo += $"Вампиризм: {(int)(playerHealth.baseLifesteal * 10000)}\n";
        statsInfo += $"Защита: {(int)playerHealth.defense}\n";
        statsInfo += $"Скорость бега: {(int)(playerMovement.moveSpeed * 200)}\n";
        statsInfo += $"Радиус сбора: {(int)(playerHealth.basePickupRadius * 20)}\n";
        statsInfo += $"Инвестиции: {(int)playerHealth.investment}\n";
        statsInfo += $"Удача: {(int)playerHealth.luck}\n";

        // Обновляем текст в окне характеристик
        statsText.text = statsInfo;

        // Показываем окно характеристик
        statsWindow.SetActive(true);
    }

    private void SpawnPlayer(GameObject playerPrefab)
    {
        GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("Спавн игрока: " + playerPrefab.name);

        StartGame();
    }

    private void StartGame()
    {
        if (!weaponSelectionManager.enabled && !waveManager.enabled && !shop.enabled && !levelUpMenu.enabled)
        {
            experienceBarImage.enabled = true;
            weaponSelectionManager.enabled = true;
            waveManager.enabled = true;
            shop.enabled = true;
            levelUpMenu.enabled = true;
        }
        else
        {
            experienceBarImage.RestartSkript();
            weaponSelectionManager.RestartScript();
            waveManager.RestartScript();
            shop.RestartScript();
            levelUpMenu.RestartScript();
        }
    }
}
