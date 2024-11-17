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

    // Ползунок громкости
    public Slider volumeSlider;

    // Источники звука для меню и игры
    public AudioSource menuMusicSource;
    public AudioSource gameMusicSource;

    public bool endlessMode = false;
    public Image checkMark;

    private void Start()
    {

        GameObject checkMarkObject = GameObject.Find("Галочка");

        if (checkMarkObject != null)
        {   
            checkMark = checkMarkObject.GetComponent<Image>();
            checkMark.gameObject.SetActive(false);
        }

        waveManager = FindObjectOfType<WaveManager>();
        LoadDifficulty();
        LoadEndlessMode();

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
        waveManager.projectile = 20f;

        // Сохраняем значения в PlayerPrefs
        PlayerPrefs.SetFloat("DamageMultiplier", waveManager.damageMultiplier);
        PlayerPrefs.SetFloat("HealthMultiplier", waveManager.healthMultiplier);
        PlayerPrefs.SetFloat("SpeedMultiplier", waveManager.speedMultiplier);
        PlayerPrefs.SetFloat("Projectile", waveManager.projectile);
        PlayerPrefs.SetInt("Difficulty", 1); // Сохраняем ID сложности
        PlayerPrefs.Save();
    }

    public void Normal()
    {
        waveManager.damageMultiplier = 15f;
        waveManager.healthMultiplier = 40f;
        waveManager.speedMultiplier = 0.5f;
        waveManager.projectile = 15f;

        // Сохраняем значения в PlayerPrefs
        PlayerPrefs.SetFloat("DamageMultiplier", waveManager.damageMultiplier);
        PlayerPrefs.SetFloat("HealthMultiplier", waveManager.healthMultiplier);
        PlayerPrefs.SetFloat("SpeedMultiplier", waveManager.speedMultiplier);
        PlayerPrefs.SetFloat("Projectile", waveManager.projectile);
        PlayerPrefs.SetInt("Difficulty", 2); // Сохраняем ID сложности
        PlayerPrefs.Save();
    }

    public void Easy()
    {
        waveManager.damageMultiplier = 10f;
        waveManager.healthMultiplier = 25f;
        waveManager.speedMultiplier = 0.5f;
        waveManager.projectile = 10f;

        // Сохраняем значения в PlayerPrefs
        PlayerPrefs.SetFloat("DamageMultiplier", waveManager.damageMultiplier);
        PlayerPrefs.SetFloat("HealthMultiplier", waveManager.healthMultiplier);
        PlayerPrefs.SetFloat("SpeedMultiplier", waveManager.speedMultiplier);
        PlayerPrefs.SetFloat("Projectile", waveManager.projectile);
        PlayerPrefs.SetInt("Difficulty", 3); // Сохраняем ID сложности
        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        // Загружаем сохраненные значения из PlayerPrefs
        if (PlayerPrefs.HasKey("DamageMultiplier"))
            waveManager.damageMultiplier = PlayerPrefs.GetFloat("DamageMultiplier");
        if (PlayerPrefs.HasKey("HealthMultiplier"))
            waveManager.healthMultiplier = PlayerPrefs.GetFloat("HealthMultiplier");
        if (PlayerPrefs.HasKey("SpeedMultiplier"))
            waveManager.speedMultiplier = PlayerPrefs.GetFloat("SpeedMultiplier");
        if (PlayerPrefs.HasKey("Projectile"))
            waveManager.projectile = PlayerPrefs.GetFloat("Projectile");

        // Загружаем сохраненную сложность
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
    private void LoadEndlessMode()
    {
        if (PlayerPrefs.HasKey("EndlessMode"))
        {
            endlessMode = PlayerPrefs.GetInt("EndlessMode") == 1;
            checkMark.gameObject.SetActive(endlessMode);
        }
        else
        {
            endlessMode = false;
            checkMark.gameObject.SetActive(false);
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

    public void EndlessMode()
    {
        if (endlessMode == false)
        {
            checkMark.gameObject.SetActive(true);
            endlessMode = true;
        }
        else
        {
            checkMark.gameObject.SetActive(false);
            endlessMode = false;
        }

        PlayerPrefs.SetInt("EndlessMode", endlessMode ? 1 : 0);
        PlayerPrefs.Save();
    }

}
