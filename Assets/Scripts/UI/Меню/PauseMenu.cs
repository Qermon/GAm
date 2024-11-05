using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu; // Панель меню паузы
    private Shop shop;
    private LevelUpMenu levelUpMenu;
    private WaveManager waveManager;
    private WeaponSelectionManager weaponSelectionManager;
    private MainMenu mainMenu;
    private CursorManager cursorManager;
    public CanvasGroup pauseMenuPanel; // CanvasGroup для панели меню
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
        // Проверяем, нажата ли клавиша ESC и открыты ли панели
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC нажата, проверяем панели");
            if (!IsAnyPanelOpen())
            {
                Debug.Log("Панели не открыты, переключаем паузу");
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
                Debug.Log("Открыта одна или несколько панелей, пауза не активируется");
            }
        }

    }
    public void OnPlayButtonPressed()
    {
        ResumeGame();
    }

    // Метод для кнопки "Настройки" (пока пустой)
    public void OnSettingsButtonPressed()
    {
        Debug.Log("Настройки будут добавлены позже.");
    }

    // Метод для кнопки "Выход"
    public void OnExitButtonPressed()
    {
        gameManager.RestartGame();
    }

    public void PauseGame()
    {
        cursorManager.ShowCursor();
        isPaused = true;
        Time.timeScale = 0f; // Останавливаем время
        pauseMenu.SetActive(true); // Показываем панель меню паузы
    }

    public void ResumeGame()
    {
        cursorManager.HideCursor();
        isPaused = false;
        Time.timeScale = 1f; // Возобновляем время
        pauseMenu.SetActive(false); // Скрываем панель меню паузы
    }
    private bool IsAnyPanelOpen()
    {
        // Проверяем состояние каждой панели
        bool isShopOpen = shop.shopPanel.activeSelf;
        bool isMainMenuOpen = mainMenu.canvasHero.activeSelf;
        bool isLevelUpMenuOpen = levelUpMenu.levelUpPanel.activeSelf;
        bool isPlayerSelectionOpen = playerSelectionManager.playerSelectionPanel.activeSelf;
        bool isWeaponSelectionOpen = weaponSelectionManager.weaponSelectionPanel.activeSelf;

        // Выводим в консоль состояние каждой панели для отладки
        Debug.Log($"Shop Open: {isShopOpen}");
        Debug.Log($"Main Menu Open: {isMainMenuOpen}");
        Debug.Log($"Level Up Menu Open: {isLevelUpMenuOpen}");
        Debug.Log($"Player Selection Open: {isPlayerSelectionOpen}");
        Debug.Log($"Weapon Selection Open: {isWeaponSelectionOpen}");

        // Если хотя бы одна панель открыта, возвращаем true
        return isShopOpen || isMainMenuOpen || isLevelUpMenuOpen || isPlayerSelectionOpen || isWeaponSelectionOpen;
    }
}
