using TMPro; // ���������� TextMeshPro
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class GameManager : MonoBehaviour
{
    public WaveManager waveManager; // ������ �� WaveManager
    public TMP_Text nextWaveTimerText; // ������ �� ����� ��� ������� ��������� �����
    public TMP_Text waveNumberText;
    private static GameManager instance; // �������� ��� ������� �� ������ �������
    private MainMenu mainMenu;
    private bool isPaused = false; // ���������� ��� ������������ ��������� �����
    private CursorManager cursorManager;
    public GameObject chestPanel; // ������ ������� � UI
    

    void Awake()
    {
        // ������� ��������
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ���������� ��� �������� ����� �����
        }
        else
        {
            Destroy(gameObject); // ���������� ������������� ���������
        }
    }

    void Start()
    {
        cursorManager = FindObjectOfType<CursorManager>();
        mainMenu = FindObjectOfType<MainMenu>();
        waveManager = FindObjectOfType<WaveManager>();
        nextWaveTimerText = GameObject.Find("NextWaveTimerText")?.GetComponent<TMP_Text>();
        waveNumberText = GameObject.Find("WaveNumberText")?.GetComponent<TMP_Text>();

        // ���������, ������� �� ������
        if (nextWaveTimerText == null || waveNumberText == null)
        {
            Debug.LogError("�� ������� ����� ���� ��� ��� ��������� ������� ��� ������ ����� ��� ������� �����.");
        }

        // ������������� ����
        waveManager = FindObjectOfType<WaveManager>(); // ����� WaveManager �� �����
        if (waveManager != null)
        {
            StartGame(); // ��������� ����, ���� WaveManager ������
        }
        else
        {
            Debug.LogError("WaveManager �� ������! ���������, ��� �� ������������ �� �����.");
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
        // ���������� UI �������
        UpdateWaveUI();

    }

    public void StartGame()
    {
        // ������ ��� ������ ����
    }

    void UpdateWaveUI()
    {
        if (waveManager.GetWaveNumber() > 0) // ���������, ��� ���� �� ���� ����� ������
        {
            float timeUntilNext = waveManager.GetTimeUntilNextWave();

            // �������� �� ������������� ��������
            if (timeUntilNext <= 0)
            {
                nextWaveTimerText.text = "0"; // ������������� ����� � "0" ���� ����� ������������� ��� ����
            }
            else
            {
                // �������� UI � �������������� timeUntilNext
                nextWaveTimerText.text = Mathf.Ceil(timeUntilNext).ToString();
            }

            waveNumberText.text = "Wave: " + waveManager.GetWaveNumber();
        }
    }

    public static GameManager GetInstance()
    {
        return instance; // ���������� ������� ��������� GameManager
    }

    public void RestartGame()
    {
        // ������������� ������� �����
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
        Debug.Log("����������");
    }

    private IEnumerator RestartGameCoroutine()
    {
        // ���� 2 �������
        yield return new WaitForSeconds(2f);

        // ������������� ������� �����
        RestartGame();
    }
}
