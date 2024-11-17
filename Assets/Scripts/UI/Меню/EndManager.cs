using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndManager : MonoBehaviour
{
    public GameObject EndMenu;
    private Shop shop;
    private WaveManager waveManager;
    private MainMenu mainMenu;
    private GameManager gameManager;
    private SettingsPanelController settingsPanelController;
    public TMP_Text winner;
    public TMP_Text lose;
    public TMP_Text record;
    public TMP_Text waveNumber;
    public PlayerHealth playerHealth;

    public int maxWaveRecord; // Переменная для хранения рекорда

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        settingsPanelController = FindObjectOfType<SettingsPanelController>();
        gameManager = FindObjectOfType<GameManager>();
        mainMenu = FindObjectOfType<MainMenu>();
        shop = FindAnyObjectByType<Shop>();
        waveManager = FindObjectOfType<WaveManager>();

        // Загружаем сохранённый рекорд
        maxWaveRecord = PlayerPrefs.GetInt("MaxWaveRecord", 0);
    }

    void Update()
    {
        
    }

    public void OnExitButtonPressed()
    {
        gameManager.RestartGame();
    }

    public void EndMenuOpen()
    {
        if (!EndMenu.activeSelf)
        {
            UpdateUI();
            EndMenu.gameObject.SetActive(true);
            GetPlayerHealth();
            Screensaver();
            Time.timeScale = 0f;
        }
    }

    public void Screensaver()
    {
        if (!settingsPanelController.endlessMode && playerHealth != null && playerHealth.currentHealth > 0)
        {
            winner.gameObject.SetActive(true);
            lose.gameObject.SetActive(false);
            record.gameObject.SetActive(false);
        }
        else if (!settingsPanelController.endlessMode && (playerHealth == null || playerHealth.currentHealth <= 0))
        {
            winner.gameObject.SetActive(false);
            lose.gameObject.SetActive(true);
            record.gameObject.SetActive(false);
        }
        else
        {
            winner.gameObject.SetActive(false);
            lose.gameObject.SetActive(false);
            record.gameObject.SetActive(true);
        }

        // Обновляем рекорд
        if (waveManager != null && waveManager.waveNumber > maxWaveRecord)
        {
            maxWaveRecord = waveManager.waveNumber;
            PlayerPrefs.SetInt("MaxWaveRecord", maxWaveRecord);
        }
    }

    private PlayerHealth GetPlayerHealth()
    {
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        return playerHealth;
    }

    private void UpdateUI()
    {
        if (waveManager != null)
        {
            // Обновляем отображение текущей волны
            waveNumber.text = "РЕЗУЛЬТАТ: " + waveManager.waveNumber;

            // Обновляем отображение рекорда
            record.text = "РЕКОРД: " + maxWaveRecord;
        }
    }
}
