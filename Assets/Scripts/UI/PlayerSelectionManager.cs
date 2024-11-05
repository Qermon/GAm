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
    public GameObject statsWindow; // ���� ��� ����������� �������������
    public TextMeshProUGUI statsText; // ���� ��������� ������� ��� ���� �������������
    public TextMeshProUGUI heroNameText; // ����� ��� ����������� ����� �����
    public Button confirmButton; // ������ ������������� ������

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

        // �� ��������� ������ ������ �����
        selectedPlayerPrefab = playerPrefab1;
        DisplayPlayerStats(selectedPlayerPrefab);
        confirmButton.interactable = false; // ��������� ������ �������������, ���� �� ������� ������� ������
    }

    public void OnPlayerButtonClick(GameObject playerPrefab)
    {
        selectedPlayerPrefab = playerPrefab;
        DisplayPlayerStats(selectedPlayerPrefab);
        confirmButton.interactable = true; // ������������ ������ �������������
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
        // ������������� ��� �����
        heroNameText.text = playerPrefab.name; // ���������, ��� � ������ ������� ����������� ���

        // �������� ���������� �������� � ������
        PlayerHealth playerHealth = playerPrefab.GetComponent<PlayerHealth>();
        Weapon[] playerWeapons = playerPrefab.GetComponentsInChildren<Weapon>();
        PlayerMovement playerMovement = playerPrefab.GetComponent<PlayerMovement>();

        // �������������� ���������� ��� ������� ������� ��������
        float totalDamage = 0f;
        float totalCritDamage = 0f;
        float totalCritChance = 0f;
        float totalAttackSpeed = 0f;
        float totalAttackRange = 0f;
        int weaponCount = playerWeapons.Length;

        // ��������� �������������� ���� ������
        foreach (Weapon weapon in playerWeapons)
        {
            totalDamage += weapon.damage;
            totalCritDamage += weapon.criticalDamage;
            totalCritChance += weapon.criticalChance;
            totalAttackSpeed += weapon.attackSpeed;
            totalAttackRange += weapon.attackRange;
        }

        // ��������� ������� ��������
        float avgDamage = weaponCount > 0 ? totalDamage / weaponCount : 0f;
        float avgCritDamage = weaponCount > 0 ? totalCritDamage / weaponCount : 0f;
        float avgCritChance = weaponCount > 0 ? totalCritChance / weaponCount : 0f;
        float avgAttackSpeed = weaponCount > 0 ? totalAttackSpeed / weaponCount : 0f;
        float avgAttackRange = weaponCount > 0 ? totalAttackRange / weaponCount : 0f;

        // ��������� ����� ��� ����������� ���� �������������
        string statsInfo = "";

        statsInfo += $"��������: {(int)playerHealth.baseMaxHealth}\n";
        statsInfo += $"����: {(int)avgDamage}\n";
        statsInfo += $"����. ����: {(int)avgCritDamage}\n";
        statsInfo += $"����. ����: {(int)(avgCritChance * 100)}%\n";
        statsInfo += $"�������� �����: {(int)(avgAttackSpeed * 300)}\n";
        statsInfo += $"��������� �����: {(int)(avgAttackRange * 100)}\n";
        statsInfo += $"�����������: {(int)(playerHealth.baseRegen * 10000)}\n";
        statsInfo += $"���������: {(int)(playerHealth.baseLifesteal * 10000)}\n";
        statsInfo += $"������: {(int)playerHealth.defense}\n";
        statsInfo += $"�������� ����: {(int)(playerMovement.moveSpeed * 200)}\n";
        statsInfo += $"������ �����: {(int)(playerHealth.basePickupRadius * 20)}\n";
        statsInfo += $"����������: {(int)playerHealth.investment}\n";
        statsInfo += $"�����: {(int)playerHealth.luck}\n";

        // ��������� ����� � ���� �������������
        statsText.text = statsInfo;

        // ���������� ���� �������������
        statsWindow.SetActive(true);
    }

    private void SpawnPlayer(GameObject playerPrefab)
    {
        GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("����� ������: " + playerPrefab.name);

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
