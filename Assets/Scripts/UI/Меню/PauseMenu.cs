using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu; // ������ ���� �����
    private Shop shop;
    private LevelUpMenu levelUpMenu;
    private WaveManager waveManager;
    private WeaponSelectionManager weaponSelectionManager;
    private MainMenu mainMenu;
    private CursorManager cursorManager;
    public CanvasGroup pauseMenuPanel; // CanvasGroup ��� ������ ����
    private PlayerSelectionManager playerSelectionManager;
    private GameManager gameManager;
    private bool isPaused = false;


    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerSelectionManager = FindObjectOfType<PlayerSelectionManager>();
        cursorManager = FindObjectOfType<CursorManager>();
        mainMenu = FindObjectOfType<MainMenu>();
        weaponSelectionManager = FindObjectOfType<WeaponSelectionManager>();
        levelUpMenu = FindAnyObjectByType<LevelUpMenu>();
        shop = FindAnyObjectByType<Shop>();
        waveManager = FindObjectOfType<WaveManager>();

    }

    void Update()
    {
        // ���������, ������ �� ������� ESC � ������� �� ������
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC ������, ��������� ������");
            if (!IsAnyPanelOpen())
            {
                Debug.Log("������ �� �������, ����������� �����");
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
            else
            {
                Debug.Log("������� ���� ��� ��������� �������, ����� �� ������������");
            }
        }

    }
    public void OnPlayButtonPressed()
    {
        ResumeGame();
    }

    // ����� ��� ������ "���������" (���� ������)
    public void OnSettingsButtonPressed()
    {
        Debug.Log("��������� ����� ��������� �����.");
    }

    // ����� ��� ������ "�����"
    public void OnExitButtonPressed()
    {
        gameManager.RestartGame();
    }

    public void PauseGame()
    {
        cursorManager.ShowCursor();
        isPaused = true;
        Time.timeScale = 0f; // ������������� �����
        pauseMenu.SetActive(true); // ���������� ������ ���� �����
    }

    public void ResumeGame()
    {
        cursorManager.HideCursor();
        isPaused = false;
        Time.timeScale = 1f; // ������������ �����
        pauseMenu.SetActive(false); // �������� ������ ���� �����
    }
    private bool IsAnyPanelOpen()
    {
        // ��������� ��������� ������ ������
        bool isShopOpen = shop.shopPanel.activeSelf;
        bool isMainMenuOpen = mainMenu.canvasHero.activeSelf;
        bool isLevelUpMenuOpen = levelUpMenu.levelUpPanel.activeSelf;
        bool isPlayerSelectionOpen = playerSelectionManager.playerSelectionPanel.activeSelf;
        bool isWeaponSelectionOpen = weaponSelectionManager.weaponSelectionPanel.activeSelf;

        // ������� � ������� ��������� ������ ������ ��� �������
        Debug.Log($"Shop Open: {isShopOpen}");
        Debug.Log($"Main Menu Open: {isMainMenuOpen}");
        Debug.Log($"Level Up Menu Open: {isLevelUpMenuOpen}");
        Debug.Log($"Player Selection Open: {isPlayerSelectionOpen}");
        Debug.Log($"Weapon Selection Open: {isWeaponSelectionOpen}");

        // ���� ���� �� ���� ������ �������, ���������� true
        return isShopOpen || isMainMenuOpen || isLevelUpMenuOpen || isPlayerSelectionOpen || isWeaponSelectionOpen;
    }
}
