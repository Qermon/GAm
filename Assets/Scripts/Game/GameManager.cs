using TMPro; // ���������� TextMeshPro
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI; // ��� ������ � Image


public class GameManager : MonoBehaviour
{
    public WaveManager waveManager; // ������ �� WaveManager
    public TMP_Text nextWaveTimerText; // ������ �� ����� ��� ������� ��������� �����
    public TMP_Text waveNumberText;
    public TMP_Text kill;
    private static GameManager instance; // �������� ��� ������� �� ������ �������
    private MainMenu mainMenu;
    private bool isPaused = false; // ���������� ��� ������������ ��������� �����
    private CursorManager cursorManager;

    public Image hpBarImage; // ������� ����������� HP-����
    public Image hpBarBackgroundImage; // ����������� ���� HP-����



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
        kill = GameObject.Find("Kill")?.GetComponent<TMP_Text>();

        // ���������, ������� �� ������
        if (nextWaveTimerText == null || waveNumberText == null)
        {
            Debug.LogError("�� ������� ����� ���� ��� ��� ��������� ������� ��� ������ ����� ��� ������� �����.");
        }

        waveManager = FindObjectOfType<WaveManager>(); // ����� WaveManager �� �����
        if (waveManager != null)
        {
            StartGame(); // ��������� ����, ���� WaveManager ������
        }
        else
        {
            Debug.LogError("WaveManager �� ������! ���������, ��� �� ������������ �� �����.");
        }


        GameObject hpBarObject = GameObject.Find("foregroundBoss"); // ����� "HPBarName" - ��� ��� ������ ������� Image
        GameObject hpBarObject1 = GameObject.Find("backgroundBoss"); // ����� "HPBarName" - ��� ��� ������ ������� Image

        if (hpBarObject != null && hpBarObject1 != null)
        {
            // ����������� ��� � ��������� Image
            hpBarImage = hpBarObject.GetComponent<Image>();
            hpBarBackgroundImage = hpBarObject1.GetComponent<Image>();

        }

        if (hpBarImage != null && hpBarBackgroundImage != null)
        {
            hpBarImage.gameObject.SetActive(false); // �������� HP-��� �� 10-� �����
            hpBarBackgroundImage.gameObject.SetActive(false); // �������� ��� HP-����
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
            kill = GameObject.Find("Kill")?.GetComponent<TMP_Text>();
        }
        // ���������� UI �������
        UpdateWaveUI();

    }

    public void KilledBoss()
    {
        hpBarImage.gameObject.SetActive(false); // �������� HP-���
        hpBarBackgroundImage.gameObject.SetActive(false); // �������� ��� HP-����
    }

    public void SpawnBoss()
    {
        hpBarImage.gameObject.SetActive(true); // ���������� HP-���
        hpBarBackgroundImage.gameObject.SetActive(true); // ���������� ��� HP-����
    }

    public void StartGame()
    {
        // ������ ��� ������ ����
    }

    void UpdateWaveUI()
    {
        if (waveManager.GetWaveNumber() > 0) // ���������, ��� ���� �� ���� ����� ������
        {
            if (waveManager.GetWaveNumber() == 10)
            {

                nextWaveTimerText.text = "����";
            }
            else if (waveManager.GetWaveNumber() % 3 == 0) // ������ ������ �����
            {
                // ���������� UI � ����������� ���������� �������
                int remainingKills = waveManager.killThreshold - waveManager.killCount;
                nextWaveTimerText.text = remainingKills.ToString();
            }
            else
            {
                // ���� ����� �� �� �������� � ���������� ����� �� ��������� �����
                float timeUntilNext = waveManager.GetTimeUntilNextWave();

                // �������� �� ������������� ��������
                if (timeUntilNext <= 0)
                {
                    nextWaveTimerText.text = "0"; // ������������� ����� � "0" ���� ����� ������������� ��� ����
                }
                else
                {
                    // ���������� UI � �������������� timeUntilNext
                    nextWaveTimerText.text = Mathf.Ceil(timeUntilNext).ToString();
                }
            }

            waveNumberText.text = "�����: " + waveManager.GetWaveNumber();
            kill.text = waveManager.killCountAll.ToString();
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
