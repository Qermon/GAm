using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    private Shop shop;
    private LevelUpMenu levelUpMenu;
    private WaveManager waveManager;
    private WeaponSelectionManager weaponSelectionManager;
    private MainMenu mainMenu;
    private CursorManager cursorManager;
    public CanvasGroup pauseMenuPanel; // CanvasGroup для панели меню
    private bool isPaused = false;

    private void Start()
    {
        cursorManager = FindObjectOfType<CursorManager>();
        mainMenu = FindObjectOfType<MainMenu>();
        weaponSelectionManager = FindObjectOfType<WeaponSelectionManager>();
        levelUpMenu = FindAnyObjectByType<LevelUpMenu>();
        shop = FindAnyObjectByType<Shop>();
        waveManager = FindObjectOfType<WaveManager>();

        // Прячем панель паузы в начале игры
        pauseMenuPanel.alpha = 0;
        pauseMenuPanel.interactable = false;
        pauseMenuPanel.blocksRaycasts = false;
    }

    private void Update()
    {
        // Обработка нажатия клавиши ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        // Показать панель паузы
        pauseMenuPanel.alpha = 1;
        pauseMenuPanel.interactable = true;
        pauseMenuPanel.blocksRaycasts = true;
        pausePanel.SetActive(true);
        cursorManager.ShowCursor();

        Time.timeScale = 0f; // Остановить время
        isPaused = true;
    }

    public void ResumeGame()
    {
        // Скрыть панель паузы
        pauseMenuPanel.alpha = 0;
        pauseMenuPanel.interactable = false;
        pauseMenuPanel.blocksRaycasts = false;
        pausePanel.SetActive(false);
        cursorManager.HideCursor();

        Time.timeScale = 1f; // Возобновить время
        isPaused = false;
    }

    // Метод для кнопки "Играть"
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
        EndGame();
        Debug.Log("Игра завершена.");
    }

    private void EndGame()
    {
        ResumeGame();

        pausePanel.SetActive(false);

        // Удаляем персонажа
        var player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            Destroy(player.gameObject);
        }
      
    }
}
