using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup mainMenuPanel; // CanvasGroup ��� ������ ����
    public GameObject canvasHero;
    private CursorManager cursorManager;

    private void Start()
    {
        cursorManager = FindObjectOfType<CursorManager>();
        // �������� ������ ��� ������� ����
        mainMenuPanel.alpha = 1;
        mainMenuPanel.interactable = true;
        mainMenuPanel.blocksRaycasts = true;
        Time.timeScale = 0f; // ������������� ���� ��� �������� ����
    }

    // ����� ��� ������ "������"
    public void OnPlayButtonPressed()
    {
        // ������ ������ ����
        mainMenuPanel.alpha = 0;
        mainMenuPanel.interactable = false;
        mainMenuPanel.blocksRaycasts = false;

        // ��������� ������ CanvasHero
        canvasHero.SetActive(true);
        Time.timeScale = 1f; // ������������ ����
    }

    // ����� ��� ������ "���������" (���� ������)
    public void OnSettingsButtonPressed()
    {
        Debug.Log("��������� ����� ��������� �����.");
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
