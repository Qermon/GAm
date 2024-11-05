using TMPro; // Используем TextMeshPro
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class GameManager : MonoBehaviour
{
    public WaveManager waveManager; // Ссылка на WaveManager
    public TMP_Text nextWaveTimerText; // Ссылка на текст для таймера следующей волны
    public TMP_Text waveNumberText;
    private static GameManager instance; // Синглтон для доступа из других классов
    private MainMenu mainMenu;
    private bool isPaused = false; // Переменная для отслеживания состояния паузы
    private CursorManager cursorManager;
    public GameObject chestPanel; // Панель сундука в UI
    

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
        cursorManager = FindObjectOfType<CursorManager>();
        mainMenu = FindObjectOfType<MainMenu>();
        waveManager = FindObjectOfType<WaveManager>();
        nextWaveTimerText = GameObject.Find("NextWaveTimerText")?.GetComponent<TMP_Text>();
        waveNumberText = GameObject.Find("WaveNumberText")?.GetComponent<TMP_Text>();

        // Проверяем, найдены ли тексты
        if (nextWaveTimerText == null || waveNumberText == null)
        {
            Debug.LogError("Не удалось найти один или оба текстовых объекта для номера волны или таймера волны.");
        }

        // Инициализация игры
        waveManager = FindObjectOfType<WaveManager>(); // Найти WaveManager на сцене
        if (waveManager != null)
        {
            StartGame(); // Запускаем игру, если WaveManager найден
        }
        else
        {
            Debug.LogError("WaveManager не найден! Убедитесь, что он присутствует на сцене.");
        }
    }

    void Update()
    {
        if (mainMenu == null)
        {
            mainMenu = FindObjectOfType<MainMenu>();
        }

            if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }

            if (nextWaveTimerText == null || waveNumberText == null)
        {
            nextWaveTimerText = GameObject.Find("NextWaveTimerText")?.GetComponent<TMP_Text>();
            waveNumberText = GameObject.Find("WaveNumberText")?.GetComponent<TMP_Text>();
        }
        // Обновление UI текстов
        UpdateWaveUI();

    }

    public void StartGame()
    {
        // Логика для старта игры
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

    public void RestartGame()
    {
        // Перезагружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        cursorManager.ShowCursor();
    }

    public void RestartGameOrChest()
    {
        if (waveManager.waveNumber < 5)
        {
            RestartGameWithDelay();
        }
        else
        {
            chestPanel.SetActive(true);
        }
    }

    public void RestartGameWithDelay()
    {
        StartCoroutine(RestartGameCoroutine());
        Debug.Log("Перезапуск");
    }

    private IEnumerator RestartGameCoroutine()
    {
        // Ждем 2 секунды
        yield return new WaitForSeconds(2f);

        // Перезагружаем текущую сцену
        RestartGame();
    }
}
