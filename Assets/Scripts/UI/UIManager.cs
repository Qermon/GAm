using TMPro; // ���������� TextMeshPro
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text waveNumberText;       // ������ �� ��������� ������� ��� ������ �����
    public TMP_Text nextWaveTimerText;    // ������ �� ��������� ������� ��� ������� ��������� �����

    private WaveManager waveManager;       // ������ �� WaveManager

    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        UpdateWaveNumber(); // ��������� ����� ����� � ������
    }

    void Update()
    {
        UpdateWaveNumber();
        UpdateNextWaveTimer();
    }

    void UpdateWaveNumber()
    {
        waveNumberText.text = "Wave: " + waveManager.GetWaveNumber(); // ���������� ������ ������ �����
    }

    void UpdateNextWaveTimer()
    {
        float timeUntilNextWave = waveManager.GetTimeUntilNextWave();
        if (timeUntilNextWave >= 0)
        {
            nextWaveTimerText.text = "Next Wave: " + Mathf.Max(Mathf.Ceil(timeUntilNextWave), 0) + "s"; // ���������, ��� ����� �� �������������
        }
        else
        {
            nextWaveTimerText.text = "Next Wave: Ready!";
        }
    }
}
