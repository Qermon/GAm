using TMPro; // Используем TextMeshPro
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text waveNumberText;       // Ссылка на текстовый элемент для номера волны
    public TMP_Text nextWaveTimerText;    // Ссылка на текстовый элемент для таймера следующей волны

    private WaveManager waveManager;       // Ссылка на WaveManager

    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        UpdateWaveNumber(); // Обновляем номер волны в начале
    }

    void Update()
    {
        UpdateWaveNumber();
        UpdateNextWaveTimer();
    }

    void UpdateWaveNumber()
    {
        waveNumberText.text = "Wave: " + waveManager.GetWaveNumber(); // Обновление текста номера волны
    }

    void UpdateNextWaveTimer()
    {
        float timeUntilNextWave = waveManager.GetTimeUntilNextWave();
        if (timeUntilNextWave >= 0)
        {
            nextWaveTimerText.text = "Next Wave: " + Mathf.Max(Mathf.Ceil(timeUntilNextWave), 0) + "s"; // Убедитесь, что время не отрицательное
        }
        else
        {
            nextWaveTimerText.text = "Next Wave: Ready!";
        }
    }
}
