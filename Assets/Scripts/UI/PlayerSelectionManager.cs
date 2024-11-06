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

    // Кнопки для выбора персонажа
    public Button button1;
    public Button button2;
    public Button button3;

    public Color activeColor = new Color(1f, 1f, 1f, 1f); // Полная непрозрачность
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.5f); // Полупрозрачность

    private GameObject selectedPlayerPrefab;
    private GameObject[] playerPrefabs;
    private Button[] buttons;

    private ExperienceBarImage experienceBarImage;
    private PlayerLevelUp playerLevelUp;
    private Shop shop;
    private LevelUpMenu levelUpMenu;
    private PlayerGold playerGold;
    private WaveManager waveManager;
    private WeaponSelectionManager weaponSelectionManager;
    private MainMenu mainMenu;

    void Start()
    {
        mainMenu = FindObjectOfType<MainMenu>();
        weaponSelectionManager = FindObjectOfType<WeaponSelectionManager>();
        playerGold = FindObjectOfType<PlayerGold>();
        levelUpMenu = FindAnyObjectByType<LevelUpMenu>();
        shop = FindAnyObjectByType<Shop>();
        playerLevelUp = FindObjectOfType<PlayerLevelUp>();
        experienceBarImage = FindObjectOfType<ExperienceBarImage>();
        waveManager = FindObjectOfType<WaveManager>();

        // Список префабов персонажей
        playerPrefabs = new GameObject[] { playerPrefab1, playerPrefab2, playerPrefab3 };
        buttons = new Button[] { button1, button2, button3 };

        // Загружаем выбранного персонажа при старте или выбираем первого по умолчанию
        LoadPlayerSelection();

        // По умолчанию показываем характеристики выбранного персонажа
        DisplayPlayerStats(selectedPlayerPrefab);

        // Кнопка подтверждения всегда активна
        confirmButton.interactable = true;

        // Устанавливаем обработчики на кнопки
        button1.onClick.AddListener(() => OnPlayerButtonClick(playerPrefab1, 1));
        button2.onClick.AddListener(() => OnPlayerButtonClick(playerPrefab2, 2));
        button3.onClick.AddListener(() => OnPlayerButtonClick(playerPrefab3, 3));
    }

    // Выбор персонажа
    public void OnPlayerButtonClick(GameObject playerPrefab, int playerId)
    {
        selectedPlayerPrefab = playerPrefab;
        DisplayPlayerStats(selectedPlayerPrefab);

        // Сохраняем выбор персонажа
        PlayerPrefs.SetInt("SelectedPlayer", playerId);
        PlayerPrefs.Save(); // Сохраняем изменения

        // Обновляем цвет кнопок
        UpdateButtonColors();
    }

    // Подсветка кнопок в зависимости от выбора персонажа
    void UpdateButtonColors()
    {
        for (int i = 0; i < playerPrefabs.Length; i++)
        {
            buttons[i].GetComponent<Image>().color = (playerPrefabs[i] == selectedPlayerPrefab) ? activeColor : inactiveColor;
        }
    }

    // Подтверждение выбора персонажа
    public void OnConfirmSelection()
    {
        if (selectedPlayerPrefab != null)
        {
            SpawnPlayer(selectedPlayerPrefab);
            playerSelectionPanel.SetActive(false);
        }
    }

    // Отображение характеристик выбранного персонажа
    private void DisplayPlayerStats(GameObject playerPrefab)
    {
        heroNameText.text = playerPrefab.name; // Убедитесь, что у вашего префаба установлено имя

        PlayerHealth playerHealth = playerPrefab.GetComponent<PlayerHealth>();
        Weapon[] playerWeapons = playerPrefab.GetComponentsInChildren<Weapon>();
        PlayerMovement playerMovement = playerPrefab.GetComponent<PlayerMovement>();

        // Суммируем характеристики всех оружий
        float totalDamage = 0f, totalCritDamage = 0f, totalCritChance = 0f, totalAttackSpeed = 0f, totalAttackRange = 0f;
        int weaponCount = playerWeapons.Length;

        foreach (Weapon weapon in playerWeapons)
        {
            totalDamage += weapon.damage;
            totalCritDamage += weapon.criticalDamage;
            totalCritChance += weapon.criticalChance;
            totalAttackSpeed += weapon.attackSpeed;
            totalAttackRange += weapon.attackRange;
        }

        float avgDamage = weaponCount > 0 ? totalDamage / weaponCount : 0f;
        float avgCritDamage = weaponCount > 0 ? totalCritDamage / weaponCount : 0f;
        float avgCritChance = weaponCount > 0 ? totalCritChance / weaponCount : 0f;
        float avgAttackSpeed = weaponCount > 0 ? totalAttackSpeed / weaponCount : 0f;
        float avgAttackRange = weaponCount > 0 ? totalAttackRange / weaponCount : 0f;

        string statsInfo = $"Здоровье: {(int)playerHealth.baseMaxHealth}\n";
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

        statsText.text = statsInfo;

        statsWindow.SetActive(true);
    }

    // Спавн выбранного персонажа
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

    // Загрузка сохраненного выбора персонажа
    void LoadPlayerSelection()
    {
        int selectedPlayerId = PlayerPrefs.GetInt("SelectedPlayer", 1); // По умолчанию выбираем первого персонажа
        selectedPlayerPrefab = playerPrefabs[selectedPlayerId - 1]; // Индексы PlayerPrefs начинаются с 1

        // Обновляем цвета кнопок
        UpdateButtonColors();
    }

    public void Exit()
    {
        mainMenu.mainMenu.SetActive(true);
        playerSelectionPanel.SetActive(false);
    }
}