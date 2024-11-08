using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    public Button hardButton;
    public Button normalButton;
    public Button easyButton;

    public Color activeColor = new Color(1f, 1f, 1f, 1f);
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.5f);
    private Button selectedButton;

    public GameObject settingsPanel;

    private WaveManager waveManager;

    // �������� ���������
    public Slider volumeSlider;

    // ��������� ����� ��� ���� � ����
    public AudioSource menuMusicSource;
    public AudioSource gameMusicSource;

    private void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        LoadDifficulty();

        hardButton.onClick.AddListener(() => SelectButton(hardButton, Hard, 1));
        normalButton.onClick.AddListener(() => SelectButton(normalButton, Normal, 2));
        easyButton.onClick.AddListener(() => SelectButton(easyButton, Easy, 3));

        InitializeVolumeSlider();
    }

    void SelectButton(Button button, System.Action difficultyAction, int difficultyId)
    {
        if (selectedButton == button) return;

        selectedButton = button;
        SetButtonTransparency();
        difficultyAction.Invoke();

        PlayerPrefs.SetInt("Difficulty", difficultyId);
        PlayerPrefs.Save();
    }

    void SetButtonTransparency()
    {
        hardButton.GetComponent<Image>().color = (selectedButton == hardButton) ? activeColor : inactiveColor;
        normalButton.GetComponent<Image>().color = (selectedButton == normalButton) ? activeColor : inactiveColor;
        easyButton.GetComponent<Image>().color = (selectedButton == easyButton) ? activeColor : inactiveColor;
    }

    public void Hard()
    {
        waveManager.damageMultiplier = 20f;
        waveManager.healthMultiplier = 80f;
        waveManager.speedMultiplier = 0.5f;
        waveManager.projectile = 1.35f;

        // ��������� �������� � PlayerPrefs
        PlayerPrefs.SetFloat("DamageMultiplier", waveManager.damageMultiplier);
        PlayerPrefs.SetFloat("HealthMultiplier", waveManager.healthMultiplier);
        PlayerPrefs.SetFloat("SpeedMultiplier", waveManager.speedMultiplier);
        PlayerPrefs.SetFloat("Projectile", waveManager.projectile);
        PlayerPrefs.SetInt("Difficulty", 1); // ��������� ID ���������
        PlayerPrefs.Save();
    }

    public void Normal()
    {
        waveManager.damageMultiplier = 15f;
        waveManager.healthMultiplier = 40f;
        waveManager.speedMultiplier = 0.5f;
        waveManager.projectile = 1.25f;

        // ��������� �������� � PlayerPrefs
        PlayerPrefs.SetFloat("DamageMultiplier", waveManager.damageMultiplier);
        PlayerPrefs.SetFloat("HealthMultiplier", waveManager.healthMultiplier);
        PlayerPrefs.SetFloat("SpeedMultiplier", waveManager.speedMultiplier);
        PlayerPrefs.SetFloat("Projectile", waveManager.projectile);
        PlayerPrefs.SetInt("Difficulty", 2); // ��������� ID ���������
        PlayerPrefs.Save();
    }

    public void Easy()
    {
        waveManager.damageMultiplier = 10f;
        waveManager.healthMultiplier = 25f;
        waveManager.speedMultiplier = 0.5f;
        waveManager.projectile = 1.2f;

        // ��������� �������� � PlayerPrefs
        PlayerPrefs.SetFloat("DamageMultiplier", waveManager.damageMultiplier);
        PlayerPrefs.SetFloat("HealthMultiplier", waveManager.healthMultiplier);
        PlayerPrefs.SetFloat("SpeedMultiplier", waveManager.speedMultiplier);
        PlayerPrefs.SetFloat("Projectile", waveManager.projectile);
        PlayerPrefs.SetInt("Difficulty", 3); // ��������� ID ���������
        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        // ��������� ����������� �������� �� PlayerPrefs
        if (PlayerPrefs.HasKey("DamageMultiplier"))
            waveManager.damageMultiplier = PlayerPrefs.GetFloat("DamageMultiplier");
        if (PlayerPrefs.HasKey("HealthMultiplier"))
            waveManager.healthMultiplier = PlayerPrefs.GetFloat("HealthMultiplier");
        if (PlayerPrefs.HasKey("SpeedMultiplier"))
            waveManager.speedMultiplier = PlayerPrefs.GetFloat("SpeedMultiplier");
        if (PlayerPrefs.HasKey("Projectile"))
            waveManager.projectile = PlayerPrefs.GetFloat("Projectile");

        // ��������� ����������� ���������
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            int difficulty = PlayerPrefs.GetInt("Difficulty");
            switch (difficulty)
            {
                case 1: Hard(); break;
                case 2: Normal(); break;
                case 3: Easy(); break;
            }
        }
    }


    public void Exit()
    {
        settingsPanel.SetActive(false);
    }

    void LoadDifficulty()
    {
        int difficultyId = PlayerPrefs.GetInt("Difficulty", 2);

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

    private void InitializeVolumeSlider()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.1f);
        volumeSlider.value = savedVolume;

        if (menuMusicSource != null) menuMusicSource.volume = savedVolume;
        if (gameMusicSource != null) gameMusicSource.volume = savedVolume;

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void SetVolume(float volume)
    {
        if (menuMusicSource != null) menuMusicSource.volume = volume;
        if (gameMusicSource != null) gameMusicSource.volume = volume;

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}
