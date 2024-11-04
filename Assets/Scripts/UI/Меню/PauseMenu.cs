using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    private Shop shop;
    private LevelUpMenu levelUpMenu;
    private WaveManager waveManager;
    private WeaponSelectionManager weaponSelectionManager;
    private MainMenu mainMenu;
    private CursorManager cursorManager;
    public CanvasGroup pauseMenuPanel; // CanvasGroup ��� ������ ����
    private bool isPaused = false;

    private void Start()
    {
        cursorManager = FindObjectOfType<CursorManager>();
        mainMenu = FindObjectOfType<MainMenu>();
        weaponSelectionManager = FindObjectOfType<WeaponSelectionManager>();
        levelUpMenu = FindAnyObjectByType<LevelUpMenu>();
        shop = FindAnyObjectByType<Shop>();
        waveManager = FindObjectOfType<WaveManager>();

        // ������ ������ ����� � ������ ����
        pauseMenuPanel.alpha = 0;
        pauseMenuPanel.interactable = false;
        pauseMenuPanel.blocksRaycasts = false;
    }

    private void Update()
    {
        // ��������� ������� ������� ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        // �������� ������ �����
        pauseMenuPanel.alpha = 1;
        pauseMenuPanel.interactable = true;
        pauseMenuPanel.blocksRaycasts = true;
        pausePanel.SetActive(true);
        cursorManager.ShowCursor();

        Time.timeScale = 0f; // ���������� �����
        isPaused = true;
    }

    public void ResumeGame()
    {
        // ������ ������ �����
        pauseMenuPanel.alpha = 0;
        pauseMenuPanel.interactable = false;
        pauseMenuPanel.blocksRaycasts = false;
        pausePanel.SetActive(false);
        cursorManager.HideCursor();

        Time.timeScale = 1f; // ����������� �����
        isPaused = false;
    }

    // ����� ��� ������ "������"
    public void OnPlayButtonPressed()
    {
        ResumeGame();
    }

    // ����� ��� ������ "���������" (���� ������)
    public void OnSettingsButtonPressed()
    {
        Debug.Log("��������� ����� ��������� �����.");
    }

    // ����� ��� ������ "�����"
    public void OnExitButtonPressed()
    {
        EndGame();
        Debug.Log("���� ���������.");
    }

    private void EndGame()
    {
        ResumeGame();

        pausePanel.SetActive(false);

        // ������� ���������
        var player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            Destroy(player.gameObject);
        }
      
    }
}
