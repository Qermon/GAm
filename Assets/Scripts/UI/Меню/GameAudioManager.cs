using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AudioSettings
{
    public AudioSource audioSource; // Ссылка на AudioSource
    public float baseVolume; // Базовая громкость для этого звука (например, 0.2 для клика, 0.3 для урона)
}

public class GameAudioManager : MonoBehaviour
{
    // Массив с настройками для каждого звука
    public AudioSettings[] audioSettings;

    // Слайдер для управления общей громкостью
    public Slider volumeSlider;

    // Ключ для сохранения и загрузки громкости в PlayerPrefs
    private const string VolumePrefKey = "GameVolume";

    void Start()
    {
        // Загружаем сохраненное значение громкости или устанавливаем на 20% по умолчанию
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 20f);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume; // Устанавливаем ползунок на сохраненное значение
            volumeSlider.onValueChanged.AddListener(UpdateVolume);
        }

        // Применяем громкость ко всем AudioSource на старте
        SetVolume(savedVolume);
    }

    // Метод для обновления громкости всех звуков
    public void UpdateVolume(float sliderValue)
    {
        SetVolume(sliderValue);

        // Сохраняем текущее значение громкости в PlayerPrefs
        PlayerPrefs.SetFloat(VolumePrefKey, sliderValue);
    }

    // Метод для установки громкости для всех звуков
    private void SetVolume(float sliderValue)
    {
        // Переводим процент ползунка в значение от 0 до 1
        float globalVolumeFactor = sliderValue / 100f;

        foreach (var audioSetting in audioSettings)
        {
            if (audioSetting.audioSource != null)
            {
                // Для каждого звука рассчитываем его громкость как (базовая громкость * процент на ползунке)
                float newVolume = audioSetting.baseVolume * globalVolumeFactor;
                audioSetting.audioSource.volume = newVolume;
            }
        }
    }
}
