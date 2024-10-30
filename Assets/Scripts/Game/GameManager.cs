using TMPro; // Используем TextMeshPro
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public WaveManager waveManager; // Ссылка на WaveManager
    public TMP_Text nextWaveTimerText; // Ссылка на текст для таймера следующей волны
    public TMP_Text waveNumberText;
    private static GameManager instance; // Синглтон для доступа из других классов


    void Awake()
    {
        // Создаем синглтон
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Не уничтожать при загрузке новой сцены
        }
        else
        {
            Destroy(gameObject); // Уничтожаем дублирующийся экземпляр
        }
    }



    void Start()
    {
        // Здесь можно инициализировать игру
        waveManager = FindObjectOfType<WaveManager>(); // Найти WaveManager на сцене
        if (waveManager != null)
        {
            StartGame(); // Запускаем игру, если WaveManager найден
        }
        else
        {
            Debug.LogError("WaveManager not found! Make sure it's present in the scene.");
        }
    }

    void Update()
    {
        // Обновление UI текстов
        UpdateWaveUI();
    }

    public void StartGame()
    {
        // Логика для запуска игры
        Debug.Log("Game has started!");
        waveManager.StartWave(); // Запускаем первую волну
    }


    void UpdateWaveUI()
    {
        if (waveManager.GetWaveNumber() > 0) // Убедитесь, что хотя бы одна волна прошла
        {
            float timeUntilNext = waveManager.GetTimeUntilNextWave();

            // Проверка на отрицательное значение
            if (timeUntilNext <= 0)
            {
                nextWaveTimerText.text = "0"; // Устанавливаем текст в "0" если время отрицательное или ноль
            }
            else
            {
                // Обновите UI с использованием timeUntilNext
                nextWaveTimerText.text = Mathf.Ceil(timeUntilNext).ToString();
            }

            waveNumberText.text = "Wave: " + waveManager.GetWaveNumber();
        }
    }


    public static GameManager GetInstance()
    {
        return instance; // Возвращаем текущий экземпляр GameManager
    }

    // Вы можете добавить другие методы для управления состоянием игры,
    // например, для паузы, перезапуска и т.д.
}
