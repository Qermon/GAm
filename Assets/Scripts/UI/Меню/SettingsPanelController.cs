using UnityEngine;
using UnityEngine.UI; // Для работы с UI элементами

public class SettingsPanelController : MonoBehaviour
{
    // UI элементы для сложности
    public Button hardButton;
    public Button normalButton;
    public Button easyButton;

    public Color activeColor = new Color(1f, 1f, 1f, 1f); // Полная непрозрачность
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.5f); // Полупрозрачность
    private Button selectedButton; // Кнопка, которая была выбрана

    // Панель настроек
    public GameObject settingsPanel; // Ссылка на объект панели настроек

    private WaveManager waveManager;

    // Применение изменений
    private void Start()
    {
        // Инициализация WaveManager
        waveManager = FindObjectOfType<WaveManager>();

        // Загружаем сохраненную сложность
        LoadDifficulty();

        // Устанавливаем обработчики кнопок сложности
        hardButton.onClick.AddListener(() => SelectButton(hardButton, Hard, 1));
        normalButton.onClick.AddListener(() => SelectButton(normalButton, Normal, 2));
        easyButton.onClick.AddListener(() => SelectButton(easyButton, Easy, 3));
    }

    // Выбор сложности
    void SelectButton(Button button, System.Action difficultyAction, int difficultyId)
    {
        // Если выбранная кнопка уже та же, ничего не делаем
        if (selectedButton == button) return;

        // Изменяем выбор на новую кнопку
        selectedButton = button;
        SetButtonTransparency();
        difficultyAction.Invoke(); // Применяем выбранную сложность

        // Сохраняем выбранную сложность
        PlayerPrefs.SetInt("Difficulty", difficultyId);
        PlayerPrefs.Save(); // Сохраняем изменения
    }

    // Устанавливаем прозрачность для кнопок
    void SetButtonTransparency()
    {
        hardButton.GetComponent<Image>().color = (selectedButton == hardButton) ? activeColor : inactiveColor;
        normalButton.GetComponent<Image>().color = (selectedButton == normalButton) ? activeColor : inactiveColor;
        easyButton.GetComponent<Image>().color = (selectedButton == easyButton) ? activeColor : inactiveColor;
    }

    // Применение сложности "Hard"
    public void Hard()
    {
        waveManager.damageMultiplier = 1.2f;
        waveManager.healthMultiplier = 1.4f;
        waveManager.speedMultiplier = 0.5f;
        waveManager.projectile = 1.25f;
    }

    // Применение сложности "Normal"
    public void Normal()
    {
        waveManager.damageMultiplier = 1.15f;
        waveManager.healthMultiplier = 1.25f;
        waveManager.speedMultiplier = 0.5f;
        waveManager.projectile = 1.2f;
    }

    // Применение сложности "Easy"
    public void Easy()
    {
        waveManager.damageMultiplier = 1.1f;
        waveManager.healthMultiplier = 1.15f;
        waveManager.speedMultiplier = 0.5f;
        waveManager.projectile = 1.13f;
    }

    // Метод для закрытия панели настроек
    public void Exit()
    {
        settingsPanel.SetActive(false); // Скрываем панель
    }

    // Загружаем сохраненную сложность
    void LoadDifficulty()
    {
        int difficultyId = PlayerPrefs.GetInt("Difficulty", 2); // Если сложность не была сохранена, по умолчанию - Normal (2)

        // В зависимости от сохраненной сложности, активируем нужную кнопку
        switch (difficultyId)
        {
            case 1:
                SelectButton(hardButton, Hard, 1);
                break;
            case 2:
                SelectButton(normalButton, Normal, 2);
                break;
            case 3:
                SelectButton(easyButton, Easy, 3);
                break;
        }
    }
}
