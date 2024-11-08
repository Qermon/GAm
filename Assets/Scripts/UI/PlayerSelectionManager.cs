using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerSelectionManager : MonoBehaviour
{
    public GameObject playerPrefab1;
    public GameObject playerPrefab2;
    public GameObject playerPrefab3;
    public Transform spawnPoint;
    public GameObject playerSelectionPanel;
    public GameObject statsWindow;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI heroNameText;
    public Button confirmButton;

    public Button button1;
    public Button button2;
    public Button button3;

    public Color activeColor = new Color(1f, 1f, 1f, 1f);
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.5f);

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

    // ����������� ��������� ��� ���� � ������� ������
    public AudioSource menuMusicSource;
    public AudioSource gameMusicSource;
    public float transitionDuration = 2f; // ������������ ��������

    public Canvas fadeCanvas;        // ��������� Canvas � ������ �������
    public CanvasGroup fadeOverlay;
    public float fadeDuration = 1f;

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

        playerPrefabs = new GameObject[] { playerPrefab1, playerPrefab2, playerPrefab3 };
        buttons = new Button[] { button1, button2, button3 };

        LoadPlayerSelection();
        DisplayPlayerStats(selectedPlayerPrefab);

        confirmButton.interactable = true;

        button1.onClick.AddListener(() => OnPlayerButtonClick(playerPrefab1, 1));
        button2.onClick.AddListener(() => OnPlayerButtonClick(playerPrefab2, 2));
        button3.onClick.AddListener(() => OnPlayerButtonClick(playerPrefab3, 3));

        fadeOverlay.alpha = 0f;           // ��������� ������������
        fadeOverlay.interactable = false; // ��������� �������������� �� ���������
        fadeCanvas.sortingOrder = 0;      // ������������� ������ ������� ����������� �� ���������

    }

    public void OnPlayerButtonClick(GameObject playerPrefab, int playerId)
    {
        selectedPlayerPrefab = playerPrefab;
        DisplayPlayerStats(selectedPlayerPrefab);

        PlayerPrefs.SetInt("SelectedPlayer", playerId);
        PlayerPrefs.Save();

        UpdateButtonColors();
    }

    void UpdateButtonColors()
    {
        for (int i = 0; i < playerPrefabs.Length; i++)
        {
            buttons[i].GetComponent<Image>().color = (playerPrefabs[i] == selectedPlayerPrefab) ? activeColor : inactiveColor;
        }
    }

    public void OnConfirmSelection()
    {
        if (selectedPlayerPrefab != null)
        {
            StartCoroutine(FadeOutThenLoadPlayer());
        }
    }


    private IEnumerator FadeOutThenLoadPlayer()
    {
        // ��������� ���� ��� ��������� � �������� ���������������
        fadeCanvas.sortingOrder = 10;
        fadeOverlay.interactable = true;

        yield return StartCoroutine(FadeIn());
        SpawnPlayer(selectedPlayerPrefab);
        playerSelectionPanel.SetActive(false);
        yield return StartCoroutine(SwitchToGameMusic());
        yield return StartCoroutine(FadeOut());

        // ������� ����������: ���������� ������� ����� � ��������� ���������������
        fadeCanvas.sortingOrder = 0;
        fadeOverlay.interactable = false;
    }

    private IEnumerator SwitchToGameMusic()
    {
        // ��������� ����������� ��������� �� ��������
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.2f);
        float elapsedTime = 0f;

        // ��������, ��� ������� �������� ������ � ������� ���������
        gameMusicSource.volume = 0f;
        gameMusicSource.Play();

        // ���������� ���������� ��������� ���� � ������������ ��������� ����
        while (elapsedTime < transitionDuration)
        {
            menuMusicSource.volume = Mathf.Lerp(savedVolume, 0f, elapsedTime / transitionDuration);
            gameMusicSource.volume = Mathf.Lerp(0f, savedVolume, elapsedTime / transitionDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���������, ��� ��������� ����������� ��������� ����� ����������
        menuMusicSource.volume = 0f;
        gameMusicSource.volume = savedVolume;
        menuMusicSource.Stop();
    }

    private void DisplayPlayerStats(GameObject playerPrefab)
    {
        heroNameText.text = playerPrefab.name;

        PlayerHealth playerHealth = playerPrefab.GetComponent<PlayerHealth>();
        Weapon[] playerWeapons = playerPrefab.GetComponentsInChildren<Weapon>();
        PlayerMovement playerMovement = playerPrefab.GetComponent<PlayerMovement>();

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

        string statsInfo = $"��������: {(int)playerHealth.baseMaxHealth}\n";
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

        statsText.text = statsInfo;

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
            experienceBarImage.RestartSkript();
            weaponSelectionManager.RestartScript();
            waveManager.RestartScript();
            shop.RestartScript();
            levelUpMenu.RestartScript();
        
    }

    void LoadPlayerSelection()
    {
        int selectedPlayerId = PlayerPrefs.GetInt("SelectedPlayer", 1);
        selectedPlayerPrefab = playerPrefabs[selectedPlayerId - 1];

        UpdateButtonColors();
    }

    public void Exit()
    {
        mainMenu.mainMenu.SetActive(true);
        playerSelectionPanel.SetActive(false);
    }


    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            fadeOverlay.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeOverlay.alpha = 1f;
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            fadeOverlay.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeOverlay.alpha = 0f;
    }





}
