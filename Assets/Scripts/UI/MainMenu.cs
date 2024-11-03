using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup mainMenuPanel; // CanvasGroup для панели меню
    public GameObject canvasHero;

    private void Start()
    {
        // Показать панель при запуске игры
        mainMenuPanel.alpha = 1;
        mainMenuPanel.interactable = true;
        mainMenuPanel.blocksRaycasts = true;
        Time.timeScale = 0f; // Останавливаем игру при открытии меню
    }

    // Метод для кнопки "Играть"
    public void OnPlayButtonPressed()
    {
        // Прячем панель меню
        mainMenuPanel.alpha = 0;
        mainMenuPanel.interactable = false;
        mainMenuPanel.blocksRaycasts = false;

        // Открываем панель CanvasHero
        canvasHero.SetActive(true);

    }

    // Метод для кнопки "Настройки" (пока пустой)
    public void OnSettingsButtonPressed()
    {
        Debug.Log("Настройки будут добавлены позже.");
    }

    // Метод для кнопки "Выход"
    public void OnExitButtonPressed()
    {
        Application.Quit(); // Завершение игры
        Debug.Log("Игра завершена.");
    }
}
