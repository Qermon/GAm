using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup mainMenuPanel; // CanvasGroup ��� ������ ����
    public GameObject canvasHero;
    private CursorManager cursorManager;
    private SettingsPanelController settingsPanelController;
    public GameObject mainMenu;

    private void Start()
    {
        
        settingsPanelController = FindObjectOfType<SettingsPanelController>();
        cursorManager = FindObjectOfType<CursorManager>();
        // �������� ������ ��� ������� ����
        mainMenu.SetActive(true);
        Time.timeScale = 0f; // ������������� ���� ��� �������� ����
    }

    // ����� ��� ������ "������"
    public void OnPlayButtonPressed()
    {
        mainMenu.SetActive(false);
        // ��������� ������ CanvasHero
        canvasHero.SetActive(true);
        Time.timeScale = 1f; // ������������ ����
    }

    public void OnInventoryButtonPressed()
    {

    }    

    // ����� ��� ������ "���������" (���� ������)
    public void OnSettingsButtonPressed()
    {
        settingsPanelController.settingsPanel.SetActive(true); // �������� ������
    }

    // ����� ��� ������ "�����"
    public void OnExitButtonPressed()
    {
        Application.Quit(); // ���������� ����
        Debug.Log("���� ���������.");
    }

    // ����� ��� ������ ���� ����� ������ ������
    public void ShowMenuAfterDeath()
    {
        cursorManager.ShowCursor();
        mainMenuPanel.alpha = 1; // ���������� ������ ����
        mainMenuPanel.interactable = true;
        mainMenuPanel.blocksRaycasts = true;
        Time.timeScale = 0f; // ������������� ����
    }
}
