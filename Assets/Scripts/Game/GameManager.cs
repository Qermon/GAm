using TMPro; // ���������� TextMeshPro
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public WaveManager waveManager; // ������ �� WaveManager
    public TMP_Text nextWaveTimerText; // ������ �� ����� ��� ������� ��������� �����
    public TMP_Text waveNumberText;
    private static GameManager instance; // �������� ��� ������� �� ������ �������


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
        // ����� ����� ���������������� ����
        waveManager = FindObjectOfType<WaveManager>(); // ����� WaveManager �� �����
        if (waveManager != null)
        {
            StartGame(); // ��������� ����, ���� WaveManager ������
        }
        else
        {
            Debug.LogError("WaveManager not found! Make sure it's present in the scene.");
        }
    }

    void Update()
    {
        // ���������� UI �������
        UpdateWaveUI();
    }

    public void StartGame()
    {
        // ������ ��� ������� ����
        Debug.Log("Game has started!");
        waveManager.StartWave(); // ��������� ������ �����
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

    // �� ������ �������� ������ ������ ��� ���������� ���������� ����,
    // ��������, ��� �����, ����������� � �.�.
}
