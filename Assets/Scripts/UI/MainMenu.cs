using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup mainMenuPanel; // CanvasGroup для панели меню
    public GameObject canvasHero;
    private CursorManager cursorManager;
    private SettingsPanelController settingsPanelController;
    public GameObject mainMenu;

    private void Start()
    {
        
        settingsPanelController = FindObjectOfType<SettingsPanelController>();
        cursorManager = FindObjectOfType<CursorManager>();
        // Показать панель при запуске игры
        mainMenu.SetActive(true);
        Time.timeScale = 0f; // Останавливаем игру при открытии меню
    }

    // Метод для кнопки "Играть"
    public void OnPlayButtonPressed()
    {
        mainMenu.SetActive(false);
        // Открываем панель CanvasHero
        canvasHero.SetActive(true);
        Time.timeScale = 1f; // Возобновляем игру
    }

    public void OnInventoryButtonPressed()
    {

    }    

    // Метод для кнопки "Настройки" (пока пустой)
    public void OnSettingsButtonPressed()
    {
        settingsPanelController.settingsPanel.SetActive(true); // Скрываем панель
    }

    // Метод для кнопки "Выход"
    public void OnExitButtonPressed()
    {
        Application.Quit(); // Завершение игры
        Debug.Log("Игра завершена.");
    }

    // Метод для показа меню после смерти игрока
    public void ShowMenuAfterDeath()
    {
        cursorManager.ShowCursor();
        mainMenuPanel.alpha = 1; // Показываем панель меню
        mainMenuPanel.interactable = true;
        mainMenuPanel.blocksRaycasts = true;
        Time.timeScale = 0f; // Останавливаем игру
    }
}
