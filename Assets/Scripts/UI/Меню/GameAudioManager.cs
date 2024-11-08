using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AudioSettings
{
    public AudioSource audioSource; // ������ �� AudioSource
    public float baseVolume; // ������� ��������� ��� ����� ����� (��������, 0.2 ��� �����, 0.3 ��� �����)
}

public class GameAudioManager : MonoBehaviour
{
    // ������ � ����������� ��� ������� �����
    public AudioSettings[] audioSettings;

    // ������� ��� ���������� ����� ����������
    public Slider volumeSlider;

    // ���� ��� ���������� � �������� ��������� � PlayerPrefs
    private const string VolumePrefKey = "GameVolume";

    void Start()
    {
        // ��������� ����������� �������� ��������� ��� ������������� �� 20% �� ���������
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 20f);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume; // ������������� �������� �� ����������� ��������
            volumeSlider.onValueChanged.AddListener(UpdateVolume);
        }

        // ��������� ��������� �� ���� AudioSource �� ������
        SetVolume(savedVolume);
    }

    // ����� ��� ���������� ��������� ���� ������
    public void UpdateVolume(float sliderValue)
    {
        SetVolume(sliderValue);

        // ��������� ������� �������� ��������� � PlayerPrefs
        PlayerPrefs.SetFloat(VolumePrefKey, sliderValue);
    }

    // ����� ��� ��������� ��������� ��� ���� ������
    private void SetVolume(float sliderValue)
    {
        // ��������� ������� �������� � �������� �� 0 �� 1
        float globalVolumeFactor = sliderValue / 100f;

        foreach (var audioSetting in audioSettings)
        {
            if (audioSetting.audioSource != null)
            {
                // ��� ������� ����� ������������ ��� ��������� ��� (������� ��������� * ������� �� ��������)
                float newVolume = audioSetting.baseVolume * globalVolumeFactor;
                audioSetting.audioSource.volume = newVolume;
            }
        }
    }
}
