using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    public Button hardButton;
    public Button normalButton;
    public Button easyButton;

    public Button fps30Button;
    public Button fps60Button;
    public Button fps90Button;
    public Button fps120Button;

    public Button selectedFPSButton;

    public Color activeColor = new Color(1f, 1f, 1f, 1f);
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.5f);
    private Button selectedButton;

    public GameObject settingsPanel;

    private WaveManager waveManager;
    private PlayerHealth playerHealth;

    // Ползунок громкости
    public Slider volumeSlider;

    // Источники звука для меню и игры
    public AudioSource menuMusicSource;
    public AudioSource gameMusicSource;

    public bool endlessMode = false;
    public bool fpsCounter = true;
    public Image checkMark;
    public Image checkMarkFps;
    public FPSCounter fPSCounter;

    private void Start()
    {

        fps30Button.onClick.AddListener(() => SelectFPSButton(fps30Button, 30));
        fps60Button.onClick.AddListener(() => SelectFPSButton(fps60Button, 60));
        fps90Button.onClick.AddListener(() => SelectFPSButton(fps90Button, 90));
        fps120Button.onClick.AddListener(() => SelectFPSButton(fps120Button, 120));

        GameObject checkMarkObject = GameObject.Find("Галочка");

        if (checkMarkObject != null)
        {   
            checkMark = checkMarkObject.GetComponent<Image>();
            checkMark.gameObject.SetActive(false);
        }

        GameObject checkMarkFpsObject = GameObject.Find("Галочка1");

        if (checkMarkFpsObject != null)
        {
            checkMarkFps = checkMarkFpsObject.GetComponent<Image>();
            checkMarkFps.gameObject.SetActive(true);  // Активируем галочку для FPS
        }

        waveManager = FindObjectOfType<WaveManager>();
        LoadDifficulty();
        LoadEndlessMode();
        LoadFPSSetting();
        LoadFpsCounter();

        hardButton.onClick.AddListener(() => SelectButton(hardButton, Hard, 1));
        normalButton.onClick.AddListener(() => SelectButton(normalButton, Normal, 2));
        easyButton.onClick.AddListener(() => SelectButton(easyButton, Easy, 3));

        InitializeVolumeSlider();

        
    }

    void SelectFPSButton(Button button, int fps)
    {
        if (selectedFPSButton == button) return;

        selectedFPSButton = button;
        SetFPSButtonTransparency();
        SetMaxFPS(fps);

        // Сохраняем выбранное значение FPS в PlayerPrefs
        PlayerPrefs.SetInt("MaxFPS", fps);
        PlayerPrefs.Save();
    }
    void SetFPSButtonTransparency()
    {
        fps30Button.GetComponent<Image>().color = (selectedFPSButton == fps30Button) ? activeColor : inactiveColor;
        fps60Button.GetComponent<Image>().color = (selectedFPSButton == fps60Button) ? activeColor : inactiveColor;
        fps90Button.GetComponent<Image>().color = (selectedFPSButton == fps90Button) ? activeColor : inactiveColor;
        fps120Button.GetComponent<Image>().color = (selectedFPSButton == fps120Button) ? activeColor : inactiveColor;
    }

    void SetMaxFPS(int fps)
    {
        // Устанавливаем targetFrameRate
        Application.targetFrameRate = fps;

        // Добавим вывод в консоль для отладки, чтобы убедиться, что FPS устанавливается
        Debug.Log("FPS ограничен на: " + fps);
    }

    private void LoadFPSSetting()
    {
        int savedFPS = PlayerPrefs.GetInt("MaxFPS", 60); // Загружаем сохранённое значение FPS (по умолчанию 60)

        switch (savedFPS)
        {
            case 30:
                SelectFPSButton(fps30Button, 30);
                break;
            case 60:
                SelectFPSButton(fps60Button, 60);  // 60 FPS выбрана по умолчанию
                break;
            case 90:
                SelectFPSButton(fps90Button, 90);
                break;
            case 120:
                SelectFPSButton(fps120Button, 120);
                break;
            default:
                SelectFPSButton(fps60Button, 60);  // По умолчанию 60 FPS
                break;
        }

        // Обновляем цвета кнопок после выбора FPS
        SetFPSButtonTransparency();
        SetMaxFPS(savedFPS);
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

    private void LoadFpsCounter()
    {
        if (PlayerPrefs.HasKey("FpsCounterMode"))
        {
            fpsCounter = PlayerPrefs.GetInt("FpsCounterMode") == 1;
            checkMarkFps.gameObject.SetActive(fpsCounter);
        }
        else
        {
            fpsCounter = false;
            checkMarkFps.gameObject.SetActive(false);
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
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth == null)
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

    public void FpsCounterMode()
    {
        if (fpsCounter == false)
        {
            checkMarkFps.gameObject.SetActive(true);
            fpsCounter = true;
            fPSCounter.fpsText.gameObject.SetActive(true);
        }
        else
        {
            checkMarkFps.gameObject.SetActive(false);
            fpsCounter = false;
            fPSCounter.fpsText.gameObject.SetActive(false);
        }

        PlayerPrefs.SetInt("FpsCounterMode", fpsCounter ? 1 : 0);
        PlayerPrefs.Save();
    }

}
